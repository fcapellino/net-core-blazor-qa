using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using BlazorAppQA.Infrastructure.ApplicationContext;
using BlazorAppQA.Infrastructure.BaseCommandHandler;
using BlazorAppQA.Infrastructure.Common;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorAppQA.Infrastructure.CommandHandlers.SelectBestAnswerHandler
{
    public class SelectBestAnswerCommandHandler : BaseCommandHandler<SelectBestAnswerCommand>
    {
        private readonly IDataProtector _dataProtector;
        private readonly HttpContext _httpContext;

        public SelectBestAnswerCommandHandler(IServiceProvider provider)
            : base(provider)
        {
            _dataProtector = provider.GetService<IDataProtectionProvider>().CreateProtector(Assembly.GetExecutingAssembly().FullName);
            _httpContext = provider.GetService<IHttpContextAccessor>().HttpContext;
        }

        protected override async Task<dynamic> ExecuteAsync(SelectBestAnswerCommand command)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using var scope = _serviceScopeFactory.CreateScope();
                using var applicationDbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
                var userId = int.Parse(_httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                var answerId = int.Parse(_dataProtector.Unprotect(command.ProtectedAnswerId));

                var selectedAnswer = await applicationDbContext.Answers
                    .Include(a => a.Question)
                    .FirstOrDefaultAsync(a => a.Id == answerId);

                if (selectedAnswer == null)
                {
                    throw new CustomException("Invalid answer specified.");
                }
                else if (selectedAnswer.Question.UserId != userId)
                {
                    throw new CustomException("You are not the owner of this question.");
                }
                else if (selectedAnswer.BestAnswer)
                {
                    throw new CustomException("You already accepted this as the best answer.");
                }

                await applicationDbContext.Answers
                     .Where(a => a.Id != selectedAnswer.Id && a.QuestionId == selectedAnswer.QuestionId)
                     .ForEachAsync(a => { a.BestAnswer = false; });

                selectedAnswer.BestAnswer = true;
                await applicationDbContext.SaveChangesAsync();
                transactionScope.Complete();
            }

            return default;
        }

        public override void Dispose() { }
    }
}
