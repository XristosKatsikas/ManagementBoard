using BoardProject.Domain.DTOs.Requests.User;
using BoardProject.Domain.DTOs.Responses;

namespace BoardProject.Domain.Services.Abstractions
{
    public interface IUserService
    {
        Task<UserResponse> GetUserAsync(GetUserRequest request, CancellationToken cancellationToken = default);
        Task<UserResponse> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken = default);
        Task<TokenResponse> SignInAsync(SignInRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteUserAsync(DeleteUserRequest request, CancellationToken cancellationToken = default);
    }
}
