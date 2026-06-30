using AutoMapper;
using DocoPark.BusinessLogic.DTOs.User;
using DocoPark.Domain.Entities;

namespace DocoPark.BusinessLogic.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserResponseDto>()
            .ForMember(dest => dest.VehicleCount, opt => opt.MapFrom(src => src.Vehicles != null ? src.Vehicles.Count : 0));

        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsPremium, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Vehicles, opt => opt.Ignore())
            .ForMember(dest => dest.Subscriptions, opt => opt.Ignore())
            .ForMember(dest => dest.Reservations, opt => opt.Ignore())
            .ForMember(dest => dest.ParkingSessions, opt => opt.Ignore())
            .ForMember(dest => dest.Transactions, opt => opt.Ignore());

        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.Vehicles, opt => opt.Ignore())
            .ForMember(dest => dest.Subscriptions, opt => opt.Ignore())
            .ForMember(dest => dest.Reservations, opt => opt.Ignore())
            .ForMember(dest => dest.ParkingSessions, opt => opt.Ignore())
            .ForMember(dest => dest.Transactions, opt => opt.Ignore());
    }
}