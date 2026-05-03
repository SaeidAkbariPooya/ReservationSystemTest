import { Component, OnInit } from '@angular/core';
import * as moment from 'moment-jalaali';
import { ReservationService } from './services/reservation.service';
import { ResourceAvailability, CreateReservationDto } from './models/reservation.model';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  // لیست منابع با نوبت‌های آزاد امروز
  resourcesAvailability: ResourceAvailability[] = [];

  // مقادیر فرم رزرو (برای هر منبع جداگانه نیست، برای سادگی یک فرم کلی)
  selectedResourceId!: number;
  selectedDatePersian: string = '';     // تاریخ شمسی به صورت رشته "1403/02/15"
  selectedTime: string = '09:00';       // ساعت شروع
  durationHours: number = 1;             // مدت زمان (حداکثر 4)

  message: string = '';
  messageType: 'success' | 'error' = 'success';

  constructor(private reservationService: ReservationService) { }

  ngOnInit(): void {
    this.loadFreeSlots();
  }

  // دریافت نوبت‌های آزاد امروز از سرور
  loadFreeSlots(): void {
    this.reservationService.getTodayFreeSlots().subscribe({
      next: (data) => {
        this.resourcesAvailability = data;
      },
      error: (err) => {
        this.showMessage('خطا در دریافت نوبت‌های آزاد', 'error');
        console.error(err);
      }
    });
  }

  // تابع رزرو برای یک منبع مشخص (با دریافت resourceId از دکمه)
  reserveResource(resourceId: number): void {
    if (!this.selectedDatePersian) {
      this.showMessage('لطفاً تاریخ را انتخاب کنید', 'error');
      return;
    }

    // تبدیل تاریخ شمسی به میلادی
    const startMoment = moment(this.selectedDatePersian, 'jYYYY/jMM/jDD');
    if (!startMoment.isValid()) {
      this.showMessage('تاریخ نامعتبر است', 'error');
      return;
    }

    const [hour, minute] = this.selectedTime.split(':');
    startMoment.hour(+hour).minute(+minute);
    const startDate = startMoment.toDate();

    // محاسبه زمان پایان
    const endDate = new Date(startDate.getTime() + this.durationHours * 60 * 60 * 1000);

    // ساخت DTO – کاربر ثابت 1 (فرض می‌کنیم چنین کاربری در دیتابیس وجود دارد)
    const dto: CreateReservationDto = {
      resourceId: resourceId,
      userId: 1,   // کاربر نمونه
      startTime: startDate,
      endTime: endDate
    };

    this.reservationService.createReservation(dto).subscribe({
      next: (response) => {
        this.showMessage(`رزرو با موفقیت انجام شد (شناسه: ${response.reservationId})`, 'success');
        // پس از رزرو موفق، لیست نوبت‌های آزاد را به‌روز می‌کنیم
        this.loadFreeSlots();
        // (اختیاری) فرم را پاک کنید
        this.selectedDatePersian = '';
        this.selectedTime = '09:00';
        this.durationHours = 1;
      },
      error: (err) => {
        const errorMsg = err.error?.message || 'خطا در ثبت رزرو';
        this.showMessage(errorMsg, 'error');
        console.error(err);
      }
    });
  }

  // لغو رزرو (برای نمونه یک رزرو با شناسه ثابت 1 – قابل گسترش)
  // در یک پیاده‌سازی واقعی باید لیست رزروهای کاربر را نمایش دهید و امکان لغو بدهید.
  // برای سادگی، این تابع فقط یک نمونه است.
  cancelSampleReservation(): void {
    const reservationId = 1; // نمونه – بهتر است از کاربر گرفته شود
    this.reservationService.cancelReservation(reservationId).subscribe({
      next: () => {
        this.showMessage('رزرو لغو شد', 'success');
        this.loadFreeSlots();
      },
      error: (err) => {
        const errorMsg = err.error?.message || 'خطا در لغو رزرو';
        this.showMessage(errorMsg, 'error');
      }
    });
  }

  private showMessage(msg: string, type: 'success' | 'error'): void {
    this.message = msg;
    this.messageType = type;
    setTimeout(() => {
      this.message = '';
    }, 5000);
  }
}