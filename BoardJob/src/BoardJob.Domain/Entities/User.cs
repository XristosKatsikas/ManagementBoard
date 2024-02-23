using Microsoft.AspNetCore.Identity;

namespace BoardJob.Domain.Entities
{
    /// <summary>
    /// The IdentityUser class identifies a storable user entity.
    /// The entity can be used to store data through the Microsoft.AspNetCore.Identity package
    /// </summary>
    public class User : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
    }
}
