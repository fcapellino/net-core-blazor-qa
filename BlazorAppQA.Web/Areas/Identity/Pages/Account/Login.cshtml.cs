using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using BlazorAppQA.Infrastructure.CommandHandlers.LogInHandler;
using BlazorAppQA.Infrastructure.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorAppQA.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        protected LogInCommandHandler LogInCommandHandler { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public LoginModel(IServiceProvider serviceProvider)
        {
            LogInCommandHandler = serviceProvider.GetService<LogInCommandHandler>();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var registerNewUserCommand = new LogInCommand()
                    {
                        Email = Input.Email,
                        Password = Input.Password,
                    };
                    await LogInCommandHandler.HandleAsync(registerNewUserCommand);
                    return LocalRedirect("/");
                }
                else
                {
                    throw new CustomException("The received data model is invalid.");
                }
            }
            catch (Exception ex)
            {
                var exception = ex.GetBaseException();
                ViewData["Errormessage"] = (exception is CustomException)
                        ? $"Error. {exception.Message}"
                        : $"Error. Could not complete this operation.";
            }
            finally
            {
                LogInCommandHandler?.Dispose();
            }

            return Page();
        }
    }
}
