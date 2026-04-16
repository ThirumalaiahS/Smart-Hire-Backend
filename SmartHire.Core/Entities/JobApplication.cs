using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHire.Core.Entities
{
    public enum ApplicationStatus
    {
        Applied, 
        PhoneScreen, 
        Interview, 
        TechnicalTest,
        Offer, 
        Rejected, 
        Withdrawn, 
        Ghosted
    }
    public class JobApplication
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;
        public string CompanyName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string? JobDescription { get; set; }
        public string? JobUrl { get; set; }
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;
        public int? AiMatchScore { get; set; }
        public string? AiFeedback { get; set; }
        public string? Notes { get; set; }
        public DateTime AppliedDate { get; set; } = DateTime.UtcNow;
        public DateTime? InterviewDate { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}
