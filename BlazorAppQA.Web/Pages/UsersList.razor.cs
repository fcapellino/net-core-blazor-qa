using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorAppQA.Infrastructure.CommandHandlers.GetUsersListHandler;
using BlazorAppQA.Infrastructure.Common;
using BlazorAppQA.Web.Common;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorAppQA.Web.Pages
{
    public class UsersListComponent : CustomComponentBase
    {
        public string SearchQuery { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 3;
        public int TotalUsersCount { get; set; }
        public IEnumerable<dynamic> UsersList { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await ExecuteAsync(async () =>
            {
                using var getUsersListCommandHandler = ServiceProvider.GetService<GetUsersListCommandHandler>();
                dynamic result = await getUsersListCommandHandler.HandleAsync(new GetUsersListCommand()
                {
                    Page = 1,
                    PageSize = PageSize
                });

                TotalUsersCount = result.TotalItemCount;
                UsersList = (result.ItemsList as IEnumerable<object>).Select(x => x.ToExpando());
            });
        }

        public async Task OnKeyUpEventAsync(KeyboardEventArgs e)
        {
            if (e.Key.Equals("Enter"))
            {
                await OnSearchAsync();
            }
        }

        public async Task OnSearchAsync()
        {
            await ExecuteAsync(async () =>
            {
                using var getUsersListCommandHandler = ServiceProvider.GetService<GetUsersListCommandHandler>();
                dynamic result = await getUsersListCommandHandler.HandleAsync(new GetUsersListCommand()
                {
                    SearchQuery = SearchQuery,
                    Page = 1,
                    PageSize = PageSize
                });

                UsersList = (result.ItemsList as IEnumerable<object>).Select(x => x.ToExpando());
                TotalUsersCount = result.TotalItemCount;
                CurrentPage = 1;
            });
        }

        public async Task OnPageChangeAsync(int page)
        {
            await ExecuteAsync(async () =>
            {
                using var getUsersListCommandHandler = ServiceProvider.GetService<GetUsersListCommandHandler>();
                dynamic result = await getUsersListCommandHandler.HandleAsync(new GetUsersListCommand()
                {
                    SearchQuery = SearchQuery,
                    Page = page,
                    PageSize = PageSize
                });

                UsersList = (result.ItemsList as IEnumerable<object>).Select(x => x.ToExpando());
                TotalUsersCount = result.TotalItemCount;
                CurrentPage = page;
            });
        }

        public async Task OnViewDetailsAsync(string protectedId)
        {
            await ExecuteAsync(async () =>
            {
                await Task.FromResult(0).ContinueWith(t => NavigationManager.NavigateTo($"/users/{protectedId}"));
            });
        }
    }
}
