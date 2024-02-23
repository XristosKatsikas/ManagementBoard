using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace BoardProject.Infrastructure.Middleware
{
    /// <summary>
    /// Measure the response time of ASP.NET Core actions -> aadd it to the middleware pipeline
    /// </summary>
    public class ResponseTimeMiddlewareAsync
    {
        private const string RESPONSE_TIME_IN_MS = "Response-Time-in-ms";

        private readonly RequestDelegate _next;

        public ResponseTimeMiddlewareAsync(RequestDelegate next)
        {
            _next = next;
        }

        public Task InvokeAsync(HttpContext context)
        {
            var watch = new Stopwatch();

            watch.Start();

            context.Response.OnStarting(() =>
            {
                watch.Stop();

                var responseTimeForCompleteRequest = watch.ElapsedMilliseconds;
                context.Response.Headers[RESPONSE_TIME_IN_MS] = responseTimeForCompleteRequest.ToString();

                return Task.CompletedTask;
            });

            return _next(context);
        }
    }
}
