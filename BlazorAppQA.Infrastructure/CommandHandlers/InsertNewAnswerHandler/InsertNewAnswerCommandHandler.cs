using System;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using BlazorAppQA.Infrastructure.ApplicationContext;
using BlazorAppQA.Infrastructure.BaseCommandHandler;
using BlazorAppQA.Infrastructure.Domain;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorAppQA.Infrastructure.CommandHandlers.InsertNewAnswerHandler
{
    public class InsertNewAnswerCommandHandler : BaseCommandHandler<InsertNewAnswerCommand>
    {
        private readonly IDataProtector _dataProtector;
        private readonly HttpContext _httpContext;

        public InsertNewAnswerCommandHandler(IServiceProvider provider)
            : base(provider)
        {
            _dataProtector = provider.GetService<IDataProtectionProvider>().CreateProtector(Assembly.GetExecutingAssembly().FullName);
            _httpContext = provider.GetService<IHttpContextAccessor>().HttpContext;
        }

        protected override async Task<dynamic> ExecuteAsync(InsertNewAnswerCommand command)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using var scope = _serviceScopeFactory.CreateScope();
                using var applicationDbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();

                var newAnswer = new Answer()
                {
                    Description = command.Description.Trim(),
                    UserId = int.Parse(_httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)),
                    QuestionId = int.Parse(_dataProtector.Unprotect(command.ProtectedQuestionId)),
                    Date = DateTime.Now
                };

                applicationDbContext.Answers.Add(newAnswer);
                await applicationDbContext.SaveChangesAsync();
                transactionScope.Complete();
            }

            return default;
        }

        public override void Dispose() { }
    }
}
