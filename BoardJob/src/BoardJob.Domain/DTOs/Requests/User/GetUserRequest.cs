namespace BoardJob.Domain.DTOs.Requests.User
{
    public record GetUserRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}
