
namespace ReservationSystem.Core.Entities
{
    public class Resource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }  
        public int MaxConcurrentUsage { get; set; } 

        public ICollection<Reservation> Reservations { get; set; }
    }

}
