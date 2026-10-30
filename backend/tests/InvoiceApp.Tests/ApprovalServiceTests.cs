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
}
