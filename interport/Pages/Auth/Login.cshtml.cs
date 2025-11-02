using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using interport.Models;

namespace interport.Web.Pages.Auth;

public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signIn;
    public LoginModel(SignInManager<ApplicationUser> signIn) => _signIn = signIn;

    [BindProperty] public InputModel Input { get; set; } = new();
    public class InputModel { public string Email { get; set; } = ""; public string Password { get; set; } = ""; public bool RememberMe { get; set; } }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        if (!ModelState.IsValid) return Page();
        var result = await _signIn.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded) return LocalRedirect(returnUrl ?? Url.Content("~/"));
        ModelState.AddModelError(string.Empty, "Invalid login.");
        return Page();
    }
}
