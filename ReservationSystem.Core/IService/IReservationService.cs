using ReservationSystem.Core.DTOs;
using ReservationSystem.Core.Entities;

namespace ReservationSystem.Core.IService
{
    public interface IReservationService
    {
        Task<Reservation> CreateReservationAsync(CreateReservationDto dto);
        Task CancelReservationAsync(int reservationId);
        Task<IEnumerable<ResourceAvailabilityDto>> GetTodayFreeSlotsAsync();
        Task<IEnumerable<ReservationReportDto>> GetReservationsReportAsync(int resourceId, DateTime from, DateTime to);
    }
}
