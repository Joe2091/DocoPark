using DocoPark.Domain.Enums;

namespace DocoPark.Domain.Entities
{
    public class ParkingSpot
    {
        public int Id { get; set; }
        public string SpotNumber { get; set; } = string.Empty;
        public SpotStatus SpotStatus { get; set; }
        public int? CurrentSessionId { get; set; }

        // Navigation properties
        public ParkingSession? CurrentSession { get; set; }
        public ICollection<ParkingSession> ParkingSessions { get; set; } = new List<ParkingSession>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}