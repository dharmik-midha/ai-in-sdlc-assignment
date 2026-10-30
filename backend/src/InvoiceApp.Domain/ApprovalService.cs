namespace InvoiceApp.Domain;

/// <summary>
/// Moves an invoice through its lifecycle: Draft -> PendingApproval -> Approved or Rejected.
/// Enforces role-based approval limits.
/// </summary>
public class ApprovalService
{
    private readonly IInvoiceRepository _repository;
    
    private const decimal ManagerApprovalLimit = 50_000m;
    private const decimal DirectorApprovalLimit = 500_000m;

    public ApprovalService(IInvoiceRepository repository)
    {
        _repository = repository;
    }

    public Invoice Submit(string invoiceId)
    {
        var invoice = Find(invoiceId);

        if (invoice.Status != InvoiceStatus.Draft)
        {
            throw new InvalidOperationException(
                $"Invoice {invoiceId} is {invoice.Status}; only drafts can be submitted.");
        }

        invoice.Status = InvoiceStatus.PendingApproval;
        _repository.Update(invoice);
        return invoice;
    }

    public Invoice Approve(string invoiceId, string approver, ApproverRole approverRole)
    {
        var invoice = Find(invoiceId);
        EnsurePending(invoice);
        
        // For invoices >$500k, require two distinct approvers (except Admin as first approver)
        if (invoice.Amount > DirectorApprovalLimit)
        {
            // Admin can approve immediately if they are the first approver
            if (approverRole == ApproverRole.Admin && string.IsNullOrEmpty(invoice.FirstApprovedBy))
            {
                invoice.Status = InvoiceStatus.Approved;
                invoice.DecidedBy = approver;
                invoice.DecidedAt = DateTime.UtcNow;
                invoice.FirstApprovedBy = approver;
                invoice.FirstApprovedAt = DateTime.UtcNow;
                _repository.Update(invoice);
                return invoice;
            }
            
            // Otherwise follow two-approver flow
            return HandleTwoApproverApproval(invoice, approver, approverRole);
        }
        
        // Single-approver validation for invoices <=$500k
        ValidateApprovalAuthority(invoice, approverRole);

        invoice.Status = InvoiceStatus.Approved;
        invoice.DecidedBy = approver;
        invoice.DecidedAt = DateTime.UtcNow;
        invoice.FirstApprovedBy = approver;
        invoice.FirstApprovedAt = DateTime.UtcNow;
        _repository.Update(invoice);
        return invoice;
    }
    
    private Invoice HandleTwoApproverApproval(Invoice invoice, string approver, ApproverRole approverRole)
    {
        // First approval
        if (string.IsNullOrEmpty(invoice.FirstApprovedBy))
        {
            // First approver can be Manager, Director, or Admin
            invoice.FirstApprovedBy = approver;
            invoice.FirstApprovedAt = DateTime.UtcNow;
            _repository.Update(invoice);
            return invoice;
        }
        
        // Second approval - validate constraints
        
        // Rule 1: Second approver must be different from first
        if (invoice.FirstApprovedBy.Equals(approver, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Same user cannot approve an invoice twice. " +
                $"First approver: {invoice.FirstApprovedBy}, Second approver: {approver}.");
        }
        
        // Rule 2: Second approver must be Director or Admin
        if (approverRole == ApproverRole.Manager)
        {
            throw new InvalidOperationException(
                $"Second approver must be Director or Admin for invoices above ${DirectorApprovalLimit:N0}. " +
                $"Current approver role: {approverRole}.");
        }
        
        // Second approval completes the process
        invoice.Status = InvoiceStatus.Approved;
        invoice.DecidedBy = approver;
        invoice.DecidedAt = DateTime.UtcNow;
        invoice.SecondApprovedBy = approver;
        invoice.SecondApprovedAt = DateTime.UtcNow;
        _repository.Update(invoice);
        return invoice;
    }

    public Invoice Reject(string invoiceId, string approver, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("A reason is required to reject an invoice.", nameof(reason));
        }

        var invoice = Find(invoiceId);
        EnsurePending(invoice);

        invoice.Status = InvoiceStatus.Rejected;
        invoice.DecidedBy = approver;
        invoice.DecidedAt = DateTime.UtcNow;
        invoice.DecisionNote = reason;
        _repository.Update(invoice);
        return invoice;
    }
    
    private void ValidateApprovalAuthority(Invoice invoice, ApproverRole role)
    {
        // Admin has unlimited approval authority
        if (role == ApproverRole.Admin)
        {
            return;
        }
        
        // Manager can approve up to $50,000
        if (role == ApproverRole.Manager && invoice.Amount > ManagerApprovalLimit)
        {
            throw new InvalidOperationException(
                $"Manager role can only approve invoices up to ${ManagerApprovalLimit:N0}. " +
                $"This invoice amount is ${invoice.Amount:N2}.");
        }
        
        // Director can approve up to $500,000
        if (role == ApproverRole.Director && invoice.Amount > DirectorApprovalLimit)
        {
            throw new InvalidOperationException(
                $"Director role can only approve invoices up to ${DirectorApprovalLimit:N0}. " +
                $"This invoice amount is ${invoice.Amount:N2}.");
        }
    }

    private Invoice Find(string invoiceId) =>
        _repository.GetById(invoiceId)
        ?? throw new KeyNotFoundException($"Invoice {invoiceId} was not found.");

    private static void EnsurePending(Invoice invoice)
    {
        if (invoice.Status != InvoiceStatus.PendingApproval)
        {
            throw new InvalidOperationException(
                $"Invoice {invoice.Id} is {invoice.Status}; only pending invoices can be decided.");
        }
    }
}
