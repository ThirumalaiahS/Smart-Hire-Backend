using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Velora.Core.DTOs.Job
{
    public record CreateJobApplicationDto(        
        string CompanyName,
        string JobTitle,
        string? JobDescription,
        string? JobUrl,
        string? Notes,
        DateTime AppliedDate,
        DateTime? InterviewDate,
        DateTime? FollowUpDate
    );

    public class CreateJobApplicationDtoValidator : AbstractValidator<CreateJobApplicationDto>
    {
        public CreateJobApplicationDtoValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Company name is required.")
                .MaximumLength(100).WithMessage("Company name must not exceed 100 characters.");
            RuleFor(x => x.JobTitle)
                .NotEmpty().WithMessage("Job title is required.")
                .MaximumLength(100).WithMessage("Job title must not exceed 100 characters.");
            RuleFor(x => x.JobDescription)
                .MaximumLength(1000).WithMessage("Job description must not exceed 1000 characters.");
            RuleFor(x => x.JobUrl)
                .MaximumLength(200).WithMessage("Job URL must not exceed 200 characters.")
                .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute)).When(x => !string.IsNullOrEmpty(x.JobUrl)).WithMessage("Invalid URL format.");
            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes must not exceed 500 characters.");
            RuleFor(x => x.AppliedDate)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Applied date cannot be in the future.");
            RuleFor(x => x.InterviewDate)
                .GreaterThanOrEqualTo(x => x.AppliedDate).When(x => x.InterviewDate.HasValue).WithMessage("Interview date cannot be before applied date.");
            RuleFor(x => x.FollowUpDate)
                .GreaterThanOrEqualTo(x => x.AppliedDate).When(x => x.FollowUpDate.HasValue).WithMessage("Follow-up date cannot be before applied date.");
        }
    }
}

