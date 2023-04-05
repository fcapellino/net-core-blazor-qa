using System;
using System.Threading.Tasks;
using BlazorAppQA.Infrastructure.BaseCommandHandler;
using BlazorAppQA.Infrastructure.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorAppQA.Infrastructure.CommandHandlers.LogOutHandler
{
    public class LogOutCommandHandler : BaseCommandHandler<LogOutCommand>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LogOutCommandHandler(IServiceProvider provider)
            : base(provider)
        {
            _signInManager = provider.GetService<SignInManager<ApplicationUser>>();
        }

        protected override async Task<dynamic> ExecuteAsync(LogOutCommand command)
        {
            await _signInManager.SignOutAsync();
            return default;
        }

        public override void Dispose() { }
    }
}
