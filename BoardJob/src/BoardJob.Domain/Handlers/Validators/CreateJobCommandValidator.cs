using FluentValidation;
using BoardJob.Domain.Commands;

namespace BoardJob.Domain.Handlers.Validators
{
    public class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
    {
        public CreateJobCommandValidator()
        {
            RuleFor(p => p.Description).NotEmpty().Must(x => x.Length <= 100);
            RuleFor(p => p.Title).NotEmpty();
            RuleFor(p => p.StartDate).NotEmpty();
            RuleFor(p => p.FinishDate).NotEmpty();
            RuleFor(x => x).Must(x => x.StartDate >= DateTime.UtcNow);
            RuleFor(x => x).Must(x => x.FinishDate >= DateTime.UtcNow);
            RuleFor(x => x).Must(x => x.FinishDate > x.StartDate).WithMessage("FinishDate must be greater than StartDate");
            RuleFor(p => p.Progress).NotEmpty().Must(x => x >= 0);
            RuleFor(p => p.Status).NotEmpty();
            RuleFor(p => p)
                .Must(x =>
                x.Status == Enums.Status.TODO ||
                x.Status == Enums.Status.INPROGRESS ||
                x.Status == Enums.Status.INREVIEW ||
                x.Status == Enums.Status.DONE);
            RuleFor(x => x.ProjectId)
               .NotEmpty()
               .WithMessage("ProjectId must exist.");
        }
    }
}
