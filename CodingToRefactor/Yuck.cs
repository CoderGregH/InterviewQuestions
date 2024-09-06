using NUnit.Framework;
using NUnitLite;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using static Yuck.Program;

namespace Yuck
{
    class Program
    {
        static int Main(string[] args)
        {

            return new AutoRun(Assembly.GetCallingAssembly()).Execute(new String[] { "--labels=All" });
        }

        public class Desk
        {
            public float DeskId { get; set; }
            public bool IsWindow { get; set; }
        }

        public class DeskBooking
        {
            public Desk Desk { get; set; }
            public DateTime Booking { get; set; }
            public string BookedByFirstName { get; set; }
            public string BookedByLastName { get; set; }
        }

        public class DeskFinder
        {
            private readonly List<Desk> _desks;
            private readonly List<DeskBooking> _bookings;

            public DeskFinder(
                List<Desk> desks,
                List<DeskBooking> bookings)
            {
                _bookings = bookings;
                _desks = desks;
            }

            public DateTime GetNextAvailableDeskDateWithDeskId(DateTime start, float deskId)
            {
                try
                {
                    var currentDate = start;

                    if (!_desks.Any(x => x.DeskId == deskId))
                    {
                        throw new Exception("This Desk does not exist");
                    }
                    else
                    {
                        for (int day = 1; day < 10; day++)
                        {
                            var bookedDays = _bookings.Where(x => x.Booking.ToShortDateString() == currentDate.ToShortDateString() && x.Desk.DeskId == deskId);

                            if (!bookedDays.Any())
                            {
                                return currentDate; //available
                            }
                            else
                            {
                                currentDate = start.AddDays(day);
                            }
                        }
                        throw new Exception("No availability for next few days. Please try another date");
                    }
                }
                catch (Exception)
                {
                    throw;
                }

            }

            public DateTime GetNextAvailableWindowSeatDeskDate(DateTime start)
            {
                try
                {
                    var currentDate = start;
                    var desksWithWindows = _desks.Count(x => x.IsWindow);

                    if (desksWithWindows > 0)
                    {
                        for (int day = 1; day < 10; day++)
                        {

                            var bookedDays = _bookings.Where(x => x.Booking.ToShortDateString() == currentDate.ToShortDateString());
                            if (!bookedDays.Any()) //if no bookings then available
                            {
                                return currentDate;
                            }
                            else
                            {
                                if (bookedDays.Count(x => x.Desk.IsWindow) < desksWithWindows)
                                {
                                    //there are less bookings for desk with windows than there are desk with windows 
                                    return currentDate;
                                }
                                else
                                {
                                    currentDate = start.AddDays(day);
                                }
                            }
                        }
                        throw new Exception("No Window Desks available for next few days. Please try another date");
                    }
                    else
                    {
                        throw new Exception("No Desks With windows exist");
                    }
                }
                catch (Exception)
                {
                    throw;
                }

            }

        }
    }
    }

    [TestFixture]
    public class MyDeskTests
    {
        private Desk desk1 => new Desk()
        {
            DeskId = 1,
            IsWindow = true
        };

        private Desk desk2 = new Desk()
        {
            DeskId = 2,
            IsWindow = true
        };

        private Desk desk3 = new Desk()
        {
            DeskId = 3,
            IsWindow = false
        };

        private List<Desk> desks => new List<Desk>() { desk1, desk2, desk3 };

        internal List<DeskBooking> GetSample()
        {
            var list = new List<DeskBooking>();
            var booking0 = new DeskBooking { Desk = desk1, Booking = new DateTime(2022, 05, 04), BookedByFirstName = "Bob", BookedByLastName = "smith" };
            var booking1 = new DeskBooking { Desk = desk1, Booking = new DateTime(2022, 05, 05), BookedByFirstName = "Bob", BookedByLastName = "smith" };
            var booking2 = new DeskBooking { Desk = desk1, Booking = new DateTime(2022, 05, 06), BookedByFirstName = "Bob", BookedByLastName = "smith" };
            var booking3 = new DeskBooking { Desk = desk1, Booking = new DateTime(2022, 05, 07), BookedByFirstName = "Bob", BookedByLastName = "smith" };
            var booking4 = new DeskBooking { Desk = desk2, Booking = new DateTime(2022, 05, 04), BookedByFirstName = "Paul", BookedByLastName = "Brown" };
            var booking5 = new DeskBooking { Desk = desk2, Booking = new DateTime(2022, 05, 05), BookedByFirstName = "Paul", BookedByLastName = "Brown" };
            var booking6 = new DeskBooking { Desk = desk2, Booking = new DateTime(2022, 05, 07), BookedByFirstName = "Paul", BookedByLastName = "Brown" };

            list.Add(booking0);
            list.Add(booking1);
            list.Add(booking2);
            list.Add(booking3);
            list.Add(booking4);
            list.Add(booking5);

            return list;
        }

        internal List<DeskBooking> GetSampleForDeskUnavailableForNextDays()
        {
            var list = new List<DeskBooking>();
            var booking0 = new DeskBooking { Desk = desk1, Booking = new DateTime(2022, 05, 04), BookedByFirstName = "Bob", BookedByLastName = "smith" };
            var booking1 = new DeskBooking { Desk = desk1, Booking = new DateTime(2022, 05, 05), BookedByFirstName = "Bob", BookedByLastName = "smith" };
            var booking2 = new DeskBooking { Desk = desk1, Booking = new DateTime(2022, 05, 06), BookedByFirstName = "Bob", BookedByLastName = "smith" };
            var booking3 = new DeskBooking { Desk = desk1, Booking = new DateTime(2022, 05, 07), BookedByFirstName = "Bob", BookedByLastName = "smith" };
            var booking4 = new DeskBooking { Desk = desk1, Booking = new DateTime(2022, 05, 08), BookedByFirstName = "Paul", BookedByLastName = "Brown" };
            var booking5 = new DeskBooking { Desk = desk1, Booking = new DateTime(2022, 05, 09), BookedByFirstName = "Paul", BookedByLastName = "Brown" };
            var booking6 = new DeskBooking { Desk = desk1, Booking = new DateTime(2022, 05, 10), BookedByFirstName = "Paul", BookedByLastName = "Brown" };
            var booking7 = new DeskBooking { Desk = desk1, Booking = new DateTime(2022, 05, 11), BookedByFirstName = "Paul", BookedByLastName = "Brown" };
            var booking8 = new DeskBooking { Desk = desk1, Booking = new DateTime(2022, 05, 12), BookedByFirstName = "Paul", BookedByLastName = "Brown" };
            var booking9 = new DeskBooking { Desk = desk1, Booking = new DateTime(2022, 05, 13), BookedByFirstName = "Paul", BookedByLastName = "Brown" };
            var booking10 = new DeskBooking { Desk = desk2, Booking = new DateTime(2022, 05, 04), BookedByFirstName = "Bob", BookedByLastName = "smith" };
            var booking11 = new DeskBooking { Desk = desk2, Booking = new DateTime(2022, 05, 05), BookedByFirstName = "Bob", BookedByLastName = "smith" };
            var booking12 = new DeskBooking { Desk = desk2, Booking = new DateTime(2022, 05, 06), BookedByFirstName = "Bob", BookedByLastName = "smith" };
            var booking13 = new DeskBooking { Desk = desk2, Booking = new DateTime(2022, 05, 07), BookedByFirstName = "Bob", BookedByLastName = "smith" };
            var booking14 = new DeskBooking { Desk = desk2, Booking = new DateTime(2022, 05, 08), BookedByFirstName = "Paul", BookedByLastName = "Brown" };
            var booking15 = new DeskBooking { Desk = desk2, Booking = new DateTime(2022, 05, 09), BookedByFirstName = "Paul", BookedByLastName = "Brown" };
            var booking16 = new DeskBooking { Desk = desk2, Booking = new DateTime(2022, 05, 10), BookedByFirstName = "Paul", BookedByLastName = "Brown" };
            var booking17 = new DeskBooking { Desk = desk2, Booking = new DateTime(2022, 05, 11), BookedByFirstName = "Paul", BookedByLastName = "Brown" };
            var booking18 = new DeskBooking { Desk = desk2, Booking = new DateTime(2022, 05, 12), BookedByFirstName = "Paul", BookedByLastName = "Brown" };
            var booking19 = new DeskBooking { Desk = desk2, Booking = new DateTime(2022, 05, 13), BookedByFirstName = "Paul", BookedByLastName = "Brown" };


            list.Add(booking0);
            list.Add(booking1);
            list.Add(booking2);
            list.Add(booking3);
            list.Add(booking4);
            list.Add(booking5);
            list.Add(booking6);
            list.Add(booking7);
            list.Add(booking8);
            list.Add(booking9);
            list.Add(booking10);
            list.Add(booking11);
            list.Add(booking12);
            list.Add(booking13);
            list.Add(booking14);
            list.Add(booking15);
            list.Add(booking16);
            list.Add(booking17);
            list.Add(booking18);
            list.Add(booking19);
            return list;
        }

        #region "Desk by Id Check"

        [Test]
        public void ShouldFindNextAvailableDateForDesk()
        {
            var target = new DeskFinder(desks, GetSample());
            var actual = target.GetNextAvailableDeskDateWithDeskId(new DateTime(2022, 05, 07), 1);

            //Should be free on the 8th as booked on the 7th
            Assert.AreEqual(new DateTime(2022, 05, 08), actual);
        }

    [Test]
    public void IsSameDateAvailableForDesk()
    {
        var target = new DeskFinder(desks, GetSample());
        var actual = target.GetNextAvailableDeskDateWithDeskId(new DateTime(2022, 05, 07), 3);

        //Desk 3 is always free as never booked
        Assert.AreEqual(new DateTime(2022, 05, 07), actual);
    }

    [Test]
        public void ShouldFindErrorIfDeskDoesNotExist()
        {
            var target = new DeskFinder(desks, GetSample());

            var didItErorr = false;

            try
            {
                var actual = target.GetNextAvailableDeskDateWithDeskId(new DateTime(2022, 05, 07), 4);
            }
            catch
            {
                didItErorr = true;
            }

            //Desk 4 is not in the sample data
            Assert.AreEqual(true, didItErorr);
        }

        [Test]
        public void ShouldFindErrorIfDateNotAvailableForNextFewDays()
        {
            var target = new DeskFinder(desks, GetSampleForDeskUnavailableForNextDays());

            var didItErorr = false;

            try
            {
                var actual = target.GetNextAvailableDeskDateWithDeskId(new DateTime(2022, 05, 04), 2);
            }
            catch
            {
                didItErorr = true;
            }

            Assert.AreEqual(true, didItErorr);
        }

        #endregion

        #region "Desk with Window"

        [Test]
        public void ShouldFindNextAvailableDateForWindowSeatDesk()
        {
            var target = new DeskFinder(desks, GetSample());
            var actual = target.GetNextAvailableWindowSeatDeskDate(new DateTime(2022, 05, 04));

            //Should be free on the 6th as desk 1 and 2 booked on the 4th and 5th
            Assert.AreEqual(new DateTime(2022, 05, 06), actual);
        }

        [Test]
        public void IsSameDateAvailableForWindowSeatDesk()
        {
            var target = new DeskFinder(desks, GetSample());
            var actual = target.GetNextAvailableWindowSeatDeskDate(new DateTime(2022, 05, 03));

            //Should be free on the 3 as no bookings are there
            Assert.AreEqual(new DateTime(2022, 05, 03), actual);

        }

        [Test]
        public void ShouldFindErrorIfWindowSeatDateNotAvailableForNextFewDays()
        {
            var target = new DeskFinder(desks, GetSampleForDeskUnavailableForNextDays());

            var didItErorr = false;

            try
            {
                var actual = target.GetNextAvailableWindowSeatDeskDate(new DateTime(2022, 05, 04));
            }
            catch
            {
                didItErorr = true;
            }

            Assert.AreEqual(true, didItErorr);
        }

        #endregion

    }
}


