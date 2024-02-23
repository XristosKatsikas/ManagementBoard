namespace BoardProject.Domain.DTOs.Requests.User
{
    public class DeleteUserRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
