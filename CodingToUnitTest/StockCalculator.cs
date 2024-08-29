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
                //Fix - StockRunningLowMultipler expecting productId but here we were passing quantity
                return intialTotal * (StockRunningLowMultipler(productId)) * BulkBuyDiscount(quanity);
            }

            public bool IsStockAvailable(int productId, int quanity)
            {
                throw new NotImplementedException();
            }

        }


        //Mock InventoryRepo
        public class InventoryRepoMock : IInventoryRepo
        {
            public int GetStock(int productId)
            {
                if (productId >= 1 && productId <= 10)
                    return 9;
                else if (productId >= 11 && productId <= 20)
                    return 15;

                return 0;
            }
        }

        [TestFixture]
        public class StockCalculatorTests
        {
            private IInventoryRepo _inventoryRepoMock;
            private Calculator _calculator;

            public StockCalculatorTests()
            {
                this._inventoryRepoMock = new InventoryRepoMock();
                _calculator = new Calculator(_inventoryRepoMock);
            }

            //[Setup]
            //public void Setup()
            //{
            //    this._inventoryRepoMock = new InventoryRepoMock();
            //    _calculator = new Calculator(_inventoryRepoMock);
            //}

            [Test]
            public void TestCheckBoolean()
            {
                Assert.IsTrue(true);
            }

            /*
                Tests for GrossTotal, NetTotal and or BulkBuyDiscount
            */
            [Test]
            public void GrossTotal_ShouldReturnExpectedResult()
            {
                //Arrange
                double price = 10;
                int quantity = 10;
                double ExpectedResult = 100;

                //Act
                double ActualResult = _calculator.GrossTotal(price, quantity);

                //Assert
                Assert.AreEqual(ActualResult, ExpectedResult);
            }

            [Test]
            [TestCase(10, 1, ExpectedResult = 12)]
            [TestCase(10, 0, ExpectedResult = 0)]
            public double NetTotal_ShouldReturnExpectedResult(double price, int quantity)
            {
                //Arrange
                //const double vatRate = 1.2;
                //double ExpectedResult = (price*quantity)*vatRate;

                //Act and Assert
                return _calculator.NetTotal(price, quantity);

            }

            [Test]
            [TestCase(99, ExpectedResult = 1)]
            [TestCase(100, ExpectedResult = 0.90)]
            [TestCase(110, ExpectedResult = 0.90)]
            [TestCase(999, ExpectedResult = 0.90)]
            [TestCase(1000, ExpectedResult = 0.8)]
            [TestCase(1010, ExpectedResult = 0.8)]
            public double BulkBuyDiscount_ShouldReturnExpectedResult(int quantity)
            {
                return _calculator.BulkBuyDiscount(quantity);
            }
            /*
                Tests for IsStockRunningLow
            */

            [Test]
            [TestCase(1, ExpectedResult = true)]
            [TestCase(9, ExpectedResult = true)]
            [TestCase(12, ExpectedResult = false)]
            public bool IsStockRunningLow_ShouldReturnExpectedResult(int productId)
            {
                return _calculator.IsStockRunningLow(productId);
            }

            /*
                Write implemenation and test IsStockAvailable
            */
            [Test]
            public void IsStockAvailable_ShouldThrowNotImplementedException()
            {
                //Arrange
                int productId = 1;
                int quantity = 1;

                Assert.Throws<NotImplementedException>(() => _calculator.IsStockAvailable(productId, quantity));
            }

            /*
                Tests for FinalTotal
            */

            [Test]
            public void FinalTotal_ShouldCalculateWithoutVat_WhenStockIsRunningLow()
            {
                //Arrange
                int productId = 1;
                double price = 10;
                int quantity = 1;
                bool calculateWithVat = false;
                double ExpectedResult = _calculator.GrossTotal(price, quantity) *
                                        (_calculator.StockRunningLowMultipler(productId) *
                                        _calculator.BulkBuyDiscount(quantity));

                //Act
                double actualResult = _calculator.FinalTotal(productId, price, quantity, calculateWithVat);

                //Assert
                Assert.AreEqual(ExpectedResult, actualResult);
            }

            [Test]
            public void FinalTotal_ShouldCalculateWithVat_WhenStockIsRunningLow()
            {
                //Arrange
                int productId = 1;
                double price = 10;
                int quantity = 1;
                bool calculateWithVat = true;
                double ExpectedResult = _calculator.NetTotal(price, quantity) *
                                        (_calculator.StockRunningLowMultipler(productId) *
                                        _calculator.BulkBuyDiscount(quantity));

                //Act
                double actualResult = _calculator.FinalTotal(productId, price, quantity, calculateWithVat);

                //Assert
                Assert.AreEqual(ExpectedResult, actualResult);
            }

            [Test]
            public void FinalTotal_ShouldCalculateWithoutVat_WhenStockIsNotRunningLow()
            {
                //Arrange
                int productId = 15;
                double price = 10;
                int quantity = 1;
                bool calculateWithVat = false;
                double ExpectedResult = _calculator.GrossTotal(price, quantity) *
                                        (_calculator.StockRunningLowMultipler(productId) *
                                        _calculator.BulkBuyDiscount(quantity));

                //Act
                double actualResult = _calculator.FinalTotal(productId, price, quantity, calculateWithVat);

                //Assert
                Assert.AreEqual(ExpectedResult, actualResult);
            }

            [Test]
            public void FinalTotal_ShouldCalculateWithVat_WhenStockIsNotRunningLow()
            {
                //Arrange
                int productId = 15;
                double price = 10;
                int quantity = 1;
                bool calculateWithVat = true;
                double ExpectedResult = _calculator.NetTotal(price, quantity) *
                                        (_calculator.StockRunningLowMultipler(productId) *
                                        _calculator.BulkBuyDiscount(quantity));

                //Act
                double actualResult = _calculator.FinalTotal(productId, price, quantity, calculateWithVat);

                //Assert
                Assert.AreEqual(ExpectedResult, actualResult);
            }

            [Test]
            public void FinalTotal_ShouldCalculateWithZeroQuantity_ReturnZero()
            {
                //Arrange
                int productId = 15;
                double price = 10;
                int quantity = 0;
                bool calculateWithVat = true;
                double ExpectedResult = 0;

                //Act
                double actualResult = _calculator.FinalTotal(productId, price, quantity, calculateWithVat);

                //Assert
                Assert.AreEqual(ExpectedResult, actualResult);
            }

            [Test]
            public void FinalTotal_ShouldCalculateWithNegativeQuantity_ReturnNegativeValue()
            {
                //Arrange
                int productId = 15;
                double price = 10;
                int quantity = -1;
                bool calculateWithVat = true;

                //Act
                double actualResult = _calculator.FinalTotal(productId, price, quantity, calculateWithVat);

                //Assert
                Assert.That(actualResult, Is.LessThan(0));
            }
        }
    }
}