class Desk {
  Id!: number;
  IsWindow!: boolean;
}

class DeskBooking {
  D!: Desk;
  booking!: Date;
  bookedByFirstName!: string;
  bookedByLastName!: string;

  constructor(D: Desk, booking: Date, bookedByFirstName: string, bookedByLastName: string) {
    this.D = D;
    this.booking = booking;
    this.bookedByFirstName = bookedByFirstName;
    this.bookedByLastName = bookedByLastName;
  }
}

class DeskFinder {
  _desks: Desk[];
  _bookings: DeskBooking[];

  constructor(desks: Desk[], bookings: DeskBooking[]) {
    this._desks = desks;
    this._bookings = bookings;
  }

  WhenIsNextDeskAvailabeBYdateAndDesk(start: Date, Id: number): Date {

    var currentDate = start;

    if (!(this._desks.some(x => x.Id === Id))) {
      throw Error("This Desk does not exist!");
    }

    for (let day = 1; day < 10; day++) {
      var daysBookings = this._bookings.filter(x => x.booking.toDateString() === currentDate.toDateString());

      if (daysBookings.length > 0) {
        if (daysBookings.some(x => x.D.Id === Id) === false) {
          //there are bookings but not for this desk
          return currentDate;
        }
      }
      else {
        //there are not bookings for the date
        return currentDate;
      }

      // start.AddDays(day)
      currentDate.setDate(start.getDate() + 1);
    }

    return new Date();
  }

  WhenIsNextWindowSeatAvailabeByDate(start: Date, hasWindow: boolean = true): Date {
    var currentDate = start;

    if (!(this._desks.some(x => x.IsWindow === hasWindow))) {
      throw Error("No Desks With windows exis!");
    }

    var desksWithWindow = this._desks.filter(x => x.IsWindow).length;

    for (let day = 1; day < 10; day++) {
      var daysBookings = this._bookings.filter(x => x.booking.toDateString() === currentDate.toDateString());

      if (daysBookings.length > 0) {
        if (daysBookings.filter(x => x.D.IsWindow === hasWindow).length < desksWithWindow) {
          //there are less bookings for desk with windows than there are desk with windows 
          return currentDate;
        }
      }
      else {
        //there are not bookings for the date, 
        //but there are desk with windows 
        return currentDate;
      }

      // start.AddDays(day)
      currentDate.setDate(start.getDate() + 1);
    }

    return new Date();
  }
}

var abc = new Desk()


const Chia = require("chai");
const sinon = require("sinon");
const sinonChai = require("sinon-chai");

const Mocha = require("mocha");
const assert = require("assert");
const mocha = new Mocha();

// Bit of a hack, but thats how to make it work in code pad
mocha.suite.emit("pre-require", this, "solution", mocha);

Mocha.describe("Test suite", function() {
  var desk1 = new Desk();
  desk1.Id = 1;
  desk1.IsWindow = true;

  var desk2 = new Desk();
  desk2.Id = 2;
  desk2.IsWindow = true;

  var desk3 = new Desk();
  desk3.Id = 3;
  desk3.IsWindow = false;

  const desks = [desk1, desk2, desk3];

  var booking0 = new DeskBooking(
    desk1,
    new Date("2022-05-04"),
    "Bob",
    "smith"
  );

  var booking1 = new DeskBooking(
    desk1,
    new Date("2022-05-05"),
    "Bob",
    "smith"
  );

  var booking2 = new DeskBooking(
    desk1,
    new Date("2022-05-06"),
    "Bob",
    "smith"
  );

  var booking3 = new DeskBooking(
    desk1,
    new Date("2022-05-07"),
    "Bob",
    "smith"
  );

  var booking4 = new DeskBooking(
    desk2,
    new Date("2022-05-04"),
    "Paul",
    "Brown"
  );

  var booking5 = new DeskBooking(
    desk2,
    new Date("2022-05-05"),
    "Paul",
    "Brown"
  );

  var booking6 = new DeskBooking(
    desk2,
    new Date("2022-05-07"),
    "Paul",
    "Brown"
  );

  const deskBookings = [booking0, booking1, booking2, booking3, booking4, booking5, booking6]

  Mocha.it("Should_Find_Next_Available_Date_Busy_On_Day_One", function() {

    const target = new DeskFinder(desks, deskBookings)
    const actual = target.WhenIsNextDeskAvailabeBYdateAndDesk(new Date("2022-05-07"), 1)

    //Should be free on the 8th as booked on the 7th
    assert.equal(new Date("2022-05-08").toDateString(), actual.toDateString())

  });

  Mocha.it("Should_Find_Next_Available_Date_Not_Busy_On_Day_One", function() {

    const target = new DeskFinder(desks, deskBookings)
    const actual = target.WhenIsNextDeskAvailabeBYdateAndDesk(new Date("2022-05-07"), 3)

    //Desk 3 is always free as never booked
    assert.equal(new Date("2022-05-07").toDateString(), actual.toDateString())

  });


  Mocha.it("Should_Find_Error_If_Desk_Does_Not_Exist", function() {

    const target = new DeskFinder(desks, deskBookings)
    const actual = target.WhenIsNextDeskAvailabeBYdateAndDesk(new Date("2022-05-07"), 3)

    var didItErorr = false;


    try{
      const actual = target.WhenIsNextDeskAvailabeBYdateAndDesk(new Date("2022-05-07"), 4)

    }
    catch{
      didItErorr = true;
    }
    //Desk 4 is not in the sample data
    assert.ok(didItErorr);

  });

  Mocha.it("Should_Find_Next_Available_Date_with_A_Window_Busy_On_Day_One_and_Two", function() {

    const target = new DeskFinder(desks, deskBookings);
    const actual = target.WhenIsNextWindowSeatAvailabeByDate(new Date("2022-05-04"), true);

    //Desk 3 is always free as never booked
    assert.equal(new Date("2022-05-06").toDateString(), actual.toDateString())

  });

  Mocha.it("Should_Find_Next_Available_Date_with_A_Window_Free_On_Day_One", function() {

    const target = new DeskFinder(desks, deskBookings);
    const actual = target.WhenIsNextWindowSeatAvailabeByDate(new Date("2022-05-03"), true);

    //Desk 3 is always free as never booked
    assert.equal(new Date("2022-05-03").toDateString(), actual.toDateString())

  });

});

mocha.run();
