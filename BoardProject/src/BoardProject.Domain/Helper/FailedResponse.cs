using BoardProject.Core.Abstractions;
using BoardProject.Core;
using BoardProject.Domain.DTOs.Responses;

namespace BoardProject.Domain.Helper
{
    public static class FailedResponse
    {
        public static async Task<IResult<ProjectResponse>> GetUnprocessableValidationResultAsync(FluentValidation.Results.ValidationResult validationResult)
        {
            return Result<ProjectResponse>.CreateFailed(
                    ResultCode.UnprocessableEntity,
                    validationResult.Errors.Select(val => val.ErrorMessage).ToList());
        }

        public static async Task<IResult<ProjectResponse>> GetUnprocessableEventResultAsync()
        {
            return Result<ProjectResponse>.CreateFailed(
                    ResultCode.UnprocessableEntity,
                    "Event cannot be sent");
        }

        public static async Task<IResult<ProjectResponse>> GetBadRequestResultAsync(Guid id)
        {
            return Result<ProjectResponse>.CreateFailed(
                ResultCode.BadRequest,
                string.Format("Bad create request for project entity with Id {0}", id));
        }
    }
}
