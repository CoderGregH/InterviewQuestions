const _ = require("lodash");

class InventoryRepo {
  GetStock(productId) {
    //This would actually call something like a DB to get the actual stock level
    return Math.random() * 120;
  }
}

class Calculator {
  _inventoryRepo;
  constructor(inventoryRepo) {
    this._inventoryRepo = inventoryRepo;
  }

  GrossTotal(price, quanity) {
    return price * quanity;
  }

  vatRate = 1.2;

  NetTotal(price, quanity) {
    return this.GrossTotal(price, quanity) * this.vatRate;
  }

  BulkBuyDiscount(quanity) {
    if (quanity < 100)
      //No discount
      return 1;

    if (quanity < 1000)
      //10 percent
      return 0.9;

    //a generous 20 percent
    return 0.8;
  }

  IsStockRunningLow(productId) {
    var currentStock = this._inventoryRepo.GetStock(productId);
    return currentStock < 10;
  }

  StockRunningLowMultipler(productId) {
    if (this.IsStockRunningLow(productId)) {
      //add five percent if stock is running low
      return 1.05;
    }
    return 1;
  }

  FinalTotal(productId, price, quanity, calculateWithVat) {
    const intialTotal = calculateWithVat
      ? this.NetTotal(price, quanity)
      : this.GrossTotal(price, quanity);

    return (
      intialTotal *
      this.StockRunningLowMultipler(quanity) *
      this.BulkBuyDiscount(quanity)
    );
  }

  IsStockAvailable(productId, quanity) {
    throw new Error("not Implemented");
  }
}

const Chia = require("chai");
const sinon = require("sinon");
const sinonChai = require("sinon-chai");

const Mocha = require("mocha");
const assert = require("assert");
const mocha = new Mocha();

// Bit of a hack, but thats how to make it work in code pad
mocha.suite.emit("pre-require", this, "solution", mocha);

Mocha.describe("Test suite for stock calculator", function () {
  let inventoryRepoStub;
  let calculator;

  beforeEach(function () {
    inventoryRepoStub = sinon.createStubInstance(InventoryRepo);
    calculator = new Calculator(inventoryRepoStub);
  });

  //Test Case for calculating gross total
  Mocha.it("Should calculate gross total correctly", function () {
    assert.strictEqual(calculator.GrossTotal(10, 5), 50);
  });

  //Test Case for calculating net total
  Mocha.it("Should calculate net total correctly", function () {
    assert.strictEqual(calculator.NetTotal(10, 5), 60);
  });

  //Test Case for applying correct bulkdiscount based on quantity
  Mocha.it("Should apply correct discount acccording to quantity", function () {
    assert.strictEqual(calculator.BulkBuyDiscount(10), 1);
    assert.strictEqual(calculator.BulkBuyDiscount(100), 0.9);
    assert.strictEqual(calculator.BulkBuyDiscount(1055), 0.8);
  });

  //Test Case for detecting stock
  Mocha.it("Should return true if stock is running low", function () {
    inventoryRepoStub.GetStock.returns(5);
    assert.strictEqual(calculator.IsStockRunningLow(1), true);
  });

  //Test Case for sufficient stock
  Mocha.it("Should return false if stock is not running low", function () {
    inventoryRepoStub.GetStock.returns(50);
    assert.strictEqual(calculator.IsStockRunningLow(1), false);
  });

  //Test case for not applying multipiler if stock is not running low 
  Mocha.it(
    "Should not apply multiplier if stock is not running low",function () {
      inventoryRepoStub.GetStock.returns(50);
      assert.strictEqual(calculator.StockRunningLowMultipler(1), 1);
    });
  
  //Test case for not applying multipiler if stock is running low 
  Mocha.it("Should apply multiplier if stock is running low", function () {
    inventoryRepoStub.GetStock.returns(5);
    assert.strictEqual(calculator.StockRunningLowMultipler(1), 1.05);
  });

  //Test case for calculate final total with vat and discounts
  Mocha.it("Should calculate final total including vat and discounts",function () {
      inventoryRepoStub.GetStock.returns(50);
      const finalTotal = calculator.FinalTotal(1, 10, 5, true);
      assert.strictEqual(finalTotal, 60);
    });

  //Test case for calculate final total with vat and stock running low
  Mocha.it("should calculate the final total with VAT and stock running low",function () {
      inventoryRepoStub.GetStock.returns(5);
      const finalTotal = calculator.FinalTotal(1, 10, 5, true);
      assert.strictEqual(finalTotal, 63);
    });

  //Test case for checking that IsStockAvailable throws an error  
  Mocha.it("Should throw an error when IsStockAvailable is called",function () {
      assert.throws(() => calculator.IsStockAvailable(1, 5),Error,"not Implemented");
    });

  Mocha.it("check boolean", function () {
    assert.ok(true);
  });

  Mocha.it("check number", function () {
    //Using Chia
    Chia.expect(2).to.equal(2);
  });
});
    
mocha.run();
