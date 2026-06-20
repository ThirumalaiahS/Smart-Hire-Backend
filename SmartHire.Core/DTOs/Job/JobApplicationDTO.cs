using FluentValidation;
using SmartHire.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHire.Core.DTOs.Job
{
    public record JobApplicationDTO(
        int Id,
        string CompanyName,
        string JobTitle,
        string Status,
        string? JobDescription,
        int? AiMatchScore,
        string? AiFeedback,
        string? Notes,
        DateTime AppliedDate,
        DateTime? InterviewDate,
        DateTime? FollowUpDate,
        string? JobUrl
    );

    public class JobApplicationDTOValidator : AbstractValidator<JobApplicationDTO>
    {
        public JobApplicationDTOValidator()
        {
            RuleFor(x => x.CompanyName)
                .MaximumLength(100).WithMessage("Company name must not exceed 100 characters.");
            RuleFor(x => x.JobTitle)
                .MaximumLength(100).WithMessage("Job title must not exceed 100 characters.");
            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes must not exceed 500 characters.");
            RuleFor(x => x.JobUrl)
                .MaximumLength(200).WithMessage("Job URL must not exceed 200 characters.")
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).When(x => !string.IsNullOrEmpty(x.JobUrl)).WithMessage("Invalid URL format.");
            RuleFor(x => x.Status)
                .Must(status => Enum.TryParse<ApplicationStatus>(status, true, out _)).WithMessage("Invalid application status.");
        }
    }
}
