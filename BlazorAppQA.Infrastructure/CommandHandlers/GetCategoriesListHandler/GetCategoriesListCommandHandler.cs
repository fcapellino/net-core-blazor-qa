using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BlazorAppQA.Infrastructure.ApplicationContext;
using BlazorAppQA.Infrastructure.BaseCommandHandler;
using BlazorAppQA.Infrastructure.Common;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorAppQA.Infrastructure.CommandHandlers.GetCategoriesHandler
{
    public class GetCategoriesListCommandHandler : BaseCommandHandler<GetCategoriesListCommand>
    {
        private readonly IDataProtector _dataProtector;

        public GetCategoriesListCommandHandler(IServiceProvider provider)
            : base(provider)
        {
            _dataProtector = provider.GetService<IDataProtectionProvider>().CreateProtector(Assembly.GetExecutingAssembly().FullName);
        }

        protected override async Task<dynamic> ExecuteAsync(GetCategoriesListCommand command)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            using var applicationDbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();

            var queryResult = await applicationDbContext.Categories
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    ProtectedId = _dataProtector.Protect(c.Id.ToString()),
                    c.Name
                })
                .ToListAsync();

            return new ListResult
            {
                TotalItemCount = queryResult.Count,
                ItemsList = queryResult
            };
        }

        public override void Dispose() { }
    }
}
