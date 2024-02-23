using BoardJob.Domain.Configurations;
using BoardJob.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BoardJob.Infrastructure.Extensions
{
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Calls to AddAuthentication and AddJwtBearer.
        /// They add the middlewares and the services used by the authentication process.
        /// 
        /// AddAuthentication specifies DefaultAuthenticationScheme and 
        /// DefaultChallengeScheme by applying the JWT bearer authentication scheme.
        /// 
        /// AddJwtBearer method defines the options related to tokenauthentication, such as the TokenValidationParameters field, 
        /// which includes the SigningKey used to validate the token parameter.
        /// 
        /// IssuerSigningKey must be the same as the key used to generate the token. Otherwise, the validation will fail. 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddTokenAuthentication(this IServiceCollection services,
            IConfiguration configuration)
        {
            var settings = configuration.GetSection("AuthenticationSettings");
            var settingsTyped = settings.Get<AuthenticationSettings>();

            services.Configure<AuthenticationSettings>(settings);
            var key = Encoding.ASCII.GetBytes(settingsTyped!.Secret);

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddAuthorization();

            return services;
        }
    }
}
