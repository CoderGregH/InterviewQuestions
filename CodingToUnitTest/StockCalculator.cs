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

        public class FakeInventoryRepo : IInventoryRepo
        {
            private readonly int _stock;
            public FakeInventoryRepo(int stock)
            {
                _stock = stock;
            }
            public int GetStock(int productId)
            {
                return _stock;
            }
        }

        public class Calculator
        {
            private readonly IInventoryRepo _inventoryRepo;
            public Calculator(IInventoryRepo inventoryRepo)
            {
                this._inventoryRepo = inventoryRepo;
            }

            public double GrossTotal(double price, int quantity) => (price * quantity);

            private const double vatRate = 1.2;
            public double NetTotal(double price, int quantity) => this.GrossTotal(price, quantity) * vatRate;

            public double BulkBuyDiscount(int quantity)
            {

                if (quantity < 100)
                    //No discount
                    return 1;

                if (quantity < 1000)
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

            public double FinalTotal(int productId, double price, int quantity, bool calculateWithVat)
            {
                var intialTotal = (calculateWithVat)
                                    ? NetTotal(price, quantity)
                                    : GrossTotal(price, quantity);

                return intialTotal * (StockRunningLowMultipler(productId)) * BulkBuyDiscount(quantity);
            }

            public bool IsStockAvailable(int productId, int quantity)
            {
                var currentStock = _inventoryRepo.GetStock(productId);
                return (currentStock >= quantity);
            }

        }


        [TestFixture]
        public class StockCalculatorTests
        {
            private Calculator _cal;
            private IInventoryRepo _fakeInventoryRepo;


            /*
                Tests for GrossTotal, NetTotal and or BulkBuyDiscount
            */

            [Test]
            public void TestGrossTotal()
            {
                _fakeInventoryRepo = new FakeInventoryRepo(1);
                _cal = new Calculator(_fakeInventoryRepo);
                var actual = _cal.GrossTotal(100, 10);
                Assert.AreEqual(1000, actual);
            }

            [Test]
            public void TestNetTotal()
            {
                _fakeInventoryRepo = new FakeInventoryRepo(1);
                _cal = new Calculator(_fakeInventoryRepo);
                var actual = _cal.NetTotal(100, 10);
                Assert.AreEqual(1200, actual);
            }

            [Test]
            public void TestBulkBuyDiscount()
            {
                _fakeInventoryRepo = new FakeInventoryRepo(1);
                _cal = new Calculator(_fakeInventoryRepo);
                Assert.AreEqual(1, _cal.BulkBuyDiscount(50));
                Assert.AreEqual(0.8, _cal.BulkBuyDiscount(2000));
                Assert.AreEqual(0.90, _cal.BulkBuyDiscount(150));
            }


            /*
                Tests for IsStockRunningLow
            */
            [Test]
            public void IsStockRunningLow()
            {
                _fakeInventoryRepo = new FakeInventoryRepo(50);//stock
                _cal = new Calculator(_fakeInventoryRepo);
                Assert.AreEqual(false, _cal.IsStockRunningLow(5));//any productId

                _fakeInventoryRepo = new FakeInventoryRepo(9);
                _cal = new Calculator(_fakeInventoryRepo);
                Assert.AreEqual(true, _cal.IsStockRunningLow(4));
            }


            [Test]
            public void TestStockRunningLowMultipler()
            {
                _fakeInventoryRepo = new FakeInventoryRepo(9);//stock
                _cal = new Calculator(_fakeInventoryRepo);
                Assert.AreEqual(1.05, _cal.StockRunningLowMultipler(4)); //any productId

                _fakeInventoryRepo = new FakeInventoryRepo(50);
                _cal = new Calculator(_fakeInventoryRepo);
                Assert.AreEqual(1, _cal.StockRunningLowMultipler(5));
            }

            /*
                 test for FinalTotal
            */
            [Test]
            public void TestFinalTotal()
            {
                //stock is available,vat is true
                _fakeInventoryRepo = new FakeInventoryRepo(10);//stock
                _cal = new Calculator(_fakeInventoryRepo);
                Assert.AreEqual(1200, _cal.FinalTotal(5, 100, 10, true)); //any productId

                //stock is available, vat is false
                _fakeInventoryRepo = new FakeInventoryRepo(10);
                _cal = new Calculator(_fakeInventoryRepo);
                Assert.AreEqual(1000, _cal.FinalTotal(5, 100, 10, false)); //any productId

                //stock is less, vat is true
                _fakeInventoryRepo = new FakeInventoryRepo(9);
                _cal = new Calculator(_fakeInventoryRepo);
                Assert.AreEqual(1260, _cal.FinalTotal(5, 100, 10, true)); //any productId 1200x1.05x

                //stock is less, vat is false
                _fakeInventoryRepo = new FakeInventoryRepo(9);
                _cal = new Calculator(_fakeInventoryRepo);
                Assert.AreEqual(1050, _cal.FinalTotal(5, 100, 10, false)); //any productId 1000x1.05x
            }

            /*
                Write implemenation and test IsStockAvailable
            */
            [Test]
            public void IsStockAvailable()
            {
                _fakeInventoryRepo = new FakeInventoryRepo(50);//stock
                _cal = new Calculator(_fakeInventoryRepo);
                Assert.AreEqual(true, _cal.IsStockAvailable(5, 20));//any productId,quantity

                _fakeInventoryRepo = new FakeInventoryRepo(50);
                _cal = new Calculator(_fakeInventoryRepo);
                Assert.AreEqual(false, _cal.IsStockAvailable(5, 100));
            }
        }
    }
}
