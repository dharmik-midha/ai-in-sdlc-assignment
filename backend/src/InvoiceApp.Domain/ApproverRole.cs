namespace InvoiceApp.Domain;

/// <summary>
/// Defines approval authority levels for role-based access control.
/// </summary>
public enum ApproverRole
{
    /// <summary>
    /// Manager: Can approve invoices up to $50,000
    /// </summary>
    Manager,
    
    /// <summary>
    /// Director: Can approve invoices up to $500,000
    /// </summary>
    Director,
    
    /// <summary>
    /// Admin: Can approve any amount
    /// </summary>
    Admin
}
