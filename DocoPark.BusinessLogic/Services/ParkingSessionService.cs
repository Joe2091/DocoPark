using DocoPark.BusinessLogic.DTOs.ParkingSession;
using DocoPark.BusinessLogic.Interfaces;
using DocoPark.BusinessLogic.Interfaces.Services;
using DocoPark.Domain.Entities;
using DocoPark.Domain.Enums;

namespace DocoPark.BusinessLogic.Services
{
    public sealed class ParkingSessionService : IParkingSessionService
    {
        private readonly IUnitOfWork unitofWork;
        private readonly IPricingService pricingService;

        public ParkingSessionService(IUnitOfWork unitofWork, IPricingService pricingService)
        {
            this.unitofWork = unitofWork;
            this.pricingService = pricingService;
        }

        public async Task<ParkingSessionResponseDto> CheckInAsync(CheckInRequestDto dto)
        {
            var vehicles = await unitofWork.Vehicles.FindAsync(v => v.LicensePlate == dto.LicensePlate);
            var vehicle = vehicles.FirstOrDefault()
            ?? throw new KeyNotFoundException($"Vehicle with plate '{dto.LicensePlate}' not found.");

            var existingSessions = await unitofWork.ParkingSessions.FindAsync(s => s.VehicleId == vehicle.Id && s.CheckOutTime == null);
            if (existingSessions.Any())
                throw new InvalidOperationException($"Vehicle '{dto.LicensePlate}'is already checked in");

            var availableSpots = await unitofWork.ParkingSpots.FindAsync(p => p.SpotStatus == SpotStatus.Available);
            var availableSpot = availableSpots.FirstOrDefault()
                ?? throw new InvalidOperationException($"No Available Parking spots '{availableSpots}'");

            var session = new ParkingSession
            {
                VehicleId = vehicle.Id,
                ParkingSpotId = availableSpot.Id,
                UserId = vehicle.UserId,
                CheckInTime = DateTime.UtcNow,
                CheckOutTime = null,
                TotalCost = 0,
                IsPaid = false
            };

            availableSpot.SpotStatus = SpotStatus.Occupied;
            availableSpot.CurrentSessionId = null;

            await unitofWork.ParkingSessions.AddAsync(session);
            await unitofWork.SaveChangesAsync();

            availableSpot.CurrentSessionId = session.Id;
            unitofWork.ParkingSpots.Update(availableSpot);
            await unitofWork.SaveChangesAsync();

            return MapToResponse(session, vehicle.LicensePlate, availableSpot.SpotNumber, availableSpot.SpotStatus);
        }

        public async Task<ParkingSessionResponseDto> CheckOutAsync(CheckOutRequestDto dto)
        {
            var vehicles = await unitofWork.Vehicles.FindAsync(v => v.LicensePlate == dto.LicensePlate);
            var vehicle = vehicles.FirstOrDefault()
            ?? throw new KeyNotFoundException($"Vehicle with plate '{dto.LicensePlate}' not found.");

            var existingSessions = await unitofWork.ParkingSessions.FindAsync(s => s.VehicleId == vehicle.Id && s.CheckOutTime == null);
            var session = existingSessions.FirstOrDefault()
             ?? throw new KeyNotFoundException($"Vehicle is not already parked.");

            var checkOutTime = DateTime.UtcNow;
            var subscriptionType = vehicle.User?.SubscriptionType ?? SubscriptionType.Hourly;

            var cost = pricingService.CalculateSessionCost(session.CheckInTime, checkOutTime, subscriptionType);

            session.CheckOutTime = checkOutTime;
            session.TotalCost = cost;
            session.IsPaid = subscriptionType == SubscriptionType.Monthly;

            unitofWork.ParkingSessions.Update(session);

            var occupiedSpots = await unitofWork.ParkingSpots.FindAsync(p => p.Id == session.ParkingSpotId);
            var occupiedSpot = occupiedSpots.First();

            occupiedSpot.SpotStatus = SpotStatus.Available;
            occupiedSpot.CurrentSessionId = null;
            unitofWork.ParkingSpots.Update(occupiedSpot);
            await unitofWork.SaveChangesAsync();

            return MapToResponse(session, vehicle.LicensePlate, occupiedSpot.SpotNumber, occupiedSpot.SpotStatus);

        }

        public async Task<IEnumerable<ParkingSessionResponseDto>> GetActiveSessionsAsync()
        {
            var allSessions = await unitofWork.ParkingSessions.FindAsync(p => p.CheckOutTime == null);
            var results = new List<ParkingSessionResponseDto>();

            foreach (var session in allSessions) {
                var vehicles = await unitofWork.Vehicles.FindAsync(v => v.Id == session.VehicleId);
                var vehicle = vehicles.First();

                var currentSpots = await unitofWork.ParkingSpots.FindAsync(v => v.Id == session.ParkingSpotId);
                var currentSpot = currentSpots.First();

                results.Add(MapToResponse(session, vehicle.LicensePlate, currentSpot.SpotNumber, currentSpot.SpotStatus));

            }
            return results;
        }

        public async Task<ParkingSessionResponseDto?> GetSessionByIdAsync(int id)
        {
            var session = await unitofWork.ParkingSessions.GetByIdAsync(id);

            if (session is null) return null;

            var vehicles = await unitofWork.Vehicles.FindAsync(v => v.Id == session.VehicleId);
            var vehicle = vehicles.First();

            var spots = await unitofWork.ParkingSpots.FindAsync(p => p.Id == session.ParkingSpotId);
            var spot = spots.First();
            return MapToResponse(session, vehicle.LicensePlate, spot.SpotNumber, spot.SpotStatus);

        }


        private static ParkingSessionResponseDto MapToResponse(
            ParkingSession session, string licensePlate, string spotNumber, SpotStatus spotStatus)
        {
            var response = new ParkingSessionResponseDto
            {
                Id = session.Id,
                CheckInTime = session.CheckInTime,
                CheckOutTime = session.CheckOutTime,
                TotalCost = session.TotalCost,
                LicensePlate = licensePlate,
                SpotNumber = spotNumber,
                SpotStatus = spotStatus,
            };
            return response;
        }
    }
}
