const _ = require("lodash");

class Greeter {
  greeting: string;
  constructor(message: string) {
    this.greeting = message;
  }
  greet() {
    return "Hello, " + this.greeting;
  }
}

interface IInventoryRepo {
  GetStock(productId: number): number;
}

class InventoryRepo implements IInventoryRepo {
  public GetStock(productId: number): number {
    //This would actually call something like a DB to get the actual stock level
    return Math.random() * 120;
  }
}

class Calculator {
  private readonly _inventoryRepo: IInventoryRepo;
  constructor(inventoryRepo: IInventoryRepo) {
    this._inventoryRepo = inventoryRepo;
  }

  public GrossTotal(price: number, quanity: number) {
    return price * quanity;
  }

  vatRate = 1.2;

  public NetTotal(price: number, quanity: number) {
    return this.GrossTotal(price, quanity) * this.vatRate;
  }

  public BulkBuyDiscount(quanity: number): number {
    if (quanity < 100)
      //No discount
      return 1;

    if (quanity < 1000)
      //10 percent
      return 0.9;

    //a generous 20 percent
    return 0.8;
  }

  public IsStockRunningLow(productId: number): boolean {
    var currentStock = this._inventoryRepo.GetStock(productId);
    return currentStock < 10;
  }

  public StockRunningLowMultipler(productId: number): number {
    if (this.IsStockRunningLow(productId)) {
      //add five percent if stock is running low
      return 1.05;
    }
    return 1;
  }

  public FinalTotal(
    productId: number,
    price: number,
    quanity: number,
    calculateWithVat: boolean
  ): number {
    var intialTotal = calculateWithVat
      ? this.NetTotal(price, quanity)
      : this.GrossTotal(price, quanity);

    return (
      intialTotal *
      this.StockRunningLowMultipler(quanity) *
      this.BulkBuyDiscount(quanity)
    );
  }

  public IsStockAvailable(productId: number, quanity: number): boolean {
    throw new Error("not Implemented");
  }
}

const Chia = require("chai");
const sinon = require("sinon");
const sinonChai = require("sinon-chai");

const Mocha = require("mocha");
const assert = require("assert");
const mocha = new Mocha();
const expect = Chia.expect;

// Bit of a hack, but thats how to make it work in code pad
mocha.suite.emit("pre-require", this, "solution", mocha);

Mocha.describe("Test suite for stock calculator", function () {
  let calculator: Calculator;
  let inventoryRepoStub: InventoryRepo;

  Mocha.beforeEach(function () {
    inventoryRepoStub = sinon.createStubInstance(InventoryRepo);
    calculator = new Calculator(inventoryRepoStub);
  });

  //Test Case for calculating gross total
  Mocha.it("Should calculate gross total correctly", function () {
    expect(calculator.GrossTotal(10, 5)).to.be.equal(50);
  });

  //Test Case for calculating net total
  Mocha.it("Should calculate net total correctly", function () {
    expect(calculator.NetTotal(10, 5)).to.be.equal(60);
  });

  //Test Case for applying correct bulkdiscount based on quantity
  Mocha.it("Should apply correct discount acccording to quantity", function () {
    expect(calculator.BulkBuyDiscount(10)).to.be.equal(1);
    expect(calculator.BulkBuyDiscount(100)).to.be.equal(0.9);
    expect(calculator.BulkBuyDiscount(1055)).to.be.equal(0.8);
  });

  //Test Case for detecting stock
  Mocha.it("Should return true if stock is running low", function () {
    inventoryRepoStub.GetStock = () => 5;
    expect(calculator.IsStockRunningLow(1)).to.be.true;
  });

  //Test Case for sufficient stock
  Mocha.it("Should return false if stock is not running low", function () {
    inventoryRepoStub.GetStock = () => 50;
    expect(calculator.IsStockRunningLow(1)).to.be.false;
  });

   //Test case for not applying multipiler if stock is not running low 
  Mocha.it(
    "Should not apply multiplier if stock is not running low",
    function () {
      inventoryRepoStub.GetStock = () => 50;
      expect(calculator.StockRunningLowMultipler(1)).to.equal(1);
    }
  );

  //Test case for not applying multipiler if stock is running low 
  Mocha.it("Should apply multiplier if stock is running low", function () {
    inventoryRepoStub.GetStock = () => 5;
    expect(calculator.StockRunningLowMultipler(1)).to.equal(1.05);
  });

  //Test case for calculate final total
  Mocha.it(
    "Should calculate final total including vat and discounts",
    function () {
      inventoryRepoStub.GetStock = () => 50;
      expect(calculator.FinalTotal(1, 10, 5, true)).to.be.equal(60);
      expect(calculator.FinalTotal(1, 200, 1200, true)).to.be.equal(230400);
    }
  );

  //Test case for checking that IsStockAvailable throws an error
  Mocha.it(
    "Should throw an error when IsStockAvailable is called",
    function () {
      expect(() => calculator.IsStockAvailable(1, 6)).to.throw(
        "not Implemented"
      );
    }
  );

  Mocha.it("check boolean", function () {
    assert.ok(true);
  });

  Mocha.it("check number", function () {
    //Using Chia
    expect(2).to.equal(2);
  });
});

mocha.run();
