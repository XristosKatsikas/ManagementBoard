using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardProject.Core
{
    public static class ResultCode
    {
        public const int Undefined = 0;

        public const int OK = 200;

        public const int Created = 201;

        public const int Accepted = 202;

        public const int NoContent = 204;

        public const int BadRequest = 400;

        public const int Unauthorized = 401;

        public const int Forbidden = 403;

        public const int NotFound = 404;

        public const int MethodNotAllowed = 405;

        public const int RequestTimeout = 408;

        public const int Conflict = 409;

        public const int PreconditionFailed = 412;

        public const int PayloadTooLarge = 413;

        public const int UnsupportedMediaType = 415;

        public const int UnprocessableEntity = 422;

        public const int Locked = 423;

        public const int MethodFailure = 424;

        public const int TooEarly = 425;

        public const int TooManyRequests = 429;

        public const int InternalServerError = 500;

        public const int BadGateway = 502;

        public const int ServiceUnavailable = 503;

        public static bool IsSuccess(int code)
        {
            return code == OK || (code > 200 && code <= 299);
        }
    }
}
