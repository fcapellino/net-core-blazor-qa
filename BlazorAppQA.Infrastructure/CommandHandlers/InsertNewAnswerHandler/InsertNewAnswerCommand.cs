using FluentValidation;

namespace BlazorAppQA.Infrastructure.CommandHandlers.InsertNewAnswerHandler
{
    public class InsertNewAnswerCommand
    {
        public string ProtectedQuestionId { get; set; }
        public string Description { get; set; }
    }

    public class InsertNewAnswerCommandValidator
        : AbstractValidator<InsertNewAnswerCommand>
    {
        public InsertNewAnswerCommandValidator()
        {
            RuleFor(x => x.ProtectedQuestionId)
                .NotEmpty();

            RuleFor(x => x.Description)
                .NotEmpty()
                    .WithMessage("You must provide a description.")
                .MaximumLength(1600);
        }
    }
}
