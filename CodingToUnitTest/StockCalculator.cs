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

            public double GrossTotal(double price, int quantity)
            {
                if (price < 0 || quantity < 0)
                    return 0;
                return price * quantity;
            }

            private const double vatRate = 1.2;
            public double NetTotal(double price, int quantity)
            {
                if (price < 0 || quantity < 0 || vatRate < 0)
                    return 0;
                return GrossTotal(price, quantity) * vatRate;
            }

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

            public bool IsStockAvailable(int productId, int quantity)
            {
                // Get the current stock level from the repository
                var currentStock = _inventoryRepo.GetStock(productId);

                // Check if the available stock is sufficient for the requested quantity
                return currentStock >= quantity;
            }

        }

        public class TestInventoryRepo : IInventoryRepo
        {
            private readonly int _stock;

            public TestInventoryRepo(int stock)
            {
                _stock = stock;
            }

            public int GetStock(int productId)
            {
                return _stock;
            }
        }


        [TestFixture]
        public class StockCalculatorTests
        {

            [Theory]
            [TestCase(-10, 5)]
            [TestCase(10, -5)]
            [TestCase(-10, -5)]
            public void GrossTotal_NegativeValues_ReturnsZero(double price, int quantity)
            {
                // Arrange
                var inventoryRepo = new TestInventoryRepo(50);
                var calculator = new Calculator(inventoryRepo);

                // Act
                var result = calculator.GrossTotal(price, quantity);

                // Assert
                Assert.That(0, Is.EqualTo(result));
            }

            [Theory]
            [TestCase(15, 3, 45)]
            [TestCase(10, 5, 50)]
            public void GrossTotal_ValidValues_ReturnsCorrectTotal(double price, int quantity, double expected)
            {
                // Arrange
                var inventoryRepo = new TestInventoryRepo(50);
                var calculator = new Calculator(inventoryRepo);

                // Act
                var result = calculator.GrossTotal(price, quantity);

                // Assert
                Assert.That(expected, Is.EqualTo(result));
            }            

            [TestCase(-10, 5, 0)]
            [TestCase(10, -5, 0)]
            [TestCase(-10, -5, 0)]
            public void NetTotal_NegativeValues_ReturnsZero(double price, int quantity, int expected)
            {
                // Arrange
                var inventoryRepo = new TestInventoryRepo(5);
                var calculator = new Calculator(inventoryRepo);

                // Act
                var result = calculator.NetTotal(price, quantity);

                // Assert
                Assert.That(expected, Is.EqualTo(result));
            }

            [TestCase(10, 5, 60)] // Example with positive values
            [TestCase(15, 3, 54)] // Example with positive values
            public void NetTotal_ValidValues_ReturnsCorrectTotal(double price, int quantity, int expected)
            {
                // Arrange
                var inventoryRepo = new TestInventoryRepo(5);
                var calculator = new Calculator(inventoryRepo);

                // Act
                var result = calculator.NetTotal(price, quantity);

                // Assert
                Assert.That(expected, Is.EqualTo(result));
            }

            [Test]
            public void TestBulkBuyDiscount_NoDiscount()
            {
                var inventoryRepo = new TestInventoryRepo(50);
                var calculator = new Calculator(inventoryRepo);

                var result = calculator.BulkBuyDiscount(50);
                Assert.That(result, Is.EqualTo(1.0));  // Expected result: 1.0 (No discount)
            }

            [Test]
            public void TestBulkBuyDiscount_TenPercentDiscount()
            {
                var inventoryRepo = new TestInventoryRepo(50);
                var calculator = new Calculator(inventoryRepo);

                var result = calculator.BulkBuyDiscount(150);
                Assert.That(result, Is.EqualTo(0.90));  // Expected result: 0.90 (10% discount)
            }

            [Test]
            public void TestBulkBuyDiscount_TwentyPercentDiscount()
            {
                var inventoryRepo = new TestInventoryRepo(50);
                var calculator = new Calculator(inventoryRepo);

                var result = calculator.BulkBuyDiscount(1200);
                Assert.That(result, Is.EqualTo(0.80));  // Expected result: 0.80 (20% discount)
            }

            [Test]
            public void TestIsStockRunningLow_StockIsLow()
            {
                // Arrange
                var inventoryRepo = new TestInventoryRepo(5);
                var calculator = new Calculator(inventoryRepo);

                // Act
                var result = calculator.IsStockRunningLow(1);

                // Assert
                Assert.That(result, Is.True);
            }

            [Test]
            public void TestIsStockRunningLow_StockIsNotLow()
            {
                // Arrange
                var inventoryRepo = new TestInventoryRepo(15);
                var calculator = new Calculator(inventoryRepo);

                // Act
                var result = calculator.IsStockRunningLow(1);

                // Assert
                Assert.That(result, Is.False);
            }

            [Test]
            public void TestStockRunningLowMultipler_StockIsLow()
            {
                // Arrange
                var inventoryRepo = new TestInventoryRepo(5); // Set low stock
                var calculator = new Calculator(inventoryRepo);

                // Act
                var result = calculator.StockRunningLowMultipler(1);

                // Assert
                Assert.That(result, Is.EqualTo(1.05)); // Expecting 5% increase
            }

            [Test]
            public void TestStockRunningLowMultipler_StockIsNotLow()
            {
                // Arrange
                var inventoryRepo = new TestInventoryRepo(20); // Set sufficient stock
                var calculator = new Calculator(inventoryRepo);

                // Act
                var result = calculator.StockRunningLowMultipler(1);

                // Assert
                Assert.That(result, Is.EqualTo(1.0)); // Expecting no increase
            }

            [Test]
            public void TestFinalTotal_WithVatAndLowStock()
            {
                // Arrange
                var inventoryRepo = new TestInventoryRepo(5); // Low stock
                var calculator = new Calculator(inventoryRepo);

                // Act
                var result = calculator.FinalTotal(1, 100.0, 50, calculateWithVat: true);

                // Assert
                var expected = 100.0 * 50 * 1.2 * 1.05 * 1.0; // NetTotal * StockRunningLowMultiplier * No discount
                Assert.That(result, Is.EqualTo(expected));
            }

            [Test]
            public void TestFinalTotal_WithoutVatAndNoLowStock()
            {
                // Arrange
                var inventoryRepo = new TestInventoryRepo(50); // Sufficient stock
                var calculator = new Calculator(inventoryRepo);

                // Act
                var result = calculator.FinalTotal(1, 100.0, 50, calculateWithVat: false);

                // Assert
                var expected = 100.0 * 50 * 1.0 * 1.0; // GrossTotal * No stock multiplier * No discount
                Assert.That(result, Is.EqualTo(expected));
            }

            [Test]
            public void TestIsStockAvailable_StockSufficient()
            {
                // Arrange
                var inventoryRepo = new TestInventoryRepo(20); // Sufficient stock
                var calculator = new Calculator(inventoryRepo);

                // Act
                var result = calculator.IsStockAvailable(1, 15); // Request 15 units

                // Assert
                Assert.That(result, Is.True); // Stock is sufficient
            }

            [Test]
            public void TestIsStockAvailable_StockInsufficient()
            {
                // Arrange
                var inventoryRepo = new TestInventoryRepo(10); // Limited stock
                var calculator = new Calculator(inventoryRepo);

                // Act
                var result = calculator.IsStockAvailable(1, 15); // Request 15 units

                // Assert
                Assert.That(result, Is.False); // Stock is insufficient
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
