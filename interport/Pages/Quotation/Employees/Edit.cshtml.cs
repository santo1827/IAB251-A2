using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace interport.Pages.Quotation.Employees;

[Authorize(Policy = "OfficerOnly")]
public class EditModel : PageModel
{
    public void OnGet() { }
}