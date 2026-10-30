using InvoiceApp.Api.Auth;
using InvoiceApp.Domain;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[RequireRole("Approver", "Admin")]
public class ApprovalsController : ApiControllerBase
{
    private readonly ApprovalService _approvals;

    public ApprovalsController(ApprovalService approvals)
    {
        _approvals = approvals;
    }

    public record RejectRequest(string Reason);

    [HttpPost("{invoiceId}/approve")]
    public ActionResult<Invoice> Approve(string invoiceId)
    {
        var session = HttpContext.GetSession();
        var approverRole = ParseRole(session.Role);
        return Handle(() => _approvals.Approve(invoiceId, session.Username, approverRole));
    }

    [HttpPost("{invoiceId}/reject")]
    public ActionResult<Invoice> Reject(string invoiceId, [FromBody] RejectRequest request) =>
        Handle(() => _approvals.Reject(invoiceId, HttpContext.GetSession().Username, request.Reason));
    
    private static ApproverRole ParseRole(string role)
    {
        return role.ToLowerInvariant() switch
        {
            "manager" => ApproverRole.Manager,
            "director" => ApproverRole.Director,
            "admin" => ApproverRole.Admin,
            "approver" => ApproverRole.Manager, // Map demo role to Manager
            _ => ApproverRole.Manager // Default for safety
        };
    }
}
