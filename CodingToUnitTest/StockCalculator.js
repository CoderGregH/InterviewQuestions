const _ = require('lodash');

class InventoryRepo  {
  GetStock(productId){
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

  FinalTotal(
    productId,
    price,
    quanity,
    calculateWithVat
  ) {
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
