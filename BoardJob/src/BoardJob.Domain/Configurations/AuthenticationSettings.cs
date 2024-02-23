namespace BoardJob.Domain.Configurations
{
    public class AuthenticationSettings
    {
        // Describes a phrase that is used to encrypt the token's information
        public string Secret { get; set; } = string.Empty;
        // Provides the number of days before the omitted tokens expire
        public int ExpirationDays { get; set; }
    }
}
