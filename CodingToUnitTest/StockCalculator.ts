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

// Bit of a hack, but thats how to make it work in code pad
mocha.suite.emit("pre-require", this, "solution", mocha);

Mocha.describe("Test suite", function () {
  Mocha.it("check boolean", function () {
    assert.ok(true);
  });

  Mocha.it("check number", function () {
    //Using Chia
    Chia.expect(2).to.equal(2);
  });
});

mocha.run();
