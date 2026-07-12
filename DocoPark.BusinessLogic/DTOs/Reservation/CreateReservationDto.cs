using System.ComponentModel.DataAnnotations;

namespace DocoPark.BusinessLogic.DTOs.Reservation
{
    public sealed class CreateReservationDto
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int ParkingSpotId { get; set; }
        [Required]
        public int VehicleId { get; set; }
        [Required]
        public DateTime ReservedFrom { get; set; }
        [Required]
        public DateTime ReservedTo { get; set; }

    }
}
