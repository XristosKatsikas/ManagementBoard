using BoardJob.Domain.Configurations;
using BoardJob.Domain.DTOs.Requests.User;
using BoardJob.Domain.DTOs.Responses;
using BoardJob.Domain.Repositories.Abstractions;
using BoardJob.Domain.Services.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BoardJob.Domain.Services
{
    public class UserService : IUserService
    {

        private readonly AuthenticationSettings _authenticationSettings;
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository, IOptions<AuthenticationSettings> authenticationSettings)
        {
            _userRepository = userRepository;
            _authenticationSettings = authenticationSettings.Value;
        }

        /// <summary>
        /// Retrieves information related to a specific user
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<UserResponse> GetUserAsync(GetUserRequest request, CancellationToken cancellationToken)
        {
            var response = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            return new UserResponse { Name = response.Name, Email = response.Email! };
        }

        /// <summary>
        /// Returns the TokenResponse instance, which contains the resulting token that will be stored by the client
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<TokenResponse> SignInAsync(SignInRequest request, CancellationToken cancellationToken)
        {
            bool isAuthenticated = await _userRepository.AuthenticateAsync(request.Email, request.Password, cancellationToken);

            if (!isAuthenticated)
            {
                return null!;
            }
            return new TokenResponse { Token = GenerateSecurityToken(request) };
        }

        /// <summary>
        /// Return a new UserResponse instance which determines the information related to the signed user
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<UserResponse> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken)
        {
            var user = new Entities.User { Email = request.Email, UserName = request.Email, Name = request.Name };
            bool isCreated = await _userRepository.SignUpAsync(user, request.Password, cancellationToken);

            if (!isCreated)
            {
                return null!;
            }
            return new UserResponse { Name = request.Name, Email = request.Email };
        }

        public async Task<bool> DeleteUserAsync(DeleteUserRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user is not null)
            {
                var result = await _userRepository.DeleteAsync(user);
                if (result)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Defines a new instance of the JwtSecurityTokenHandler type, 
        /// which provides some utilities for generating and creating tokens
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string GenerateSecurityToken(SignInRequest request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_authenticationSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, request.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(_authenticationSettings.ExpirationDays),
                SigningCredentials = new SigningCredentials( new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
