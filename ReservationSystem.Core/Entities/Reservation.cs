using ReservationSystem.Core.Enum;

namespace ReservationSystem.Core.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public int UserId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.Active;

        public Resource Resource { get; set; }
        public User User { get; set; }
    }

}
