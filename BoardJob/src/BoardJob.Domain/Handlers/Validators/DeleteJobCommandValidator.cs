using FluentValidation;
using BoardJob.Domain.Commands;

namespace BoardJob.Domain.Handlers.Validators
{
    public class DeleteJobCommandValidator : AbstractValidator<DeleteJobCommand>
    {
        public DeleteJobCommandValidator()
        {
            RuleFor(p => p.Id).NotEmpty();
        }
    }
}
