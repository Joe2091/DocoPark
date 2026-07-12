using DocoPark.BusinessLogic.DTOs.Reservation;
using DocoPark.BusinessLogic.Interfaces;
using DocoPark.BusinessLogic.Interfaces.Services;
using DocoPark.Domain.Entities;
using DocoPark.Domain.Enums;
using System.Net.NetworkInformation;

namespace DocoPark.BusinessLogic.Services
{
    public sealed class ReservationService : IReservationService
    {
        private readonly IUnitOfWork unitofWork;

        public ReservationService(IUnitOfWork unitofWork)
        {
            this.unitofWork = unitofWork;
        }

        public async Task<ReservationResponseDto> CreateReservationAsync(CreateReservationDto dto)
        {
            var users = await unitofWork.Users.FindAsync(u => u.Id == dto.UserId);
            var user = users.FirstOrDefault()
                ?? throw new KeyNotFoundException($"User with ID {dto.UserId} not found.");

            var vehicles = await unitofWork.Vehicles.FindAsync(v => v.Id == dto.VehicleId);
            var vehicle = vehicles.FirstOrDefault()
                ?? throw new KeyNotFoundException($"Vehicle with ID {dto.VehicleId} not found.");

            var parkingSpots = await unitofWork.ParkingSpots.FindAsync(p => p.Id == dto.ParkingSpotId);
            var parkingSpot = parkingSpots.FirstOrDefault()
                ?? throw new KeyNotFoundException($"Parking spot with ID {dto.ParkingSpotId} not found.");

            var existingReservations = await unitofWork.Reservations.FindAsync(p => p.ParkingSpotId == dto.ParkingSpotId && p.Status == ReservationStatus.Active);
            var hasConflict = existingReservations.Any(
                 r => r.ReservedFrom < dto.ReservedTo && r.ReservedTo > dto.ReservedFrom);
            if (hasConflict)
                throw new InvalidOperationException("This spot is already reserved for the requested time slot.");

            var reservation = new Reservation
            {
                UserId = user.Id,
                VehicleId = vehicle.Id,
                ParkingSpotId = parkingSpot.Id,
                ReservedFrom = dto.ReservedFrom,
                ReservedTo = dto.ReservedTo,
                Status = ReservationStatus.Active,
                CreatedDate = DateTime.UtcNow
            };
            parkingSpot.SpotStatus = SpotStatus.Reserved;

            await unitofWork.Reservations.AddAsync(reservation);
            unitofWork.ParkingSpots.Update(parkingSpot);
            await unitofWork.SaveChangesAsync();

            return MapToResponse(reservation, vehicle.LicensePlate, parkingSpot.SpotNumber);
        }

        public async Task<ReservationResponseDto?> GetReservationsByIdAsync(int id)
        {
            var reservation = await unitofWork.Reservations.GetByIdAsync(id);
            if (reservation is null) return null;

            var vehicles = await unitofWork.Vehicles.FindAsync(v => v.Id == reservation.VehicleId);
            var vehicle = vehicles.First();

            var spots = await unitofWork.ParkingSpots.FindAsync(p => p.Id == reservation.ParkingSpotId);
            var spot = spots.First();

            return MapToResponse(reservation, vehicle.LicensePlate, spot.SpotNumber);
        }

        public async Task<IEnumerable<ReservationResponseDto>> GetActiveReservationsAsync()
        {
            var activeReservations = await unitofWork.Reservations.FindAsync(r => r.Status == ReservationStatus.Active);
            var results = new List<ReservationResponseDto>();

            foreach (var reservation in activeReservations)
            {
                var vehicles = await unitofWork.Vehicles.FindAsync(v => v.Id == reservation.VehicleId);
                var vehicle = vehicles.First();

                var spots = await unitofWork.ParkingSpots.FindAsync(p => p.Id == reservation.ParkingSpotId);
                var spot = spots.First();

                results.Add(MapToResponse(reservation, vehicle.LicensePlate, spot.SpotNumber));
            }

            return results;
        }

        public async Task<IEnumerable<ReservationResponseDto?>> GetReservationByUserIdAsync(int userId)
        {
            var userReservations = await unitofWork.Reservations.FindAsync(r => r.UserId == userId);
            var results = new List<ReservationResponseDto>();

            foreach (var reservation in userReservations)
            {
                var vehicles = await unitofWork.Vehicles.FindAsync(v => v.Id == reservation.VehicleId);
                var vehicle = vehicles.First();

                var spots = await unitofWork.ParkingSpots.FindAsync(p => p.Id == reservation.ParkingSpotId);
                var spot = spots.First();

                results.Add(MapToResponse(reservation, vehicle.LicensePlate, spot.SpotNumber));
            }

            return results;
        }

        public async Task<bool> CancelReservationAsync(int id)
        {
            var reservation = await unitofWork.Reservations.GetByIdAsync(id);
            if (reservation is null) return false;

            reservation.Status = ReservationStatus.Cancelled;
            unitofWork.Reservations.Update(reservation);

            var spots = await unitofWork.ParkingSpots.FindAsync(p => p.Id == reservation.ParkingSpotId);
            var spot = spots.First();
            spot.SpotStatus = SpotStatus.Available;
            unitofWork.ParkingSpots.Update(spot);

            await unitofWork.SaveChangesAsync();

            return true;
        }

        private static ReservationResponseDto MapToResponse(
            Reservation reservation, string licensePlate, string spotNumber)
        {
            return new ReservationResponseDto
            {
                Id = reservation.Id,
                UserId = reservation.UserId,
                VehicleId = reservation.VehicleId,
                ParkingSpotId = reservation.ParkingSpotId,
                ReservedFrom = reservation.ReservedFrom,
                ReservedTo = reservation.ReservedTo,
                Status = reservation.Status,
                CreatedDate = reservation.CreatedDate,
                LicensePlate = licensePlate,
                SpotNumber = spotNumber
            };
        }
    }
}