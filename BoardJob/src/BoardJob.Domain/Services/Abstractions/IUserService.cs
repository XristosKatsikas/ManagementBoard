using BoardJob.Domain.DTOs.Requests.User;
using BoardJob.Domain.DTOs.Responses;

namespace BoardJob.Domain.Services.Abstractions
{
    public interface IUserService
    {
        Task<UserResponse> GetUserAsync(GetUserRequest request, CancellationToken cancellationToken = default);
        Task<UserResponse> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken = default);
        Task<TokenResponse> SignInAsync(SignInRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteUserAsync(DeleteUserRequest request, CancellationToken cancellationToken = default);
    }
}
