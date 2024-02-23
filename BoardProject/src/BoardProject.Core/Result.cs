using BoardProject.Core.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BoardProject.Core
{
    public class Result<T> : IResult<T>
    {
        public T Data { get; set; }

        public ILogger Logger { get; set; }

        [JsonPropertyName("status")]
        public int StatusCode { get; set; }

        public List<string> Errors { get; set; } = new List<string>();

        public bool Success => ResultCode.IsSuccess(StatusCode);

        public static IResult<T> CreateSuccessful(T data)
        {
            return CreateSuccessful(data, ResultCode.OK);
        }

        public static IResult<T> CreateSuccessful(T data, int statusCode)
        {
            if (typeof(T) != typeof(object) && data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return new Result<T>() { Data = data, StatusCode = statusCode };
        }

        public static IResult<T> CreateFailed<Y>(IResult<Y> result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return CreateFailed(result.Logger, result.StatusCode, result.Errors);
        }

        public static IResult<T> CreateFailed(int errorCode, string errorText)
        {
            return CreateFailed(errorCode, errorText);
        }

        public static IResult<T> CreateFailed(int errorCode, List<string> errors)
        {
            return CreateFailed(errorCode, errors);
        }

        public static IResult<T> CreateFailed(ILogger logger, int errorCode, string errorText)
        {
            return CreateFailed(logger, errorCode, new List<string> { errorText });
        }

        public static IResult<T> CreateFailed(ILogger logger, int errorCode, List<string> errors)
        {
            if (ResultCode.IsSuccess(errorCode))
            {
                throw new ArgumentOutOfRangeException(nameof(errorCode));
            }

            return new Result<T> { Logger = logger, StatusCode = errorCode, Errors = errors };
        }
    }
}
