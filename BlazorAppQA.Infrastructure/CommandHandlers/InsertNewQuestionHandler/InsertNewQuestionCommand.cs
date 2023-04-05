using System.Linq;
using System.Text.RegularExpressions;
using BlazorInputFile;
using FluentValidation;

namespace BlazorAppQA.Infrastructure.CommandHandlers.InsertNewQuestionHandler
{
    public class InsertNewQuestionCommand
    {
        public string Title { get; set; }
        public string Tags { get; set; }
        public string ProtectedCategoryId { get; set; }
        public string Description { get; set; }
        public IFileListEntry[] Files { get; set; }
    }

    public class InsertNewQuestionCommandValidator
        : AbstractValidator<InsertNewQuestionCommand>
    {
        public InsertNewQuestionCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                    .WithMessage("Title must not be empty.")
                .MaximumLength(200);

            RuleFor(x => x.Tags)
                .NotEmpty()
                    .WithMessage("You must enter a list of tags.")
                .Must(x => Regex.IsMatch(x, @"[\w\s]+"));

            RuleFor(x => x.ProtectedCategoryId)
                .NotEmpty()
                    .WithMessage("You must select a category.");

            RuleFor(x => x.Description)
                .NotEmpty()
                    .WithMessage("You must provide a description.")
                .MaximumLength(1600);

            RuleFor(x => x.Files)
                .Must(x =>
                {
                    return x == null || (x.Length < 5 && x.All(file => file != null));
                });
        }
    }
}
