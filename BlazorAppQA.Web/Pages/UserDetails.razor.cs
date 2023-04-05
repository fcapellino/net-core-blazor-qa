using System.Threading.Tasks;
using BlazorAppQA.Infrastructure.CommandHandlers.GetUserHandler;
using BlazorAppQA.Infrastructure.Common;
using BlazorAppQA.Web.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorAppQA.Web.Pages
{
    public class UserDetailsComponent : CustomComponentBase
    {
        [Parameter]
        public string ProtectedId { get; set; }
        public dynamic RegisteredUser { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await ExecuteAsync(async () =>
            {
                if (!string.IsNullOrEmpty(ProtectedId))
                {
                    using var getUserCommandHandler = ServiceProvider.GetService<GetUserCommandHandler>();
                    object result = await getUserCommandHandler.HandleAsync(new GetUserCommand()
                    {
                        ProtectedUserId = ProtectedId
                    });

                    RegisteredUser = result.ToExpando();
                }
            });
        }
    }
}
