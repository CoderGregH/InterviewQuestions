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

// Mocha.describe("Test suite for Stock Available", function () {
//   Mocha.it("check Stock Availablility", function () {
//     expect(cal.IsStockAvailable(1,100)).to.be.a('boolean')
//   });
// });


Mocha.describe('Test Suite For Calculator',function(){

  let calculator: Calculator;
  let inventoryRepoStub: InventoryRepo;

  Mocha.beforeEach(() => {
    inventoryRepoStub = {
      GetStock: () => 100 
    };
    calculator = new Calculator(inventoryRepoStub);
  });
  //  GrossTotal Check list
  Mocha.it("should calculate gross total correctly", function () {
    expect(calculator.GrossTotal(100,5)).to.be.equal(500)
  });

  // NetTotal Check List
  Mocha.it("should calculate net total correctly", function () {
    expect(calculator.NetTotal(100,5)).to.be.a('number').equal(600)
  });

  // BulkBuyDiscount Check List
  Mocha.it("should apply bulk buy discount correctly", function () {
    expect(calculator.BulkBuyDiscount(98)).to.be.a('number').equal(1)
    expect(calculator.BulkBuyDiscount(500)).to.be.a('number').equal(0.9)
    expect(calculator.BulkBuyDiscount(1001)).to.be.a('number').equal(0.8)
  });


  //  stock runnning low 
  Mocha.it('should determine stock running low correctly', () => {
    inventoryRepoStub.GetStock = () => 8;
    expect(calculator.IsStockRunningLow(1)).to.be.true;

    inventoryRepoStub.GetStock = () => 12;
    expect(calculator.IsStockRunningLow(1)).to.be.false;
  });

  // multiple Stock 
  Mocha.it('should apply stock running low multiplier correctly', () => {
    inventoryRepoStub.GetStock = () => 8;
    expect(calculator.StockRunningLowMultipler(1)).to.equal(1.05);

    inventoryRepoStub.GetStock = () => 12;
    expect(calculator.StockRunningLowMultipler(1)).to.equal(1);
  });

  // Is stock Available
  Mocha.it('should throw an error for IsStockAvailable', () => {
    expect(() => calculator.IsStockAvailable(1, 10)).to.throw('not Implemented');
  });


  Mocha.it('should calculate final total correctly', () => {

    // Vat Applied Without Low Stock and BulkBuyDiscount As less than 100
    inventoryRepoStub.GetStock = () => 12;
    expect(calculator.FinalTotal(1,100,98,true)).to.be.equal(11760)

    // Vat Applied Without Low Stock and BulkBuyDiscount As less than 1000
    inventoryRepoStub.GetStock = () => 12;
    expect(calculator.FinalTotal(1,100,998,true)).to.be.equal(107784)

    // Vat Applied Without Low Stock and BulkBuyDiscount As more than 1000
    inventoryRepoStub.GetStock = () => 12;
    expect(calculator.FinalTotal(1,100,1001,true)).to.be.equal(96096)

    // Vat Applied With Low Stock and BulkBuyDiscount As less than 100
    inventoryRepoStub.GetStock = () => 8;
    expect(calculator.FinalTotal(1,100,98,true)).to.be.equal(12348)
    
    // Vat Applied With Low Stock and BulkBuyDiscount As less than 1000
    inventoryRepoStub.GetStock = () => 8;
    expect(calculator.FinalTotal(1,100,998,true)).to.be.equal(113173.2)

    // Vat Applied With Low Stock and BulkBuyDiscount As more than 1000
    inventoryRepoStub.GetStock = () => 8;
    expect(calculator.FinalTotal(1,100,1001,true)).to.be.equal(100900.8)

    // Vat Not Applied Without Low Stock and BulkBuyDiscount As less than 100
    inventoryRepoStub.GetStock = () => 12;
    expect(calculator.FinalTotal(1,100,98,false)).to.be.equal(9800)

    // Vat Not Applied Without Low Stock and BulkBuyDiscount As less than 1000
    inventoryRepoStub.GetStock = () => 12;
    expect(calculator.FinalTotal(1,100,998,false)).to.be.equal(89820)

    // Vat Not Applied Without Low Stock and BulkBuyDiscount As more than 1000
    inventoryRepoStub.GetStock = () => 12;
    expect(calculator.FinalTotal(1,100,1001,false)).to.be.equal(80080)

    // Vat Not Applied With Low Stock and BulkBuyDiscount As less than 100
    inventoryRepoStub.GetStock = () => 8;
    expect(calculator.FinalTotal(1,100,98,false)).to.be.equal(10290)

    // Vat Not Applied With Low Stock and BulkBuyDiscount As less than 1000
    inventoryRepoStub.GetStock = () => 8;
    expect(calculator.FinalTotal(1,100,998,false)).to.be.equal(94311)

    // Vat Not Applied With Low Stock and BulkBuyDiscount As more than 1000
    inventoryRepoStub.GetStock = () => 8;
    expect(calculator.FinalTotal(1,100,1001,false)).to.be.equal(84084)


  });
});

Mocha.before(function () {
  console.log('---------------Started the Test-----------------')
});

Mocha.after(function () {
  console.log('---------------Done the Test-----------------')
});

mocha.run();