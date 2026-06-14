using FluentValidation;
using SmartHire.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHire.Core.DTOs.Job
{
    public record UpdateJobApplicationDto(
        string? CompanyName,
        string? JobTitle,
        string? JobDescription,
        string? JobUrl,
        string? Notes,
        DateTime? InterviewDate,
        DateTime? FollowUpDate,
        string? Status
    );

    public class UpdateJobApplicationDtoValidator : AbstractValidator<UpdateJobApplicationDto>
    {
        public UpdateJobApplicationDtoValidator()
        {
            RuleFor(x => x.CompanyName)
                .MaximumLength(100).WithMessage("Company name must not exceed 100 characters.");
            RuleFor(x => x.JobTitle)
                .MaximumLength(100).WithMessage("Job title must not exceed 100 characters.");
            RuleFor(x => x.JobDescription)
                .MaximumLength(1000).WithMessage("Job description must not exceed 1000 characters.");
            RuleFor(x => x.JobUrl)
                .MaximumLength(200).WithMessage("Job URL must not exceed 200 characters.")
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).When(x => !string.IsNullOrEmpty(x.JobUrl)).WithMessage("Invalid URL format.");
            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes must not exceed 500 characters.");
            RuleFor(x => x.InterviewDate)
                .GreaterThanOrEqualTo(DateTime.UtcNow.AddYears(-1)).When(x => x.InterviewDate.HasValue).WithMessage("Interview date cannot be more than a year in the past.");
            RuleFor(x => x.FollowUpDate)
                .GreaterThanOrEqualTo(DateTime.UtcNow.AddYears(-1)).When(x => x.FollowUpDate.HasValue).WithMessage("Follow-up date cannot be more than a year in the past.");
            RuleFor(x => x.Status)
                .Must(status => Enum.TryParse<ApplicationStatus>(status, true, out _)).When(x => !string.IsNullOrEmpty(x.Status)).WithMessage("Invalid application status.");
        }
    }
}
