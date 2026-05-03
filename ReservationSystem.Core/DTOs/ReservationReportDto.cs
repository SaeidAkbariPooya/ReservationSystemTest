using ReservationSystem.Core.Enum;

namespace ReservationSystem.Core.DTOs
{
    public class ReservationReportDto
    {
        public int Id { get; set; }
        public string ResourceName { get; set; }
        public string UserName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ReservationStatus Status { get; set; }
    }
}
