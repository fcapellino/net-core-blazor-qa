using System;
using System.Threading.Tasks;
using BlazorAppQA.Infrastructure.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace BlazorAppQA.Web.Common
{
    public class CustomComponentBase : ComponentBase
    {
        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        protected IServiceProvider ServiceProvider { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        protected async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return await AuthenticationStateProvider.GetAuthenticationStateAsync();
        }

        protected async Task ExecuteAsync(Func<Task> function)
        {
            try
            {
                await function();
            }
            catch (Exception ex)
            {
                var exception = ex.GetBaseException();
                var errorMessage = (exception is CustomException)
                        ? $"Error. {exception.Message}"
                        : $"Error. Could not complete this operation.";

                await ShowErrorMessageAsync(errorMessage);
            }
        }

        protected async Task ShowSuccessMessageAsync(string message)
        {
            await JSRuntime.InvokeAsync<string>("jsMethods.showSuccessMessage", message);
        }

        protected async Task ShowErrorMessageAsync(string message)
        {
            await JSRuntime.InvokeAsync<string>("jsMethods.showErrorMessage", message);
        }
    }
}
