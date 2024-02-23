using FluentValidation;

namespace BoardProject.Domain.DTOs.Requests.Project.Validators
{
    public class GetProjectRequestValidator : AbstractValidator<GetProjectRequest>
    {
        public GetProjectRequestValidator() 
        {
            RuleFor(p => p.Id).NotEmpty();
        }
    }
}
