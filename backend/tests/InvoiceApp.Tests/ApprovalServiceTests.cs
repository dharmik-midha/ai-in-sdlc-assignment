using InvoiceApp.Domain;
using Xunit;

namespace InvoiceApp.Tests;

public class ApprovalServiceTests
{
    private static Invoice InvoiceWith(string id, InvoiceStatus status, decimal amount = 100m) =>
        new() { Id = id, CustomerName = "Test Corp", Amount = amount, Status = status };

    private static ApprovalService ServiceFor(params Invoice[] invoices) =>
        new(new FakeInvoiceRepository(invoices));

    [Fact]
    public void Submit_MovesDraftToPendingApproval()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.Draft);

        var result = ServiceFor(invoice).Submit("INV-1");

        Assert.Equal(InvoiceStatus.PendingApproval, result.Status);
    }

    [Fact]
    public void Submit_RejectsInvoiceThatIsNotADraft()
    {
        var service = ServiceFor(InvoiceWith("INV-1", InvoiceStatus.Approved));

        Assert.Throws<InvalidOperationException>(() => service.Submit("INV-1"));
    }

    [Fact]
    public void Reject_RecordsReasonAndApprover()
    {
        var service = ServiceFor(InvoiceWith("INV-1", InvoiceStatus.PendingApproval));

        var result = service.Reject("INV-1", "demo.approver", "Duplicate invoice");

        Assert.Equal(InvoiceStatus.Rejected, result.Status);
        Assert.Equal("Duplicate invoice", result.DecisionNote);
    }

    [Fact]
    public void Reject_RequiresAReason()
    {
        var service = ServiceFor(InvoiceWith("INV-1", InvoiceStatus.PendingApproval));

        Assert.Throws<ArgumentException>(() => service.Reject("INV-1", "demo.approver", "  "));
    }

    // === Single-Approver Limit Tests (Task 3) ===
    
    [Fact]
    public void Manager_CanApprove_UpTo50k()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval, 50_000m);
        var service = ServiceFor(invoice);

        var result = service.Approve("INV-1", "manager.alice", ApproverRole.Manager);

        Assert.Equal(InvoiceStatus.Approved, result.Status);
        Assert.Equal("manager.alice", result.DecidedBy);
    }
    
    [Fact]
    public void Manager_CannotApprove_Over50k()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval, 50_001m);
        var service = ServiceFor(invoice);

        var ex = Assert.Throws<InvalidOperationException>(
            () => service.Approve("INV-1", "manager.alice", ApproverRole.Manager));
        
        Assert.Contains("Manager role can only approve invoices up to $50,000", ex.Message);
        Assert.Contains("$50,001.00", ex.Message);
        
        // Verify invoice unchanged
        Assert.Equal(InvoiceStatus.PendingApproval, invoice.Status);
    }
    
    [Fact]
    public void Director_CanApprove_UpTo500k()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval, 500_000m);
        var service = ServiceFor(invoice);

        var result = service.Approve("INV-1", "director.bob", ApproverRole.Director);

        Assert.Equal(InvoiceStatus.Approved, result.Status);
        Assert.Equal("director.bob", result.DecidedBy);
    }
    
    [Fact]
    public void Director_CanApprove_At500k()
    {
        // At exactly $500k limit - this should succeed  
        var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval, 500_000m);
        var service = ServiceFor(invoice);

        var result = service.Approve("INV-1", "director.bob", ApproverRole.Director);

        Assert.Equal(InvoiceStatus.Approved, result.Status);
    }
    
    [Fact]
    public void Admin_CanApprove_AnyAmount()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval, 1_000_000m);
        var service = ServiceFor(invoice);

        var result = service.Approve("INV-1", "admin.charlie", ApproverRole.Admin);

        Assert.Equal(InvoiceStatus.Approved, result.Status);
        Assert.Equal("admin.charlie", result.DecidedBy);
    }
    
    // === Two-Approver Tests (Task 4) ===
    
    [Fact]
    public void TwoApprover_ManagerThenDirector_Succeeds()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval, 750_000m);
        var service = ServiceFor(invoice);

        // First approval by Manager
        var result1 = service.Approve("INV-1", "manager.alice", ApproverRole.Manager);
        Assert.Equal(InvoiceStatus.PendingApproval, result1.Status);
        Assert.Equal("manager.alice", result1.FirstApprovedBy);
        Assert.Null(result1.SecondApprovedBy);

        // Second approval by Director
        var result2 = service.Approve("INV-1", "director.bob", ApproverRole.Director);
        Assert.Equal(InvoiceStatus.Approved, result2.Status);
        Assert.Equal("manager.alice", result2.FirstApprovedBy);
        Assert.Equal("director.bob", result2.SecondApprovedBy);
        Assert.Equal("director.bob", result2.DecidedBy);
    }
    
    [Fact]
    public void TwoApprover_DirectorThenDirector_Succeeds()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval, 600_000m);
        var service = ServiceFor(invoice);

        // First approval by Director
        var result1 = service.Approve("INV-1", "director.alice", ApproverRole.Director);
        Assert.Equal(InvoiceStatus.PendingApproval, result1.Status);
        Assert.Equal("director.alice", result1.FirstApprovedBy);

        // Second approval by another Director
        var result2 = service.Approve("INV-1", "director.bob", ApproverRole.Director);
        Assert.Equal(InvoiceStatus.Approved, result2.Status);
        Assert.Equal("director.alice", result2.FirstApprovedBy);
        Assert.Equal("director.bob", result2.SecondApprovedBy);
    }
    
    [Fact]
    public void TwoApprover_SameUserTwice_Fails()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval, 800_000m);
        var service = ServiceFor(invoice);

        // First approval
        service.Approve("INV-1", "director.bob", ApproverRole.Director);

        // Second approval by same user - should fail
        var ex = Assert.Throws<InvalidOperationException>(
            () => service.Approve("INV-1", "director.bob", ApproverRole.Director));
        
        Assert.Contains("Same user cannot approve an invoice twice", ex.Message);
        Assert.Contains("director.bob", ex.Message);
        
        // Verify invoice still pending
        Assert.Equal(InvoiceStatus.PendingApproval, invoice.Status);
    }
    
    [Fact]
    public void TwoApprover_ManagerThenManager_Fails()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval, 600_000m);
        var service = ServiceFor(invoice);

        // First approval by Manager
        service.Approve("INV-1", "manager.alice", ApproverRole.Manager);

        // Second approval by another Manager - should fail
        var ex = Assert.Throws<InvalidOperationException>(
            () => service.Approve("INV-1", "manager.bob", ApproverRole.Manager));
        
        Assert.Contains("Second approver must be Director or Admin", ex.Message);
        Assert.Contains("Manager", ex.Message);
        
        // Verify invoice still pending
        Assert.Equal(InvoiceStatus.PendingApproval, invoice.Status);
    }
    
    [Fact]
    public void TwoApprover_AdminCanBeSecondApprover()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval, 900_000m);
        var service = ServiceFor(invoice);

        // First approval by Manager
        service.Approve("INV-1", "manager.alice", ApproverRole.Manager);

        // Second approval by Admin - should succeed
        var result = service.Approve("INV-1", "admin.charlie", ApproverRole.Admin);
        
        Assert.Equal(InvoiceStatus.Approved, result.Status);
        Assert.Equal("manager.alice", result.FirstApprovedBy);
        Assert.Equal("admin.charlie", result.SecondApprovedBy);
    }
    
    [Fact]
    public void TwoApprover_RejectionClearsPreviousApproval()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval, 750_000m);
        var service = ServiceFor(invoice);

        // First approval
        service.Approve("INV-1", "manager.alice", ApproverRole.Manager);
        Assert.Equal("manager.alice", invoice.FirstApprovedBy);

        // Rejection by another user
        var result = service.Reject("INV-1", "director.bob", "Budget constraints");
        
        Assert.Equal(InvoiceStatus.Rejected, result.Status);
        Assert.Equal("Budget constraints", result.DecisionNote);
        Assert.Equal("director.bob", result.DecidedBy);
        
        // FirstApprovedBy should remain unchanged (audit trail)
        Assert.Equal("manager.alice", result.FirstApprovedBy);
    }
}
