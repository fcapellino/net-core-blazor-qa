using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using BlazorAppQA.Infrastructure.ApplicationContext;
using BlazorAppQA.Infrastructure.BaseCommandHandler;
using BlazorAppQA.Infrastructure.Common;
using BlazorAppQA.Infrastructure.Domain;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorAppQA.Infrastructure.CommandHandlers.InsertNewQuestionHandler
{
    public class InsertNewQuestionCommandHandler : BaseCommandHandler<InsertNewQuestionCommand>
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IDataProtector _dataProtector;
        private readonly HttpContext _httpContext;

        public InsertNewQuestionCommandHandler(IServiceProvider provider)
            : base(provider)
        {
            _hostingEnvironment = provider.GetService<IWebHostEnvironment>();
            _dataProtector = provider.GetService<IDataProtectionProvider>().CreateProtector(Assembly.GetExecutingAssembly().FullName);
            _httpContext = provider.GetService<IHttpContextAccessor>().HttpContext;
        }

        protected override async Task<dynamic> ExecuteAsync(InsertNewQuestionCommand command)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using var scope = _serviceScopeFactory.CreateScope();
                using var applicationDbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();

                var tagsArray = command.Tags.Split("\u0020", StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim().ToLowerInvariant()).Distinct();
                var newQuestion = new Question()
                {
                    Title = command.Title.Trim(),
                    TagsArray = string.Join(";", tagsArray),
                    CategoryId = int.Parse(_dataProtector.Unprotect(command.ProtectedCategoryId)),
                    Description = command.Description.Trim(),
                    UserId = int.Parse(_httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)),
                    Date = DateTime.Now
                };

                applicationDbContext.Questions.Add(newQuestion);
                if (command.Files != null)
                {
                    await command.Files.AsEnumerable().ForEachAsync(async file =>
                    {
                        var fileName = Guid.NewGuid();
                        var absolutePath = Path.Combine(_hostingEnvironment.WebRootPath, $"qimages\\{fileName}.jpg");
                        var fileInfo = new FileInfo(absolutePath);
                        fileInfo.Directory.Create();
                        using var fileSteam = new FileStream(absolutePath, FileMode.Create);
                        await file.Data.CopyToAsync(fileSteam);

                        newQuestion.QuestionImages.Add(new QuestionImage()
                        {
                            FileName = fileName
                        });
                    });
                }

                await applicationDbContext.SaveChangesAsync();
                transactionScope.Complete();
            }

            return default;
        }

        public override void Dispose() { }
    }
}
