using ReservationSystem.Core.Entities;

namespace ReservationSystem.Core.RepositoryInterfaces
{
    public interface IReservationRepository : IRepository<Reservation>
    {
        Task<bool> HasOverlapAsync(int resourceId, DateTime start, DateTime end, int? excludeReservationId = null);
        Task<int> GetActiveReservationsCountForUserAsync(int userId, DateTime currentTime);
        Task<IEnumerable<Reservation>> GetReservationsForResourceInRangeAsync(int resourceId, DateTime from, DateTime to);
    }
}
