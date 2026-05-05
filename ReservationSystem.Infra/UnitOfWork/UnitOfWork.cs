using ReservationSystem.Core.Entities;
using ReservationSystem.Core.RepositoryInterfaces;
using ReservationSystem.Infra.Context;
using ReservationSystem.Infra.Repositories;

namespace ReservationSystem.Infra.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ReservationSystemDbContext _context;
        public IReservationRepository Reservations { get; private set; }
        public IRepository<Resource> Resources { get; private set; }
        public IRepository<User> Users { get; private set; }

        public UnitOfWork(ReservationSystemDbContext context)
        {
            _context = context;
            Reservations = new ReservationRepository(_context);
            Resources = new Repository<Resource>(_context);
            Users = new Repository<User>(_context);
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
    }
}
