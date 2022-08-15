using NUnit.Framework;
using NUnitLite;
using System;
using System.Reflection;


namespace CodepadTestSample
{
    class Program
    {
        static int Main(string[] args)
        {
            return new AutoRun(Assembly.GetCallingAssembly()).Execute(new String[] { "--labels=All" });
        }

        public interface IInventoryRepo
        {
            int GetStock(int productId);

        }

        public class InventoryRepo
        {
            public int GetStock(int productId)
            {
                //This would actually call something like a DB to get the actual stock level
                return new Random().Next(0, 120);
            }
        }

        public class Calculator
        {
            private readonly IInventoryRepo _inventoryRepo;
            public Calculator(IInventoryRepo inventoryRepo)
            {
                this._inventoryRepo = inventoryRepo;
            }

            public double GrossTotal(double price, int quanity) => (price * quanity);

            private const double vatRate = 1.2;
            public double NetTotal(double price, int quanity) => this.GrossTotal(price, quanity) * vatRate;

            public double BulkBuyDiscount(int quanity)
            {

                if (quanity < 100)
                    //No discount
                    return 1;

                if (quanity < 1000)
                    //10 percent
                    return 0.90;

                //a generous 20 percent 
                return 0.8;

            }


            public bool IsStockRunningLow(int productId)
            {
                var currentStock = _inventoryRepo.GetStock(productId);
                return (currentStock < 10);
            }

            public double StockRunningLowMultipler(int productId)
            {
                if (IsStockRunningLow(productId))
                {
                    //add five percent if stock is running low
                    return 1.05;
                }
                return 1;
            }

            public double FinalTotal(int productId, double price, int quanity, bool calculateWithVat)
            {
                var intialTotal = (calculateWithVat)
                                    ? NetTotal(price, quanity)
                                    : GrossTotal(price, quanity);

                return intialTotal * (StockRunningLowMultipler(quanity)) * BulkBuyDiscount(quanity);
            }

            public bool IsStockAvailable(int productId, int quanity)
            {
                throw new NotImplementedException();
            }

        }


        [TestFixture]
        public class StockCalculatorTests
        {
            [Test]
            public void TestCheckBoolean()
            {
                Assert.IsTrue(true);
            }

            /*
                Tests for GrossTotal, NetTotal and or BulkBuyDiscount
            */


            /*
                Tests for IsStockRunningLow
            */

            /*
                Write implemenation and test IsStockAvailable
            */


        }
    }
}
