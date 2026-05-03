
using ReservationSystem.Core.Enum;

namespace ReservationSystem.Core.DTOs
{
    public class ReservationDto
    {
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ReservationStatus Status { get; set; }
    }

}
