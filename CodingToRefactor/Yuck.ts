export class Desk {
  id: number;
  isWindow: boolean;

  constructor(id: number, isWindow: boolean) {
    this.id = id;
    this.isWindow = isWindow;
  }
}

export class DeskBooking {
  desk: Desk;
  booking: Date;
  bookedByFirstName: string;
  bookedByLastName: string;

  constructor(
    desk: Desk,
    booking: Date,
    bookedByFirstName: string,
    bookedByLastName: string,
  ) {
    this.desk = desk;
    this.booking = booking;
    this.bookedByFirstName = bookedByFirstName;
    this.bookedByLastName = bookedByLastName;
  }
}

export class DeskFinder {
  private desks: Desk[];
  private bookings: DeskBooking[];

  constructor(desks: Desk[], bookings: DeskBooking[]) {
    this.desks = desks;
    this.bookings = bookings;
  }

  public whenIsNextDeskAvailableByDateAndDesk(start: Date, id: number): Date {
    let currentDate = new Date(start.getTime());

    if (!this.desks.some((x) => x.id === id)) {
      throw new Error("This Desk does not exist");
    }

    for (let day = 1; day < 10; day++) {
      const dayBookings = this.bookings.filter(
        (x) => x.booking.toDateString() === currentDate.toDateString(),
      );

      if (dayBookings.length > 0) {
        if (!dayBookings.some((x) => x.desk.id === id)) {
          return currentDate;
        }
      } else {
        return currentDate;
      }

      currentDate = new Date(start.getTime());
      currentDate.setDate(start.getDate() + day);
    }

    return new Date(0); // Return an invalid date to indicate no available date found within the range
  }

  public whenIsNextWindowSeatAvailableByDateAndWindow(
    start: Date,
    hasWindow = true,
  ): Date {
    let currentDate = new Date(start.getTime());

    if (!this.desks.some((x) => x.isWindow === hasWindow)) {
      throw new Error("No Desks With windows exist");
    }

    const desksWithWindows = this.desks.filter((x) => x.isWindow).length;

    for (let day = 1; day < 10; day++) {
      const dayBookings = this.bookings.filter(
        (x) => x.booking.toDateString() === currentDate.toDateString(),
      );

      if (dayBookings.length > 0) {
        if (
          dayBookings.filter((x) => x.desk.isWindow === hasWindow).length <
          desksWithWindows
        ) {
          return currentDate;
        }
      } else {
        return currentDate;
      }

      currentDate = new Date(start.getTime());
      currentDate.setDate(start.getDate() + day);
    }

    return new Date(0); // Return an invalid date to indicate no available date found within the range
  }
}
