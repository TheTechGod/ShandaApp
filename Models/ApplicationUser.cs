using Microsoft.AspNetCore.Identity;
using ShandaApp.Models.Enums;

namespace ShandaApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsPersonalMode { get; set; }
        public string PreferredVoice { get; set; }
        public bool UseAdminTone { get; set; }
        public AIMood AIMood { get; set; }

    }
}
