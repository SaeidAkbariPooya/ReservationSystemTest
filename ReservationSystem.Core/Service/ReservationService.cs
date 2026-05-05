using AutoMapper;
using ReservationSystem.Core.DTOs;
using ReservationSystem.Core.IService;
using ReservationSystem.Core.Entities;
using ReservationSystem.Core.Enum;
using ReservationSystem.Core.IRepositories;


namespace ReservationSystem.Core.Service
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReservationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Reservation> CreateReservationAsync(CreateReservationDto dto)
        {
            // 1. حداکثر زمان 4 ساعت
            var duration = dto.EndTime - dto.StartTime;
            if (duration.TotalHours > 4)
                throw new ApplicationException("مدت زمان رزرو نمی‌تواند بیشتر از 4 ساعت باشد.");

            // 2. بررسی تداخل زمانی
            var hasOverlap = await _unitOfWork.Reservations.HasOverlapAsync(dto.ResourceId, dto.StartTime, dto.EndTime);
            if (hasOverlap)
                throw new ApplicationException("منبع مورد نظر در این بازه زمانی قبلاً رزرو شده است.");

            // 3. حداکثر 2 رزرو فعال همزمان برای کاربر
            var activeCount = await _unitOfWork.Reservations.GetActiveReservationsCountForUserAsync(dto.UserId, DateTime.Now);
            if (activeCount >= 2)
                throw new ApplicationException("هر کاربر حداکثر می‌تواند 2 رزرو فعال همزمان داشته باشد.");

            var reservation = new Reservation
            {
                ResourceId = dto.ResourceId,
                UserId = dto.UserId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Status = ReservationStatus.Active
            };

            await _unitOfWork.Reservations.AddAsync(reservation);
            await _unitOfWork.CompleteAsync();
            return reservation;
        }

        public async Task CancelReservationAsync(int reservationId)
        {
            var reservation = await _unitOfWork.Reservations.GetByIdAsync(reservationId);
            if (reservation == null)
                throw new KeyNotFoundException("رزرو مورد نظر یافت نشد.");

            if (reservation.StartTime <= DateTime.Now)
                throw new ApplicationException("فقط رزروهایی که هنوز شروع نشده‌اند قابل لغو هستند.");

            reservation.Status = ReservationStatus.Cancelled;
            _unitOfWork.Reservations.Update(reservation);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<ResourceAvailabilityDto>> GetTodayFreeSlotsAsync()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var allResources = await _unitOfWork.Resources.GetAllAsync();

            var result = new List<ResourceAvailabilityDto>();

            foreach (var resource in allResources)
            {
                // دریافت رزروهای فعال امروز
                var reservations = await _unitOfWork.Reservations.FindAsync(r =>
                    r.ResourceId == resource.Id &&
                    r.Status == ReservationStatus.Active &&
                    r.StartTime < tomorrow &&
                    r.EndTime > today);

                var ordered = reservations.OrderBy(r => r.StartTime).ToList();
                var freeSlots = new List<TimeSlotDto>();

                DateTime slotStart = today;
                foreach (var res in ordered)
                {
                    if (res.StartTime > slotStart)
                    {
                        freeSlots.Add(new TimeSlotDto { StartTime = slotStart, EndTime = res.StartTime });
                    }
                    slotStart = res.EndTime > slotStart ? res.EndTime : slotStart;
                }

                if (slotStart < tomorrow)
                {
                    freeSlots.Add(new TimeSlotDto { StartTime = slotStart, EndTime = tomorrow });
                }

                result.Add(new ResourceAvailabilityDto
                {
                    ResourceId = resource.Id,
                    ResourceName = resource.Name,
                    FreeSlots = freeSlots
                });
            }

            return result;
        }

        public async Task<IEnumerable<ReservationReportDto>> GetReservationsReportAsync(int resourceId, DateTime from, DateTime to)
        {
            var reservations = await _unitOfWork.Reservations.GetReservationsForResourceInRangeAsync(resourceId, from, to);
            return _mapper.Map<IEnumerable<ReservationReportDto>>(reservations);
        }
    }
}