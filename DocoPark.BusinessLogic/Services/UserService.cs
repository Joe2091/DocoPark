using AutoMapper;
using DocoPark.BusinessLogic.DTOs.User;
using DocoPark.BusinessLogic.Interfaces;
using DocoPark.BusinessLogic.Interfaces.Services;
using DocoPark.Domain.Entities;
using DocoPark.Domain.Enums;

namespace DocoPark.BusinessLogic.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        return _mapper.Map<IEnumerable<UserResponseDto>>(users);
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        return user is null ? null : _mapper.Map<UserResponseDto>(user);
    }

    public async Task<UserResponseDto> CreateUserAsync(CreateUserDto dto)
    {
        var user = _mapper.Map<User>(dto);

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user is null)
            return null;

        _mapper.Map(dto, user);

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user is null)
            return false;

        _unitOfWork.Users.Remove(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<UserResponseDto>> GetUsersBySubscriptionTypeAsync(string subscriptionType)
    {
        if (!Enum.TryParse<SubscriptionType>(subscriptionType, ignoreCase: true, out var type))
            return Enumerable.Empty<UserResponseDto>();

        var users = await _unitOfWork.Users.FindAsync(u => u.SubscriptionType == type);
        return _mapper.Map<IEnumerable<UserResponseDto>>(users);
    }
}