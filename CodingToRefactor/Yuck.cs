using NUnit.Framework;
using NUnitLite;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

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
            public float Id { get; set; }
            public bool IsWindow { get; set; }
        }

        public class DeskBooking
        {
            public Desk D { get; set; }
            public DateTime booking { get; set; }
            public string bookedByFirstName { get; set; }
            public string bookedByLastName { get; set; }
        }

        public class DeskFinder
        {
            List<Desk> _desks;
            List<DeskBooking> _bookings;

            public DeskFinder(
                List<Desk> desks,
                List<DeskBooking> bookings)
            {
                _bookings = bookings;
                _desks = desks;
            }

            public DateTime WhenIsNextDeskAvailabeBYdateAndDesk(DateTime start, float Id)
            {
                var currentDate = start;

                if (!_desks.Any(x => x.Id == Id))
                {
                    throw new Exception("This Desk does not exist");
                }

                for (int day = 1; day < 10; day++)
                {

                    var daysBookings = _bookings.Where(x => x.booking.ToShortDateString() == currentDate.ToShortDateString());

                    if (daysBookings.Any())
                    {
                        if (daysBookings.Any(x => x.D.Id == Id) == false)
                        {
                            //there are bookings but not for this desk
                            return currentDate;
                        }
                    }
                    else
                    {
                        //there are not bookings for the date
                        return currentDate;
                    }


                    currentDate = start.AddDays(day);
                }

                return new DateTime();
            }

            public DateTime WhenIsNextWindowSeatAvailabeByDateAndWindow(DateTime start, bool hasWindow = true)
            {
                var currentDate = start;

                if (!_desks.Any(x => x.IsWindow == hasWindow))
                {
                    throw new Exception("No Desks With windows exist");
                }

                var desksWithWindows = _desks.Count(x => x.IsWindow);

                for (int day = 1; day < 10; day++)
                {

                    var daysBookings = _bookings.Where(x => x.booking.ToShortDateString() == currentDate.ToShortDateString());

                    if (daysBookings.Any())
                    {
                        if (daysBookings.Count(x => x.D.IsWindow == hasWindow) < desksWithWindows)
                        {
                            //there are less bookings for desk with windows than there are desk with windows 
                            return currentDate;
                        }
                    }
                    else
                    {
                        //there are not bookings for the date, 
                        //but there are desk with windows 
                        return currentDate;
                    }


                    currentDate = start.AddDays(day);
                }

                return new DateTime();
            }

        }

        [TestFixture]
        public class MyDeskTests
        {
            private Desk desk1 => new Desk()
            {
                Id = 1,
                IsWindow = true
            };

            private Desk desk2 = new Desk()
            {
                Id = 2,
                IsWindow = true
            };

            private Desk desk3 = new Desk()
            {
                Id = 3,
                IsWindow = false
            };

            private List<Desk> desks => new List<Desk>() { desk1, desk2, desk3 };


            internal List<DeskBooking> GetSample()
            {


                var booking0 = new DeskBooking()
                {
                    D = desk1,
                    booking = new DateTime(2022, 05, 04),
                    bookedByFirstName = "Bob",
                    bookedByLastName = "smith"
                };

                var booking1 = new DeskBooking()
                {
                    D = desk1,
                    booking = new DateTime(2022, 05, 05),
                    bookedByFirstName = "Bob",
                    bookedByLastName = "smith"
                };

                var booking2 = new DeskBooking()
                {
                    D = desk1,
                    booking = new DateTime(2022, 05, 06),
                    bookedByFirstName = "Bob",
                    bookedByLastName = "smith"
                };

                var booking3 = new DeskBooking()
                {
                    D = desk1,
                    booking = new DateTime(2022, 05, 07),
                    bookedByFirstName = "Bob",
                    bookedByLastName = "smith"
                };

                var booking4 = new DeskBooking()
                {
                    D = desk2,
                    booking = new DateTime(2022, 05, 04),
                    bookedByFirstName = "Paul",
                    bookedByLastName = "Brown"
                };

                var booking5 = new DeskBooking()
                {
                    D = desk2,
                    booking = new DateTime(2022, 05, 05),
                    bookedByFirstName = "Paul",
                    bookedByLastName = "Brown"
                };

                var booking6 = new DeskBooking()
                {
                    D = desk2,
                    booking = new DateTime(2022, 05, 07),
                    bookedByFirstName = "Paul",
                    bookedByLastName = "Brown"
                };

                return new List<DeskBooking>()
                {
                    booking0,
                    booking1,
                    booking2,
                    booking3,
                    booking4,
                    booking5
                }
                ;
            }

            #region "Desk by Id Check"


            [Test]
            public void Should_Find_Next_Available_Date_Busy_On_Day_One()
            {
                var target = new DeskFinder(desks, GetSample());
                var actual = target.WhenIsNextDeskAvailabeBYdateAndDesk(new DateTime(2022, 05, 07), 1);

                //Should be free on the 8th as booked on the 7th
                Assert.AreEqual(new DateTime(2022, 05, 08), actual);
            }

            [Test]
            public void Should_Find_Next_Available_Date_Not_Busy_On_Day_One()
            {
                var target = new DeskFinder(desks, GetSample());
                var actual = target.WhenIsNextDeskAvailabeBYdateAndDesk(new DateTime(2022, 05, 07), 3);

                //Desk 3 is always free as never booked
                Assert.AreEqual(new DateTime(2022, 05, 07), actual);
            }

            [Test]
            public void Should_Find_Error_If_Desk_Does_Not_Exist()
            {
                var target = new DeskFinder(desks, GetSample());

                var didItErorr = false;

                try
                {
                    var actual = target.WhenIsNextDeskAvailabeBYdateAndDesk(new DateTime(2022, 05, 07), 4);
                }
                catch
                {
                    didItErorr = true;
                }

                //Desk 4 is not in the sample data
                Assert.AreEqual(true, didItErorr);
            }

            #endregion

            #region "Desk with Window"

            [Test]
            public void Should_Find_Next_Available_Date_with_A_Window_Busy_On_Day_One_and_Two()
            {
                var target = new DeskFinder(desks, GetSample());
                var actual = target.WhenIsNextWindowSeatAvailabeByDateAndWindow(new DateTime(2022, 05, 04), true);

                //Should be free on the 6th as desk 1 and 2 booked on the 4th and 5th
                Assert.AreEqual(new DateTime(2022, 05, 06), actual);
            }

            [Test]
            public void Should_Find_Next_Available_Date_with_A_Window_Free_On_Day_One()
            {
                var target = new DeskFinder(desks, GetSample());
                var actual = target.WhenIsNextWindowSeatAvailabeByDateAndWindow(new DateTime(2022, 05, 03), true);

                //Should be free on the 6th as desk 1 and 2 booked on the 4th and 6th
                Assert.AreEqual(new DateTime(2022, 05, 03), actual);
            }

            #endregion




        }
    }
}
