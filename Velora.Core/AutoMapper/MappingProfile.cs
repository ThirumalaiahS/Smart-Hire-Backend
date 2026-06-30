using AutoMapper;
using Velora.Core.DTOs.Job;
using Velora.Core.Entities;

namespace Velora.Core.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Job Application Mappings
            CreateMap<JobApplication, JobApplicationDTO>().ReverseMap();
            CreateMap<JobApplication, CreateJobApplicationDto>().ReverseMap();
            CreateMap<UpdateJobApplicationDto, JobApplication>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}

