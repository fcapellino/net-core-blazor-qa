using System;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using BlazorAppQA.Infrastructure.ApplicationContext;
using BlazorAppQA.Infrastructure.BaseCommandHandler;
using BlazorAppQA.Infrastructure.Common;
using BlazorAppQA.Infrastructure.Domain;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorAppQA.Infrastructure.CommandHandlers.VoteForAnswerHandler
{
    public class VoteForAnswerCommandHandler : BaseCommandHandler<VoteForAnswerCommand>
    {
        private readonly IDataProtector _dataProtector;
        private readonly HttpContext _httpContext;

        public VoteForAnswerCommandHandler(IServiceProvider provider)
            : base(provider)
        {
            _dataProtector = provider.GetService<IDataProtectionProvider>().CreateProtector(Assembly.GetExecutingAssembly().FullName);
            _httpContext = provider.GetService<IHttpContextAccessor>().HttpContext;
        }

        protected override async Task<dynamic> ExecuteAsync(VoteForAnswerCommand command)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using var scope = _serviceScopeFactory.CreateScope();
                using var applicationDbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
                var userId = int.Parse(_httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                var answerId = int.Parse(_dataProtector.Unprotect(command.ProtectedAnswerId));

                var submittedVote = await applicationDbContext.Votes
                    .FirstOrDefaultAsync(v => v.UserId == userId && v.AnswerId == answerId && v.Upvote == command.UpVote);
                if (submittedVote != null)
                {
                    throw new CustomException($"You already {(submittedVote.Upvote ? "upvoted" : "downvoted")} this answer.");
                }

                var newVote = new Vote()
                {
                    UserId = userId,
                    AnswerId = answerId,
                    Upvote = command.UpVote,
                    Date = DateTime.Now
                };

                applicationDbContext.Votes.Add(newVote);
                await applicationDbContext.SaveChangesAsync();
                transactionScope.Complete();
            }

            return default;
        }

        public override void Dispose() { }
    }
}
