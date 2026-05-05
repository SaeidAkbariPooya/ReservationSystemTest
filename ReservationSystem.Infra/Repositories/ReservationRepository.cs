using Microsoft.EntityFrameworkCore;
using ReservationSystem.Core.Entities;
using ReservationSystem.Core.Enum;
using ReservationSystem.Core.RepositoryInterfaces;
using ReservationSystem.Infra.Context;

namespace ReservationSystem.Infra.Repositories
{
    public class ReservationRepository : Repository<Reservation>, IReservationRepository
    {
        public ReservationRepository(ReservationSystemDbContext context) : base(context) { }

        public async Task<bool> HasOverlapAsync(int resourceId, DateTime start, DateTime end, int? excludeReservationId = null)
        {
            var query = _context.Reservations
                .Where(r => r.ResourceId == resourceId &&
                            r.Status == ReservationStatus.Active &&
                            r.StartTime < end &&
                            r.EndTime > start);

            if (excludeReservationId.HasValue)
                query = query.Where(r => r.Id != excludeReservationId.Value);

            return await query.AnyAsync();
        }

        public async Task<int> GetActiveReservationsCountForUserAsync(int userId, DateTime currentTime)
        {
            return await _context.Reservations
                .CountAsync(r => r.UserId == userId &&
                                 r.Status == ReservationStatus.Active &&
                                 r.EndTime > currentTime);
        }

        public async Task<IEnumerable<Reservation>> GetReservationsForResourceInRangeAsync(int resourceId, DateTime from, DateTime to)
        {
            return await _context.Reservations
                .Include(r => r.Resource)
                .Include(r => r.User)
                .Where(r => r.ResourceId == resourceId &&
                            r.StartTime >= from &&
                            r.EndTime <= to)
                .OrderBy(r => r.StartTime)
                .ToListAsync();
        }

    }
}
