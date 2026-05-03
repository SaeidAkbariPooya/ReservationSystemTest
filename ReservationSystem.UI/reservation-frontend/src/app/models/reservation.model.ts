export interface CreateReservationDto {
    resourceId: number;
    userId: number;
    startTime: Date;
    endTime: Date;
}

export interface TimeSlot {
    startTime: Date;
    endTime: Date;
}

export interface ResourceAvailability {
    resourceId: number;
    resourceName: string;
    freeSlots: TimeSlot[];
}