using BoardProject.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace BoardProject.Core.ApiExtensions
{
    public static class MvcExtensions
    {
        public static ProjectObjectResult<T> ApiResponse<T>(this ControllerBase controller, IResult<T> result)
        {
            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            return new ProjectObjectResult<T>(result);
        }
    }
}
