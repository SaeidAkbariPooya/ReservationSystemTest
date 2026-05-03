import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateReservationDto, ResourceAvailability } from '../models/reservation.model';

@Injectable({
    providedIn: 'root'
})
export class ReservationService {
    // آدرس base API – در صورت نیاز بر اساس آدرس بک‌اند تغییر دهید
    private apiUrl = 'https://localhost:7038/api/reservations';

    constructor(private http: HttpClient) { }

    createReservation(dto: CreateReservationDto): Observable<any> {
        return this.http.post(`${this.apiUrl}`, dto);
    }

    cancelReservation(id: number): Observable<any> {
        return this.http.delete(`${this.apiUrl}/${id}`);
    }

    getTodayFreeSlots(): Observable<ResourceAvailability[]> {
        return this.http.get<ResourceAvailability[]>(`${this.apiUrl}/free-slots/today`);
    }

    getReport(resourceId: number, fromDate: Date, toDate: Date): Observable<any> {
        const params = { resourceId, fromDate: fromDate.toISOString(), toDate: toDate.toISOString() };
        return this.http.get(`${this.apiUrl}/report`, { params });
    }
}