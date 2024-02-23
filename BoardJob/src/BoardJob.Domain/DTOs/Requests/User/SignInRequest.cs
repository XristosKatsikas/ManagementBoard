namespace BoardJob.Domain.DTOs.Requests.User
{
    public record SignInRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
