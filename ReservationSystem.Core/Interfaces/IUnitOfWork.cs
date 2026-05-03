using ReservationSystem.Core.Entities;

namespace ReservationSystem.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IReservationRepository Reservations { get; }
        IRepository<Resource> Resources { get; }
        IRepository<User> Users { get; }
        Task<int> CompleteAsync();
    }
}
