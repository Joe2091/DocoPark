using AutoMapper;
using DocoPark.BusinessLogic.DTOs.User;
using DocoPark.BusinessLogic.Interfaces;
using DocoPark.BusinessLogic.Interfaces.Services;
using DocoPark.Domain.Entities;
using DocoPark.Domain.Enums;

namespace DocoPark.BusinessLogic.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await unitOfWork.Users.GetAllAsync();
        var results = new List<UserResponseDto>();

        foreach (var user in users)
        {
            var vehicles = await unitOfWork.Vehicles.FindAsync(v => v.UserId == user.Id);
            
            results.Add(new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                SubscriptionType = user.SubscriptionType,
                IsPremium = user.IsPremium,
                CreatedDate = user.CreatedDate,
                VehicleCount = vehicles.Count()
            });
        }

        return results;
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(int id)
    {
        var user = await unitOfWork.Users.GetByIdAsync(id);
        if (user is null) return null;

        var vehicles = await unitOfWork.Vehicles.FindAsync(v => v.UserId == user.Id);

        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            SubscriptionType = user.SubscriptionType,
            IsPremium = user.IsPremium,
            CreatedDate = user.CreatedDate,
            VehicleCount = vehicles.Count()
        };
    }

    public async Task<UserResponseDto> CreateUserAsync(CreateUserDto dto)
    {
        var user = mapper.Map<User>(dto);

        await unitOfWork.Users.AddAsync(user);
        await unitOfWork.SaveChangesAsync();

        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            SubscriptionType = user.SubscriptionType,
            IsPremium = user.IsPremium,
            CreatedDate = user.CreatedDate,
            VehicleCount = 0 
        };
    }

    public async Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserDto dto)
    {
        var user = await unitOfWork.Users.GetByIdAsync(id);
        if (user is null)
            return null;

        mapper.Map(dto, user);

        unitOfWork.Users.Update(user);
        await unitOfWork.SaveChangesAsync();

        var vehicles = await unitOfWork.Vehicles.FindAsync(v => v.UserId == user.Id);

        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            SubscriptionType = user.SubscriptionType,
            IsPremium = user.IsPremium,
            CreatedDate = user.CreatedDate,
            VehicleCount = vehicles.Count()
        };
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await unitOfWork.Users.GetByIdAsync(id);
        if (user is null)
            return false;

        unitOfWork.Users.Remove(user);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<UserResponseDto>> GetUsersBySubscriptionTypeAsync(string subscriptionType)
    {
        if (!Enum.TryParse<SubscriptionType>(subscriptionType, ignoreCase: true, out var type))
            return Enumerable.Empty<UserResponseDto>();

        var users = await unitOfWork.Users.FindAsync(u => u.SubscriptionType == type);
        var results = new List<UserResponseDto>();

        foreach (var user in users)
        {
            var vehicles = await unitOfWork.Vehicles.FindAsync(v => v.UserId == user.Id);
            
            results.Add(new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                SubscriptionType = user.SubscriptionType,
                IsPremium = user.IsPremium,
                CreatedDate = user.CreatedDate,
                VehicleCount = vehicles.Count()
            });
        }

        return results;
    }
}