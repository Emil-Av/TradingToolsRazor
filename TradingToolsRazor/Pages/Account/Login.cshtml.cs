using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.RequestModels;
using System.ComponentModel.DataAnnotations;

namespace TradingToolsRazor.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public AccountInputModel AccountInput { get; set; } = new AccountInputModel();

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            // Clear any error messages when loading the page fresh
            ModelState.Clear();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Page("/Home/Index");

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(AccountInput.Username);
                
                if (user == null)
                {
                    ErrorMessage = "Invalid login attempt.";
                    return Page();
                }

                if (!user.IsActive)
                {
                    ErrorMessage = "Your account is not active. Please contact the administrator.";
                    return Page();
                }

                var result = await _signInManager.PasswordSignInAsync(AccountInput.Username, AccountInput.Password, AccountInput.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    ErrorMessage = "Invalid login attempt.";
                    return Page();
                }
            }

            return Page();
        }
    }
}
