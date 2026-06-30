using AutoMapper;
using DocoPark.BusinessLogic.DTOs.Vehicle;
using DocoPark.BusinessLogic.Interfaces;
using DocoPark.BusinessLogic.Interfaces.Services;
using DocoPark.Domain.Entities;

namespace DocoPark.BusinessLogic.Services;

public sealed class VehicleService : IVehicleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public VehicleService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<VehicleResponseDto>> GetAllVehiclesAsync()
    {
        var vehicles = await _unitOfWork.Vehicles.GetAllAsync();
        return _mapper.Map<IEnumerable<VehicleResponseDto>>(vehicles);
    }

    public async Task<VehicleResponseDto?> GetVehicleByIdAsync(int id)
    {
        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(id);
        return vehicle is null ? null : _mapper.Map<VehicleResponseDto>(vehicle);
    }

    public async Task<IEnumerable<VehicleResponseDto>> GetVehiclesByUserIdAsync(int userId)
    {
        var vehicles = await _unitOfWork.Vehicles.FindAsync(v => v.UserId == userId);
        return _mapper.Map<IEnumerable<VehicleResponseDto>>(vehicles);
    }

    public async Task<VehicleResponseDto?> GetVehicleByLicensePlateAsync(string licensePlate)
    {
        var vehicles = await _unitOfWork.Vehicles.FindAsync(v => v.LicensePlate == licensePlate);
        var vehicle = vehicles.FirstOrDefault();
        return vehicle is null ? null : _mapper.Map<VehicleResponseDto>(vehicle);
    }

    public async Task<VehicleResponseDto> CreateVehicleAsync(CreateVehicleDto dto)
    {
        var vehicle = _mapper.Map<Vehicle>(dto);

        await _unitOfWork.Vehicles.AddAsync(vehicle);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<VehicleResponseDto>(vehicle);
    }

    public async Task<VehicleResponseDto?> UpdateVehicleAsync(int id, UpdateVehicleDto dto)
    {
        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(id);
        if (vehicle is null)
            return null;

        _mapper.Map(dto, vehicle);

        _unitOfWork.Vehicles.Update(vehicle);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<VehicleResponseDto>(vehicle);
    }

    public async Task<bool> DeleteVehicleAsync(int id)
    {
        var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(id);
        if (vehicle is null)
            return false;

        _unitOfWork.Vehicles.Remove(vehicle);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}