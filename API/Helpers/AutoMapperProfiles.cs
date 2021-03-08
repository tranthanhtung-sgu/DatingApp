using System.Linq;
using API.DTOs;
using API.Extentions;
using API.Models;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => 
                    src.Photos.FirstOrDefault(x =>x.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(sourceMember => 
                    sourceMember.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDto>();
        }
    }
}