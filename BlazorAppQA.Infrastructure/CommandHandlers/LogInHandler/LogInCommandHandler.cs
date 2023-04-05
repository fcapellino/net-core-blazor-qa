using System;
using System.Threading.Tasks;
using BlazorAppQA.Infrastructure.BaseCommandHandler;
using BlazorAppQA.Infrastructure.Common;
using BlazorAppQA.Infrastructure.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorAppQA.Infrastructure.CommandHandlers.LogInHandler
{
    public class LogInCommandHandler : BaseCommandHandler<LogInCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LogInCommandHandler(IServiceProvider provider)
            : base(provider)
        {
            _userManager = provider.GetService<UserManager<ApplicationUser>>();
            _signInManager = provider.GetService<SignInManager<ApplicationUser>>();
        }

        protected override async Task<dynamic> ExecuteAsync(LogInCommand command)
        {
            var applicationUser = await _userManager.FindByEmailAsync(command.Email.Trim().Normalize().ToLowerInvariant());
            var result = await _signInManager.PasswordSignInAsync(applicationUser != null ? applicationUser.UserName : string.Empty, command.Password, isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                throw new CustomException("Invalid user account or password.");
            }

            return default;
        }

        public override void Dispose() => _userManager?.Dispose();
    }
}
