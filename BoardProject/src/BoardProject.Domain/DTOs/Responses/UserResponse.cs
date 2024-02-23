namespace BoardProject.Domain.DTOs.Responses
{
    public record UserResponse
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}