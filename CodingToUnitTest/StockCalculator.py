import random

class InventoryRepo:
    def GetStock(self, productId):
        return random.random() * 120

class Calculator:
    vatRate = 1.2

    def __init__(self, inventoryRepo : InventoryRepo ):
        self.inventoryRepo = inventoryRepo

    def GrossTotal(self, price, quanity):
        return price * quanity

    def NetTotal(self, price, quanity):
        return self.GrossTotal(price, quanity) * self.vatRate

    def BulkBuyDiscount(self, quanity) :
        if (quanity < 100):
            return 1.0

        if (quanity < 1000):
            return 0.9

        return 0.8

    def IsStockRunningLow(self, productId):
        currentStock = self.inventoryRepo.GetStock(productId)
        return currentStock < 10

    def StockRunningLowMultipler(self, productId):
        if (self.IsStockRunningLow(productId)):
            return 1.05

        return 1.0


    def FinalTotal(self, productId, price, quanity, calculateWithVat ):
        intialTotal = self.NetTotal(price, quanity) if calculateWithVat else self.GrossTotal(price, quanity)

        return ( intialTotal * self.StockRunningLowMultipler(quanity) * self.BulkBuyDiscount(quanity))

    def IsStockAvailable(self, productId, quanity):
        raise Exception("not Implemeneted Yet!")



import unittest

class TestAbsFunction(unittest.TestCase):
    def test_positive_number(self):
        self.assertEqual(abs(10), 10)

    def test_negative_number(self):
        self.assertEqual(abs(-10), 10)

    def test_zero(self):
        self.assertEqual(abs(0), 0)


unittest.main()


