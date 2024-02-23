using FluentValidation;
using BoardJob.Domain.Commands;

namespace BoardJob.Domain.Handlers.Validators
{
    public class GetJobCommandValidator : AbstractValidator<GetJobCommand>
    {
        public GetJobCommandValidator()
        {
            RuleFor(p => p.Id).NotEmpty();
        }
    }
}
