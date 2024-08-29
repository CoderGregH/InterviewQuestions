using System;
using System.Collections.Generic;
using System.Linq;

public class Desk
{
    public int DeskId { get; set; }
    public bool IsWindowSeat { get; set; }
}

public class DeskBooking
{
    public Desk DeskInfo { get; set; }
    public DateTime BookingDate { get; set; }
}

class DeskFinder
{
    List<Desk> desks;
    List<DeskBooking> deskBookings;
    public DeskFinder()
    {
        desks = new List<Desk>(){
            new Desk(){DeskId = 1,IsWindowSeat=false},
            new Desk(){DeskId = 2,IsWindowSeat=true},
            new Desk(){DeskId = 3,IsWindowSeat=true},
            new Desk(){DeskId = 4,IsWindowSeat=true},
        };

        deskBookings = new List<DeskBooking>(){
            new DeskBooking(){
                 DeskInfo = new Desk(){DeskId = 1,IsWindowSeat=false},
                 BookingDate = new DateTime(2024,08,29)
                },
           new DeskBooking(){
                 DeskInfo = new Desk(){DeskId = 1,IsWindowSeat=false},
                 BookingDate = new DateTime(2024,08,31)
                },
            new DeskBooking(){
                 DeskInfo = new Desk(){DeskId = 2,IsWindowSeat=true},
                 BookingDate = new DateTime(2024,09,01)
                },
            new DeskBooking(){
                 DeskInfo = new Desk(){DeskId = 3,IsWindowSeat=true},
                 BookingDate = new DateTime(2024,09,01)
                },
        };
    }

    public DateTime GetAvailableDateForDesk(int deskId, DateTime currentDate)
    {
        var desk = desks.SingleOrDefault(x => x.DeskId == deskId);
        if (desk == null)
            throw new ArgumentException("Invalid desk ID", nameof(deskId));

        var bookings = deskBookings.Where(b => b.DeskInfo.DeskId == deskId && b.BookingDate.ToShortDateString() == currentDate.ToShortDateString());

        if (bookings.Any())
        {
            var bookingDates = deskBookings.Where(b => b.DeskInfo.DeskId == deskId).Select(x => x.BookingDate);

            currentDate = currentDate.AddDays(1);

            while (bookingDates.Any(x =>
            x.ToShortDateString() == currentDate.ToShortDateString()))
            {
                currentDate = currentDate.AddDays(1);
            }
        }

        return currentDate;
    }

    public DateTime GetAvailableDateForWindowSeat(DateTime start, bool hasWindow = true)
    {
        var deskWithWindowSeat = desks.Where(x => x.IsWindowSeat == hasWindow);
        if (deskWithWindowSeat.Count() == 0)
        {
            throw new ArgumentException("No Desks With windows exist");
        }
        var AvailableDeskWithWindowSeat = deskWithWindowSeat.Where(x => !deskBookings.Select(b => b.DeskInfo.DeskId).Contains(x.DeskId));
        if (!AvailableDeskWithWindowSeat.Any())
        {
            var bookingDates = deskBookings.
                Where(x => x.DeskInfo.IsWindowSeat == hasWindow).
                Select(x => x.BookingDate).Distinct();

            while (bookingDates.Any(x =>
            x.ToShortDateString() == start.ToShortDateString()))
            {
                start = start.AddDays(1);
            }
        }
        return start;
    }

    static void Main(string[] args)
    {
        DeskFinder deskFinder = new DeskFinder();
        try
        {
            int DeskId = 1;
            DateTime BookingDate = DateTime.Now;

            DateTime AvailableDate = deskFinder.GetAvailableDateForDesk(DeskId, BookingDate);
            Console.WriteLine("Available Date for Desk: " + AvailableDate.ToShortDateString());

            DateTime AvailableDateWithWindowSeat = deskFinder.GetAvailableDateForWindowSeat(BookingDate);
            Console.WriteLine("Available Date with window seat : " + AvailableDate.ToShortDateString());

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }
}