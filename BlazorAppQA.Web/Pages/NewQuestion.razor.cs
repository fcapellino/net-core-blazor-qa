using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorAppQA.Infrastructure.CommandHandlers.GetCategoriesHandler;
using BlazorAppQA.Infrastructure.CommandHandlers.InsertNewQuestionHandler;
using BlazorAppQA.Infrastructure.Common;
using BlazorAppQA.Web.Common;
using BlazorInputFile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorAppQA.Web.Pages
{
    [Authorize]
    public class NewQuestionComponent : CustomComponentBase
    {
        public InsertNewQuestionCommand InsertNewQuestionCommand { get; set; } = new InsertNewQuestionCommand();
        public IEnumerable<dynamic> CategoriesList { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await ExecuteAsync(async () =>
            {
                using var getCategoriesCommandHandler = ServiceProvider.GetService<GetCategoriesListCommandHandler>();
                dynamic result = await getCategoriesCommandHandler.HandleAsync(new GetCategoriesListCommand());

                CategoriesList = (result.ItemsList as IEnumerable<object>).Select(x => x.ToExpando());
            });
        }

        public async Task HandleFileSelectedAsync(IFileListEntry[] files)
        {
            await Task.FromResult(0).ContinueWith(t => InsertNewQuestionCommand.Files = files);
        }

        public async Task OnSubmitAsync()
        {
            await ExecuteAsync(async () =>
            {
                using var insertNewQuestionCommandHandler = ServiceProvider.GetService<InsertNewQuestionCommandHandler>();
                await insertNewQuestionCommandHandler.HandleAsync(InsertNewQuestionCommand);
                await ShowSuccessMessageAsync("Your question has been successfully submitted.");
                NavigationManager.NavigateTo(@"/");
            });
        }

        public async Task OnCancelAsync()
        {
            await ExecuteAsync(async () =>
            {
                await Task.FromResult(0).ContinueWith(t => NavigationManager.NavigateTo(@"/"));
            });
        }
    }
}
