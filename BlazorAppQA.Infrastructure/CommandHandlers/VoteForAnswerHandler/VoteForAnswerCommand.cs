using FluentValidation;

namespace BlazorAppQA.Infrastructure.CommandHandlers.VoteForAnswerHandler
{
    public class VoteForAnswerCommand
    {
        public string ProtectedAnswerId { get; set; }
        public bool UpVote { get; set; }
    }

    public class VoteForAnswerCommandValidator
        : AbstractValidator<VoteForAnswerCommand>
    {
        public VoteForAnswerCommandValidator()
        {
            RuleFor(x => x.ProtectedAnswerId)
                .NotEmpty();
        }
    }
}
