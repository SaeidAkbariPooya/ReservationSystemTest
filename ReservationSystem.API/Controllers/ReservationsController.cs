using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ReservationSystem.Core.DTOs;
using ReservationSystem.Core.IService;

namespace ReservationSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly IValidator<CreateReservationDto> _createValidator;

        public ReservationsController(IReservationService reservationService, IValidator<CreateReservationDto> createValidator)
        {
            _reservationService = reservationService;
            _createValidator = createValidator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReservationDto dto)
        {
            await _createValidator.ValidateAndThrowAsync(dto);
            var reservation = await _reservationService.CreateReservationAsync(dto);
            return Ok(new { Message = "رزرو با موفقیت ایجاد شد", ReservationId = reservation.Id });
        }

        [HttpPut("Cancel{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            await _reservationService.CancelReservationAsync(id);
            return Ok(new { Message = "رزرو لغو شد" });
        }

        [HttpGet("free-slots/today")]
        public async Task<IActionResult> GetTodayFreeSlots()
        {
            var slots = await _reservationService.GetTodayFreeSlotsAsync();
            return Ok(slots);
        }

        [HttpGet("report")]
        public async Task<IActionResult> GetReport([FromQuery] int resourceId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            var report = await _reservationService.GetReservationsReportAsync(resourceId, fromDate, toDate);
            return Ok(report);
        }
    }
}
