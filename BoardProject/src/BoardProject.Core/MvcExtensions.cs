using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace BoardProject.Core
{
    public static class MvcExtensions
    {
        public static ObjectResult ApiResponse<T>(this ControllerBase controller, IResult<T> result) where T : class
        {
            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            return new ObjectResult(result);
        }
    }
}
