using NUnit.Framework;
using NUnitLite;
using System;
using System.Reflection;
using Moq;


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
                if (productId <= 0)
                {
                    throw new ArgumentException("Invalid product ID");
                }

                // This would actually call something like a DB to get the actual stock level
                return new Random().Next(0, 120);
            }
        }

        public class Calculator
        {
            private readonly IInventoryRepo _inventoryRepo;
            private const double vatRate = 1.2;
            public Calculator(IInventoryRepo inventoryRepo)
            {
                this._inventoryRepo = inventoryRepo;
            }

            public double GrossTotal(double price, int quantity)
            {
                if (price <= 0) throw new ArgumentException("Price cannot be 0 or negative");
                if (quantity <= 0) throw new ArgumentException("Quantity cannot be 0 or negative");
                return price * quantity;
            }

            public double NetTotal(double price, int quantity)
            {
                if (price <= 0) throw new ArgumentException("Price cannot be 0 or negative");
                if (quantity <= 0) throw new ArgumentException("Quantity cannot be 0 or negative");
                return this.GrossTotal(price, quantity) * vatRate;
            }

           public double BulkBuyDiscount(int quantity)
            {
                if (quantity <= 0) throw new ArgumentException("Quantity cannot be 0 or negative");

                if (quantity < 100)
                    return 1; // No discount

                if (quantity < 1000)
                    return 0.90; // 10% discount

                return 0.80; // 20% discount
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
                    //Add five percent if stock is running low
                    return 1.05;
                }
                return 1;
            }

            public double FinalTotal(int productId, double price, int quantity, bool calculateWithVat)
            {
                var intialTotal = (calculateWithVat)
                                    ? NetTotal(price, quantity)
                                    : GrossTotal(price, quantity);

                return intialTotal * (StockRunningLowMultipler(quantity)) * BulkBuyDiscount(quantity);
            }

            public bool IsStockAvailable(int productId, int quantity)
            {
                if (quantity <= 0)
                {
                    throw new ArgumentException("Quantity cannot be 0 or negative");
                }

                var currentStock = _inventoryRepo.GetStock(productId);
                return currentStock >= quantity;
            }
        }


        [TestFixture]
        public class StockCalculatorTests
        {
            [SetUp]
            public void SetUp()
            {
                _mockRepo = new Mock<IInventoryRepo>();
                _calculator = new Calculator(_mockRepo.Object);
            }

            [Test]
            public void TestGrossTotal_ValidInput_ShouldReturnCorrectValue()
            {
                double price = 10.0;
                int quantity = 5;
                double expectedTotal = price * quantity;

                double result = _calculator.GrossTotal(price, quantity);

                Assert.AreEqual(expectedTotal, result, "GrossTotal calculation is incorrect.");
            }

            [Test]
            public void TestGrossTotal_PriceZero_ShouldThrowException()
            {
                double price = 0;
                int quantity = 5;

                var ex = Assert.Throws<ArgumentException>(() => _calculator.GrossTotal(price, quantity));
                Assert.That(ex.Message, Is.EqualTo("Price cannot be 0 or negative"));
            }

            [Test]
            public void TestGrossTotal_PriceNegative_ShouldThrowException()
            {
                double price = -10.0;
                int quantity = 5;

                var ex = Assert.Throws<ArgumentException>(() => _calculator.GrossTotal(price, quantity));
                Assert.That(ex.Message, Is.EqualTo("Price cannot be 0 or negative"));
            }

            [Test]
            public void TestGrossTotal_QuantityZero_ShouldThrowException()
            {
                double price = 10.0;
                int quantity = 0;

                var ex = Assert.Throws<ArgumentException>(() => _calculator.GrossTotal(price, quantity));
                Assert.That(ex.Message, Is.EqualTo("Quantity cannot be 0 or negative"));
            }

            [Test]
            public void TestGrossTotal_QuantityNegative_ShouldThrowException()
            {
                double price = 10.0;
                int quantity = -5;

                var ex = Assert.Throws<ArgumentException>(() => _calculator.GrossTotal(price, quantity));

                Assert.That(ex.Message, Is.EqualTo("Quantity cannot be 0 or negative"));
            }

            [Test]
            public void TestNetTotal_ValidInput_ShouldReturnCorrectValue()
            {
                double price = 10.0;      
                int quantity = 5;         
                double expectedNetTotal = 60.0;  

                double result = _calculator.NetTotal(price, quantity);

                Assert.AreEqual(expectedNetTotal, result, "NetTotal calculation is incorrect.");
            }

            [Test]
            public void TestNetTotal_PriceZero_ShouldThrowException()
            {
                double price = 0;
                int quantity = 5;

                var ex = Assert.Throws<ArgumentException>(() => _calculator.NetTotal(price, quantity));

                Assert.That(ex.Message, Is.EqualTo("Price cannot be 0 or negative"));
            }

            [Test]
            public void TestNetTotal_PriceNegative_ShouldThrowException()
            {
                double price = -10.0;
                int quantity = 5;

                var ex = Assert.Throws<ArgumentException>(() => _calculator.NetTotal(price, quantity));

                Assert.That(ex.Message, Is.EqualTo("Price cannot be 0 or negative"));
            }

            [Test]
            public void TestNetTotal_QuantityZero_ShouldThrowException()
            {
                double price = 10.0;
                int quantity = 0;

                var ex = Assert.Throws<ArgumentException>(() => _calculator.NetTotal(price, quantity));

                Assert.That(ex.Message, Is.EqualTo("Quantity cannot be 0 or negative"));
            }

            [Test]
            public void TestNetTotal_QuantityNegative_ShouldThrowException()
            {
                double price = 10.0;
                int quantity = -5;

                var ex = Assert.Throws<ArgumentException>(() => _calculator.NetTotal(price, quantity));

                Assert.That(ex.Message, Is.EqualTo("Quantity cannot be 0 negative"));
            }

            [Test]
            public void TestBulkBuyDiscount_ValidInput_ShouldReturnCorrectDiscount()
            {
                int quantityNoDiscount = 10;        // Quantity with no discount
                int quantityTenPercent = 500;       // Quantity for a 10% discount
                int quantityTwentyPercent = 1500;   // Quantity for a 20% discount
                
                double expectedDiscountNoDiscount = 1.0;    // No discount
                double expectedDiscountTenPercent = 0.90;   // 10% discount
                double expectedDiscountTwentyPercent = 0.80; // 20% discount

                double resultNoDiscount = _calculator.BulkBuyDiscount(quantityNoDiscount);
                double resultTenPercent = _calculator.BulkBuyDiscount(quantityTenPercent);
                double resultTwentyPercent = _calculator.BulkBuyDiscount(quantityTwentyPercent);

                Assert.AreEqual(expectedDiscountNoDiscount, resultNoDiscount, "BulkBuyDiscount No Discount calculation is incorrect.");
                Assert.AreEqual(expectedDiscountTenPercent, resultTenPercent, "BulkBuyDiscount 10% calculation is incorrect.");
                Assert.AreEqual(expectedDiscountTwentyPercent, resultTwentyPercent, "BulkBuyDiscount 20% calculation is incorrect.");
}

            [Test]
            public void TestBulkBuyDiscount_QuantityZero_ShouldThrowException()
            {
                int quantity = 0;          // Zero quantity
                
                var ex = Assert.Throws<ArgumentException>(() => _calculator.BulkBuyDiscount(quantity));

                Assert.That(ex.Message, Is.EqualTo("Quantity cannot be 0 or negative"));
            }

            [Test]
            public void TestBulkBuyDiscount_QuantityNegative_ShouldThrowException()
            {
                int quantity = -10;        

                var ex = Assert.Throws<ArgumentException>(() => _calculator.BulkBuyDiscount(quantity));

                Assert.That(ex.Message, Is.EqualTo("Quantity cannot be 0 or negative"));
            }

            [Test]
            public void TestIsStockRunningLow_StockBelowThreshold_ShouldReturnTrue()
            {
                int productId = 1;          // Example product ID
                int lowStock = 5;           // Stock level below the threshold
                var mockRepo = new Mock<IInventoryRepo>();
                mockRepo.Setup(repo => repo.GetStock(productId)).Returns(lowStock);
                var calculator = new Calculator(mockRepo.Object);

                // Act
                bool result = calculator.IsStockRunningLow(productId);

                // Assert
                Assert.IsTrue(result, "IsStockRunningLow should return true when stock is below the threshold.");
            }

            [Test]
            public void TestIsStockRunningLow_StockAboveThreshold_ShouldReturnFalse()
            {
                // Arrange
                int productId = 2;          // Example product ID
                int sufficientStock = 15;  // Stock level above the threshold
                var mockRepo = new Mock<IInventoryRepo>();
                mockRepo.Setup(repo => repo.GetStock(productId)).Returns(sufficientStock);
                var calculator = new Calculator(mockRepo.Object);

                // Act
                bool result = calculator.IsStockRunningLow(productId);

                // Assert
                Assert.IsFalse(result, "IsStockRunningLow should return false when stock is above the threshold.");
            }

            [Test]
            public void TestIsStockRunningLow_InvalidProductID_ShouldThrowException()
            {
                // Arrange
                int productId = 0;          
                var mockRepo = new Mock<IInventoryRepo>();
                mockRepo.Setup(repo => repo.GetStock(invalidProductId)).Throws(new ArgumentException("Invalid product ID"));
                var calculator = new Calculator(mockRepo.Object);

                Assert.Throws<ArgumentException>(() => calculator.IsStockRunningLow(invalidProductId), "Expected Exception for invalid product ID.");
            }

            [Test]
            public void TestIsStockAvailable_StockAvailable_ShouldReturnTrue()
            {
                // Arrange
                int productId = 1;          // Example product ID
                int quantityRequired = 5;  // Quantity needed
                int currentStock = 10;      // Stock level sufficient to meet the requirement
                var mockRepo = new Mock<IInventoryRepo>();
                mockRepo.Setup(repo => repo.GetStock(productId)).Returns(currentStock);
                var calculator = new Calculator(mockRepo.Object);

                // Act
                bool result = calculator.IsStockAvailable(productId, quantityRequired);

                // Assert
                Assert.IsTrue(result, "IsStockAvailable should return true when stock is sufficient.");
            }

            [Test]
            public void TestIsStockAvailable_StockNotAvailable_ShouldReturnFalse()
            {
                // Arrange
                int productId = 2;          // Example product ID
                int quantityRequired = 15; // Quantity needed
                int currentStock = 10;     // Stock level insufficient to meet the requirement
                var mockRepo = new Mock<IInventoryRepo>();
                mockRepo.Setup(repo => repo.GetStock(productId)).Returns(currentStock);
                var calculator = new Calculator(mockRepo.Object);

                // Act
                bool result = calculator.IsStockAvailable(productId, quantityRequired);

                // Assert
                Assert.IsFalse(result, "IsStockAvailable should return false when stock is insufficient.");
            }

            [Test]
            public void TestIsStockAvailable_NegativeQuantity_ShouldThrowArgumentException()
            {
                int productId = 3;          // Example product ID
                int negativeQuantity = -5; // Invalid quantity
                var mockRepo = new Mock<IInventoryRepo>();
                var calculator = new Calculator(mockRepo.Object);

                var exception = Assert.Throws<ArgumentException>(() => {
                    _calculator.IsStockAvailable(productId, negativeQuantity); // Example quantity
                });

                Assert.AreEqual("Quantity cannot be 0 or negative", exception.Message);
            }

            [Test]
            public void TestIsStockAvailable_InvalidProductId_ShouldThrowException()
            {
                int invalidProductId = -1; // Example of an invalid product ID

                _mockInventoryRepo.Setup(repo => repo.GetStock(invalidProductId))
                                .Throws(new ArgumentException("Invalid product ID"));

                var exception = Assert.Throws<ArgumentException>(() =>
                {
                    _calculator.IsStockAvailable(invalidProductId, 10); // Example quantity
                });

                Assert.AreEqual("Invalid product ID", exception.Message);
            }

        }
    }
}
