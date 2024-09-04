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

Mocha.describe('Calculator', () => {
  let cal;
  let inventoryRepoMock;

  beforeEach(() => {
    inventoryRepoMock = sinon.createStubInstance(InventoryRepo);
    cal = new Calculator(inventoryRepoMock);
  });

  Mocha.it('GrossTotal', () => {
    const price = 50;
    const quantity = 5;
    const result = cal.GrossTotal(price, quantity);
    Chia.expect(result).to.equal(250);
  });

  Mocha.it('NetTotal', () => {
    const price = 50;
    const quantity = 5;
    const expectedGrossTotal = 250;
    const vatRate = 1.2;
    sinon.stub(cal, 'GrossTotal').returns(expectedGrossTotal);

    const result = cal.NetTotal(price, quantity);
    Chia.expect(result).to.equal(expectedGrossTotal * vatRate);
  });

  Mocha.it('BulkBuyDiscount correctly for quantity < 100', () => {
    const quantity = 50;
    const result = cal.BulkBuyDiscount(quantity);
    Chia.expect(result).to.equal(1);
  });

  Mocha.it('BulkBuyDiscount correctly for quantity < 1000', () => {
    const quantity = 500;
    const result = cal.BulkBuyDiscount(quantity);
    Chia.expect(result).to.equal(0.9);
  });

  Mocha.it('BulkBuyDiscount correctly for quantity >= 1000', () => {
    const quantity = 1000;
    const result = cal.BulkBuyDiscount(quantity);
    Chia.expect(result).to.equal(0.8);
  });

  Mocha.it('if stock is running low', () => {
    inventoryRepoMock.GetStock.returns(5);
    const result = cal.IsStockRunningLow('product-id');
    Chia.expect(result).to.be.true;
  });

  Mocha.it('if stock is not running low', () => {
    inventoryRepoMock.GetStock.returns(15);
    const result = cal.IsStockRunningLow('product-id');
    Chia.expect(result).to.be.false;
  });

  Mocha.it('StockRunningLowMultipler correctly when stock is running low', () => {
    sinon.stub(cal, 'IsStockRunningLow').returns(true);
    const result = cal.StockRunningLowMultipler('product-id');
    Chia.expect(result).to.equal(1.05);
  });

  Mocha.it('StockRunningLowMultipler correctly when stock is not running low', () => {
    sinon.stub(cal, 'IsStockRunningLow').returns(false);
    const result = cal.StockRunningLowMultipler('product-id');
    Chia.expect(result).to.equal(1);
  });

  Mocha.it('FinalTotal correctly', () => {
    const price = 100;
    const quantity = 10;
    const productId = 'product-id';
    sinon.stub(cal, 'NetTotal').returns(1200);
    sinon.stub(cal, 'GrossTotal').returns(1000);
    sinon.stub(cal, 'StockRunningLowMultipler').returns(1.05);
    sinon.stub(cal, 'BulkBuyDiscount').returns(0.9);

    const result = cal.FinalTotal(productId, price, quantity, true);
    Chia.expect(result).to.equal(1200 * 1.05 * 0.9);
  });

});

mocha.run();