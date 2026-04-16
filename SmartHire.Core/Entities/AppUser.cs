using Microsoft.AspNetCore.Identity;

namespace SmartHire.Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? ResumeText { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
    }
}
