using Microsoft.AspNetCore.Http;
using Polly;
using System.Net.Http;

namespace BoardProject.Core.WebApi.Middleware
{
    public class RetryPolicyMiddleware
    {
        private readonly RequestDelegate _next;

        public RetryPolicyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            await retryPolicy.ExecuteAsync(async () =>
            {
                await _next(context);
            });
        }
    }
}
