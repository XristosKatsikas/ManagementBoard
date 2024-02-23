using FluentValidation;
using FluentValidation.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardProject.Domain.DTOs.Requests.Project.Validators
{
    public class DeleteProjectRequestValidator : AbstractValidator<DeleteProjectRequest>
    {
        public DeleteProjectRequestValidator()
        {
            RuleFor(p => p.Id).NotEmpty();
        }
    }
}
