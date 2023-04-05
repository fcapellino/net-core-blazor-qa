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

namespace BlazorAppQA.Infrastructure.CommandHandlers.GetQuestionHandler
{
    public class GetQuestionCommandHandler : BaseCommandHandler<GetQuestionCommand>
    {
        private readonly IDataProtector _dataProtector;

        public GetQuestionCommandHandler(IServiceProvider provider)
            : base(provider)
        {
            _dataProtector = provider.GetService<IDataProtectionProvider>().CreateProtector(Assembly.GetExecutingAssembly().FullName);
        }

        protected override async Task<dynamic> ExecuteAsync(GetQuestionCommand command)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            using var applicationDbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
            var questionId = int.Parse(_dataProtector.Unprotect(command.ProtectedQuestionId));

            var submittedQuestion = await applicationDbContext.Questions
                .Include(q => q.User)
                .Include(q => q.Category)
                .Include(q => q.QuestionImages)
                .Include(q => q.QuestionAnswers)
                    .ThenInclude(a => a.User)
                .Include(q => q.QuestionAnswers)
                    .ThenInclude(a => a.AnswerVotes)
                .Where(q => q.Id == questionId)
                .Select(q => new
                {
                    ProtectedId = _dataProtector.Protect(q.Id.ToString()),
                    q.Title,
                    q.Description,
                    q.Date,
                    User = new
                    {
                        ProtectedId = _dataProtector.Protect(q.User.Id.ToString()),
                        q.User.UserName,
                        q.User.Base64AvatarImage
                    },
                    Tags = q.TagsArray.Split(";", StringSplitOptions.None),
                    CategoryName = q.Category.Name,
                    Images = q.QuestionImages.Select(i => i.FileName.ToString()),
                    Answers = q.QuestionAnswers.Select(a => new
                    {
                        ProtectedId = _dataProtector.Protect(a.Id.ToString()),
                        User = new
                        {
                            ProtectedId = _dataProtector.Protect(a.User.Id.ToString()),
                            a.User.UserName,
                            a.User.Base64AvatarImage
                        },
                        a.Description,
                        a.BestAnswer,
                        a.Date,
                        Votes = (a.AnswerVotes.Count(v => v.Upvote) - a.AnswerVotes.Count(v => !v.Upvote))
                    }),
                })
                .FirstOrDefaultAsync();

            if (submittedQuestion == null)
            {
                throw new CustomException("Invalid question specified.");
            }

            return submittedQuestion;
        }

        public override void Dispose() { }
    }
}
