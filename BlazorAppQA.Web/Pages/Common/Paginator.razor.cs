using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace BlazorAppQA.Web.Pages.Common
{
    public class PaginatorComponent : ComponentBase
    {
        [Parameter]
        public int TotalItemsCount { get; set; }

        [Parameter]
        public int CurrentPage { get; set; }

        [Parameter]
        public int PageSize { get; set; }

        [Parameter]
        public EventCallback<int> PageChanged { get; set; }

        public int TotalPages { get; set; }

        protected override void OnParametersSet()
        {
            if (PageSize > 0)
            {
                TotalPages = (int)Math.Ceiling(TotalItemsCount / (double)PageSize);
            }

            base.OnParametersSet();
        }

        protected async Task PreviousPageButtonClickedAsync()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
            }

            await PageChanged.InvokeAsync(CurrentPage);
        }

        protected async Task PageButtonClickedAsync(int page)
        {
            CurrentPage = page;
            await PageChanged.InvokeAsync(page);
        }

        protected async Task NextPageButtonClickedAsync()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
            }

            await PageChanged.InvokeAsync(CurrentPage);
        }
    }
}
