import { Desk, DeskBooking, DeskFinder } from "./Yuck";

describe("DeskFinder", () => {
  const desk1 = new Desk(1, true);
  const desk2 = new Desk(2, true);
  const desk3 = new Desk(3, false);
  const desks = [desk1, desk2, desk3];

  function getSampleBookings(): DeskBooking[] {
    return [
      new DeskBooking(desk1, new Date(2022, 4, 4), "Bob", "Smith"),
      new DeskBooking(desk1, new Date(2022, 4, 5), "Bob", "Smith"),
      new DeskBooking(desk1, new Date(2022, 4, 6), "Bob", "Smith"),
      new DeskBooking(desk1, new Date(2022, 4, 7), "Bob", "Smith"),
      new DeskBooking(desk2, new Date(2022, 4, 4), "Paul", "Brown"),
      new DeskBooking(desk2, new Date(2022, 4, 5), "Paul", "Brown"),
      new DeskBooking(desk2, new Date(2022, 4, 7), "Paul", "Brown"),
    ];
  }

  test("Should find next available date busy on day one", () => {
    const finder = new DeskFinder(desks, getSampleBookings());
    const nextAvailable = finder.whenIsNextDeskAvailableByDateAndDesk(
      new Date(2022, 4, 7),
      1,
    );
    expect(nextAvailable).toEqual(new Date(2022, 4, 8));
  });

  test("Should find next available date not busy on day one", () => {
    const finder = new DeskFinder(desks, getSampleBookings());
    const nextAvailable = finder.whenIsNextDeskAvailableByDateAndDesk(
      new Date(2022, 4, 7),
      3,
    );
    expect(nextAvailable).toEqual(new Date(2022, 4, 7));
  });

  test("Should throw error if desk does not exist", () => {
    const finder = new DeskFinder(desks, getSampleBookings());
    expect(() => {
      finder.whenIsNextDeskAvailableByDateAndDesk(new Date(2022, 4, 7), 4);
    }).toThrow("This Desk does not exist");
  });

  test("Should find next available date with a window busy on day one and two", () => {
    const finder = new DeskFinder(desks, getSampleBookings());
    const nextAvailable = finder.whenIsNextWindowSeatAvailableByDateAndWindow(
      new Date(2022, 4, 4),
      true,
    );
    expect(nextAvailable).toEqual(new Date(2022, 4, 6));
  });

  test("Should find next available date with a window free on day one", () => {
    const finder = new DeskFinder(desks, getSampleBookings());
    const nextAvailable = finder.whenIsNextWindowSeatAvailableByDateAndWindow(
      new Date(2022, 4, 3),
      true,
    );
    expect(nextAvailable).toEqual(new Date(2022, 4, 3));
  });
});
