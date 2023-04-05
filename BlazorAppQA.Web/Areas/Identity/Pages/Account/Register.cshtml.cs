using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using BlazorAppQA.Infrastructure.CommandHandlers.RegisterNewUserHandler;
using BlazorAppQA.Infrastructure.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorAppQA.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        protected RegisterNewUserCommandHandler RegisterNewUserCommandHandler { get; set; }

        public class InputModel
        {
            [Required]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            public string Password { get; set; }

            [Required]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmedPassword { get; set; }

            [Required]
            public IFormFile Image { get; set; }

            [Required]
            [Url, StringLength(250, MinimumLength = 30)]
            public string LinkedinUrl { get; set; }

            [Required]
            [StringLength(800, MinimumLength = 50)]
            public string Biography { get; set; }
        }

        public RegisterModel(IServiceProvider serviceProvider)
        {
            RegisterNewUserCommandHandler = serviceProvider.GetService<RegisterNewUserCommandHandler>();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var registerNewUserCommand = new RegisterNewUserCommand()
                    {
                        UserName = Input.UserName,
                        Email = Input.Email,
                        Password = Input.Password,
                        Base64AvatarImage = await ConvertImageToBase64StringAsync(Input.Image),
                        LinkedinUrl = Input.LinkedinUrl,
                        Biography = Input.Biography
                    };
                    await RegisterNewUserCommandHandler.HandleAsync(registerNewUserCommand);
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
                RegisterNewUserCommandHandler?.Dispose();
            }

            return Page();
        }

        protected async Task<string> ConvertImageToBase64StringAsync(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            using var img = Image.FromStream(memoryStream);
            var thumbnail = img.GetThumbnailImage(200, 200, () => false, IntPtr.Zero);

            using var thumbnailMemoryStream = new MemoryStream();
            thumbnail.Save(thumbnailMemoryStream, ImageFormat.Jpeg);
            return Convert.ToBase64String(thumbnailMemoryStream.GetBuffer());
        }
    }
}
