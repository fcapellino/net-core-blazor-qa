using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorAppQA.Infrastructure.CommandHandlers.GetStatisticsHandler;
using BlazorAppQA.Infrastructure.Common;
using BlazorAppQA.Web.Common;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorAppQA.Web.Pages
{
    public class RightPanelComponent : CustomComponentBase
    {
        public int TotalQuestions { get; set; }
        public int TotalAnswers { get; set; }
        public IEnumerable<dynamic> TopUsersList { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await ExecuteAsync(async () =>
            {
                using var getStatisticsCommandHandler = ServiceProvider.GetService<GetStatisticsCommandHandler>();
                object result = await getStatisticsCommandHandler.HandleAsync(new GetStatisticsCommand());

                TotalQuestions = Convert.ToInt32(result.ToExpando().TotalQuestions);
                TotalAnswers = Convert.ToInt32(result.ToExpando().TotalAnswers);
                TopUsersList = (result.ToExpando().TopUsersList as IEnumerable<object>).Select(x => x.ToExpando());
            });
        }
    }
}
