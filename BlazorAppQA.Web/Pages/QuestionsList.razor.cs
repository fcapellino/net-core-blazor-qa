using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorAppQA.Infrastructure.CommandHandlers.GetCategoriesHandler;
using BlazorAppQA.Infrastructure.CommandHandlers.GetQuestionsListHandler;
using BlazorAppQA.Infrastructure.Common;
using BlazorAppQA.Web.Common;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorAppQA.Web.Pages
{
    public class QuestionsListComponent : CustomComponentBase
    {
        public string SearchQuery { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 3;
        public string ProtectedCategoryId { get; set; }
        public int TotalQuestionsCount { get; set; }
        public IEnumerable<dynamic> QuestionsList { get; set; }
        public IEnumerable<dynamic> CategoriesList { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await ExecuteAsync(async () =>
            {
                using var getQuestionsListCommandHandler = ServiceProvider.GetService<GetQuestionsListCommandHandler>();
                dynamic questionsListResult = await getQuestionsListCommandHandler.HandleAsync(new GetQuestionsListCommand()
                {
                    Page = 1,
                    PageSize = PageSize
                });

                TotalQuestionsCount = questionsListResult.TotalItemCount;
                QuestionsList = (questionsListResult.ItemsList as IEnumerable<object>).Select(x => x.ToExpando());

                using var getCategoriesCommandHandler = ServiceProvider.GetService<GetCategoriesListCommandHandler>();
                dynamic categoriesResult = await getCategoriesCommandHandler.HandleAsync(new GetCategoriesListCommand());

                CategoriesList = (categoriesResult.ItemsList as IEnumerable<object>).Select(x => x.ToExpando());
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
                using var getQuestionsListCommandHandler = ServiceProvider.GetService<GetQuestionsListCommandHandler>();
                dynamic result = await getQuestionsListCommandHandler.HandleAsync(new GetQuestionsListCommand()
                {
                    SearchQuery = SearchQuery,
                    Page = 1,
                    PageSize = PageSize,
                    ProtectedCategoryId = ProtectedCategoryId
                });

                QuestionsList = (result.ItemsList as IEnumerable<object>).Select(x => x.ToExpando());
                TotalQuestionsCount = result.TotalItemCount;
                CurrentPage = 1;
            });
        }

        public async Task OnPageChangeAsync(int page)
        {
            await ExecuteAsync(async () =>
            {
                using var getQuestionsListCommandHandler = ServiceProvider.GetService<GetQuestionsListCommandHandler>();
                dynamic result = await getQuestionsListCommandHandler.HandleAsync(new GetQuestionsListCommand()
                {
                    SearchQuery = SearchQuery,
                    Page = page,
                    PageSize = PageSize,
                    ProtectedCategoryId = ProtectedCategoryId
                });

                QuestionsList = (result.ItemsList as IEnumerable<object>).Select(x => x.ToExpando());
                TotalQuestionsCount = result.TotalItemCount;
                CurrentPage = page;
            });
        }

        public async Task OnQuestionClickAsync(string protectedId)
        {
            await ExecuteAsync(async () =>
            {
                await Task.FromResult(0).ContinueWith(t => NavigationManager.NavigateTo($"/questions/{protectedId}"));
            });
        }

        public async Task OnUserClickAsync(string protectedId)
        {
            await ExecuteAsync(async () =>
            {
                await Task.FromResult(0).ContinueWith(t => NavigationManager.NavigateTo($"/users/{protectedId}"));
            });
        }
    }
}
