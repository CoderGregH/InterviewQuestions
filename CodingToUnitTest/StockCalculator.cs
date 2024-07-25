using NUnit.Framework;
using NUnitLite;
using Moq;
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

        public class InventoryRepo : IInventoryRepo
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

            public double GrossTotal(double price, int quantity) => (price * quantity);

            private const double vatRate = 1.2;
            public double NetTotal(double price, int quantity) => this.GrossTotal(price, quantity) * vatRate;

            public double BulkBuyDiscount(int quantity)
            {
                if (quantity < 100)
                    // No discount
                    return 1;

                if (quantity < 1000)
                    // 10 percent
                    return 0.90;

                // A generous 20 percent 
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
                var initialTotal = (calculateWithVat)
                                    ? NetTotal(price, quantity)
                                    : GrossTotal(price, quantity);

                return initialTotal * (StockRunningLowMultipler(productId)) * BulkBuyDiscount(quantity);
            }

            public bool IsStockAvailable(int productId, int quantity)
            {
                var currentStock = _inventoryRepo.GetStock(productId);
                return currentStock >= quantity;
            }
        }

        [TestFixture]
        public class StockCalculatorTests
        {
            private Calculator _calculator;
            private Mock<IInventoryRepo> _mockInventoryRepo;

            [SetUp]
            public void Setup()
            {
                _mockInventoryRepo = new Mock<IInventoryRepo>();
                _calculator = new Calculator(_mockInventoryRepo.Object);
            }

            [Test]
            public void TestCheckBoolean()
            {
                Assert.IsTrue(true);
            }

            [Test]
            public void GrossTotal_ShouldCalculateCorrectly()
            {
                var result = _calculator.GrossTotal(10, 2);
                Assert.AreEqual(20, result);
            }

            [Test]
            public void NetTotal_ShouldCalculateCorrectly()
            {
                var result = _calculator.NetTotal(10, 2);
                Assert.AreEqual(24, result); // 10 * 2 * 1.2
            }

            [Test]
            public void BulkBuyDiscount_ShouldReturnCorrectDiscount()
            {
                Assert.AreEqual(1, _calculator.BulkBuyDiscount(50));
                Assert.AreEqual(0.90, _calculator.BulkBuyDiscount(500));
                Assert.AreEqual(0.8, _calculator.BulkBuyDiscount(1000));
            }

            [Test]
            public void IsStockRunningLow_ShouldReturnTrue_WhenStockIsLow()
            {
                _mockInventoryRepo.Setup(repo => repo.GetStock(It.IsAny<int>())).Returns(5);
                var result = _calculator.IsStockRunningLow(1);
                Assert.IsTrue(result);
            }

            [Test]
            public void IsStockRunningLow_ShouldReturnFalse_WhenStockIsNotLow()
            {
                _mockInventoryRepo.Setup(repo => repo.GetStock(It.IsAny<int>())).Returns(15);
                var result = _calculator.IsStockRunningLow(1);
                Assert.IsFalse(result);
            }

            [Test]
            public void IsStockAvailable_ShouldReturnTrue_WhenStockIsSufficient()
            {
                _mockInventoryRepo.Setup(repo => repo.GetStock(It.IsAny<int>())).Returns(20);
                var result = _calculator.IsStockAvailable(1, 10);
                Assert.IsTrue(result);
            }

            [Test]
            public void IsStockAvailable_ShouldReturnFalse_WhenStockIsInsufficient()
            {
                _mockInventoryRepo.Setup(repo => repo.GetStock(It.IsAny<int>())).Returns(5);
                var result = _calculator.IsStockAvailable(1, 10);
                Assert.IsFalse(result);
            }
        }
    }
}
