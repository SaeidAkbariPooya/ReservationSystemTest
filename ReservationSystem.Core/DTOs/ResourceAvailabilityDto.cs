
namespace ReservationSystem.Core.DTOs
{
    public class ResourceAvailabilityDto
    {
        public int ResourceId { get; set; }
        public string ResourceName { get; set; }
        public List<TimeSlotDto> FreeSlots { get; set; }
    }
}
