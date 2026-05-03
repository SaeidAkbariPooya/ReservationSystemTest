using AutoMapper;
using ReservationSystem.Core.DTOs;
using ReservationSystem.Core.Entities;

namespace ReservationSystem.Core.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Reservation, ReservationReportDto>()
                .ForMember(dest => dest.ResourceName, opt => opt.MapFrom(src => src.Resource.Name))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name));
            CreateMap<Resource, ResourceAvailabilityDto>();
        }
    }
}
