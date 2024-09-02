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
    return Math.random() * 120;
  }
}

class Calculator {
  private readonly _inventoryRepo: IInventoryRepo;
  constructor(inventoryRepo: IInventoryRepo) {
    this._inventoryRepo = inventoryRepo;
  }

  public GrossTotal(price: number, quantity: number) {
    return price * quantity;
  }

  vatRate = 1.2;

  public NetTotal(price: number, quantity: number) {
    return this.GrossTotal(price, quantity) * this.vatRate;
  }

  public BulkBuyDiscount(quantity: number): number {
    if (quantity < 100) return 1;

    if (quantity < 1000) return 0.9;

    return 0.8;
  }

  public IsStockRunningLow(productId: number): boolean {
    var currentStock = this._inventoryRepo.GetStock(productId);
    return currentStock < 10;
  }

  public StockRunningLowMultiplier(productId: number): number {
    if (this.IsStockRunningLow(productId)) {
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

    const total =
      intialTotal *
      this.StockRunningLowMultiplier(productId) *
      this.BulkBuyDiscount(quanity);

    return parseFloat(total.toFixed(1));
  }

  public IsStockAvailable(productId: number, quanity: number): boolean {
    throw new Error("not Implemented");
  }
}

const Chai = require("chai");
const sinon = require("sinon");
const sinonChai = require("sinon-chai");

const Mocha = require("mocha");
const assert = require("assert");
const mocha = new Mocha();
let inventoryRepoMock: InventoryRepo = {
  GetStock: sinon.stub().returns(100),
};

let calculator = new Calculator(inventoryRepoMock);
// mocha.suite.emit("pre-require", this, "solution", mocha);

Mocha.describe("Test suite", function () {
  Mocha.it("check boolean", function () {
    assert.ok(true);
  });

  Mocha.it("check number", function () {
    Chai.expect(2).to.equal(2);
  });
});

Mocha.describe("Calculator", function () {
  Mocha.it("it should return the gross total", function () {
    Chai.expect(calculator.GrossTotal(2, 2)).to.equal(4);
  });

  Mocha.it("it should return NetTotal", function () {
    Chai.expect(calculator.NetTotal(2, 2)).to.equal(4.8);
  });
});

Mocha.describe("BulkBuyDiscount", function () {
  assert.strictEqual(calculator.BulkBuyDiscount(10), 1);
  assert.strictEqual(calculator.BulkBuyDiscount(100), 0.9);
  assert.strictEqual(calculator.BulkBuyDiscount(1055), 0.8);
});

Mocha.describe("FinalTotal", function () {
  Mocha.it(
    "it should return the finalTotal with no vat, no discount and stock is not running low",
    function () {
      const total = calculator.FinalTotal(1, 2, 3, false);
      Chai.expect(total).to.equal(6);
    }
  );

  Mocha.it(
    "it should return the finalTotal with vat, no discount and stock is not running low",
    function () {
      const total = calculator.FinalTotal(1, 2, 3, true);
      Chai.expect(total).to.equal(7.2);
    }
  );
});

Mocha.it("Should throw an error when IsStockAvailable is called",function () {
  assert.throws(() => calculator.IsStockAvailable(1, 5),Error,"not Implemented");
});

mocha.run();