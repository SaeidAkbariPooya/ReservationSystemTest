
namespace ReservationSystem.Core.DTOs
{
    public class ReportRequest
    {
        public int ResourceId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
