using DocoPark.Domain.Enums;

namespace DocoPark.BusinessLogic.DTOs.Reservation
{
    public sealed record ReservationResponseDto
    {
        public int Id { get; init; }
        public int UserId { get; init; }
        public int VehicleId { get; init; }
        public int ParkingSpotId { get; init; }
        public DateTime ReservedFrom { get; init; }
        public DateTime ReservedTo { get; init; }
        public ReservationStatus Status { get; init; }
        public DateTime CreatedDate { get; init; }
        public string SpotNumber { get; init; } = string.Empty;
        public string LicensePlate { get; init; } = string.Empty;
    }
}
