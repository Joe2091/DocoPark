using AutoMapper;
using DocoPark.BusinessLogic.DTOs.Vehicle;
using DocoPark.BusinessLogic.Interfaces;
using DocoPark.BusinessLogic.Interfaces.Services;
using DocoPark.Domain.Entities;

namespace DocoPark.BusinessLogic.Services;

public sealed class VehicleService : IVehicleService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IMapper mapper;

    public VehicleService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        this.unitOfWork = unitOfWork;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<VehicleResponseDto>> GetAllVehiclesAsync()
    {
        var vehicles = await unitOfWork.Vehicles.GetAllAsync();
        var results = new List<VehicleResponseDto>();

        foreach (var vehicle in vehicles)
        {
            string? ownerName = null;
            if (vehicle.UserId.HasValue)
            {
                var users = await unitOfWork.Users.FindAsync(u => u.Id == vehicle.UserId);
                ownerName = users.FirstOrDefault()?.Name;
            }

            results.Add(new VehicleResponseDto
            {
                Id = vehicle.Id,
                LicensePlate = vehicle.LicensePlate,
                Color = vehicle.Color,
                VehicleType = vehicle.VehicleType,
                UserId = vehicle.UserId,
                OwnerName = ownerName
            });
        }

        return results;
    }

    public async Task<VehicleResponseDto?> GetVehicleByIdAsync(int id)
    {
        var vehicle = await unitOfWork.Vehicles.GetByIdAsync(id);
        if (vehicle is null) return null;

        string? ownerName = null;
        if (vehicle.UserId.HasValue)
        {
            var users = await unitOfWork.Users.FindAsync(u => u.Id == vehicle.UserId);
            ownerName = users.FirstOrDefault()?.Name;
        }

        return new VehicleResponseDto
        {
            Id = vehicle.Id,
            LicensePlate = vehicle.LicensePlate,
            Color = vehicle.Color,
            VehicleType = vehicle.VehicleType,
            UserId = vehicle.UserId,
            OwnerName = ownerName
        };
    }

    public async Task<IEnumerable<VehicleResponseDto>> GetVehiclesByUserIdAsync(int userId)
    {
        var vehicles = await unitOfWork.Vehicles.FindAsync(v => v.UserId == userId);
        var users = await unitOfWork.Users.FindAsync(u => u.Id == userId);
        var ownerName = users.FirstOrDefault()?.Name;

        return vehicles.Select(vehicle => new VehicleResponseDto
        {
            Id = vehicle.Id,
            LicensePlate = vehicle.LicensePlate,
            Color = vehicle.Color,
            VehicleType = vehicle.VehicleType,
            UserId = vehicle.UserId,
            OwnerName = ownerName
        });
    }

    public async Task<VehicleResponseDto> CreateVehicleAsync(CreateVehicleDto dto)
    {
        var vehicle = mapper.Map<Vehicle>(dto);

        await unitOfWork.Vehicles.AddAsync(vehicle);
        await unitOfWork.SaveChangesAsync();

        string? ownerName = null;
        if (vehicle.UserId.HasValue)
        {
            var users = await unitOfWork.Users.FindAsync(u => u.Id == vehicle.UserId);
            ownerName = users.FirstOrDefault()?.Name;
        }

        return new VehicleResponseDto
        {
            Id = vehicle.Id,
            LicensePlate = vehicle.LicensePlate,
            Color = vehicle.Color,
            VehicleType = vehicle.VehicleType,
            UserId = vehicle.UserId,
            OwnerName = ownerName
        };
    }

    public async Task<VehicleResponseDto?> UpdateVehicleAsync(int id, UpdateVehicleDto dto)
    {
        var vehicle = await unitOfWork.Vehicles.GetByIdAsync(id);
        if (vehicle is null)
            return null;

        mapper.Map(dto, vehicle);

        unitOfWork.Vehicles.Update(vehicle);
        await unitOfWork.SaveChangesAsync();

        return mapper.Map<VehicleResponseDto>(vehicle);
    }

    public async Task<bool> DeleteVehicleAsync(int id)
    {
        var vehicle = await unitOfWork.Vehicles.GetByIdAsync(id);
        if (vehicle is null)
            return false;

        unitOfWork.Vehicles.Remove(vehicle);
        await unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<VehicleResponseDto?> GetVehicleByLicensePlateAsync(string licensePlate)
    {
        var vehicles = await unitOfWork.Vehicles.FindAsync(v => v.LicensePlate == licensePlate);
        var vehicle = vehicles.FirstOrDefault();

        if (vehicle is null) return null;

        string? ownerName = null;
        if (vehicle.UserId.HasValue)
        {
            var users = await unitOfWork.Users.FindAsync(u => u.Id == vehicle.UserId);
            ownerName = users.FirstOrDefault()?.Name;
        }

        return new VehicleResponseDto
        {
            Id = vehicle.Id,
            LicensePlate = vehicle.LicensePlate,
            Color = vehicle.Color,
            VehicleType = vehicle.VehicleType,
            UserId = vehicle.UserId,
            OwnerName = ownerName
        };
    }
}