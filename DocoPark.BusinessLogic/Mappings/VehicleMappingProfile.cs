using AutoMapper;
using DocoPark.BusinessLogic.DTOs.Vehicle;
using DocoPark.Domain.Entities;

namespace DocoPark.BusinessLogic.Mappings;

public class VehicleMappingProfile : Profile
{
    public VehicleMappingProfile()
    {
        CreateMap<Vehicle, VehicleResponseDto>()
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.User != null ? src.User.Name : null));

        CreateMap<CreateVehicleDto, Vehicle>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.ParkingSessions, opt => opt.Ignore())
            .ForMember(dest => dest.Reservations, opt => opt.Ignore());

        CreateMap<UpdateVehicleDto, Vehicle>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.ParkingSessions, opt => opt.Ignore())
            .ForMember(dest => dest.Reservations, opt => opt.Ignore());
    }
}