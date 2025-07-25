using FluentValidation;
using BoardJob.Domain.Queries;

namespace BoardJob.Domain.Handlers.Validators
{
    public class GetJobQueryValidator : AbstractValidator<GetJobQuery>
    {
        public GetJobQueryValidator()
        {
            RuleFor(p => p.Id).NotEmpty();
        }
    }
}
