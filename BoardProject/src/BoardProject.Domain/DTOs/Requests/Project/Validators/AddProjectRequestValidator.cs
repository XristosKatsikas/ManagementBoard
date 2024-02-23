using FluentValidation;

namespace BoardProject.Domain.DTOs.Requests.Project.Validators
{
    public class AddProjectRequestValidator : AbstractValidator<AddProjectRequest>
    {
        public AddProjectRequestValidator()
        {
            RuleFor(p => p.Description).NotEmpty().Must(x => x.Length <= 100);
            RuleFor(p => p.Name).NotEmpty().Must(x => x.Length <= 100);
            RuleFor(p => p.Title).NotEmpty();
            RuleFor(p => p.StartDate).NotEmpty();
            RuleFor(p => p.FinishDate).NotEmpty();
            RuleFor(x => x).Must(x => x.StartDate >= DateTime.UtcNow);
            RuleFor(x => x).Must(x => x.FinishDate >= DateTime.UtcNow);
            RuleFor(x => x).Must(x => x.FinishDate > x.StartDate).WithMessage("FinishDate must be greater than StartDate");
            RuleFor(p => p.Progress).NotEmpty().Must(x => x >= 0);
        }
    }
}
