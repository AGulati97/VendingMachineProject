using System;
using System.Collections.Generic;




namespace VendingMachineApp
{
    public class VendingMachine
    {
        private readonly ICoinValidator _coinValidator;
        private readonly IProductCatalog _productCatalog;
        private readonly ITransactionService _transactionService;

        public string DisplayMessage { get; private set; } = "INSERT COIN";
        public string CoinReturn { get; private set; } = string.Empty;

        public VendingMachine(ICoinValidator coinValidator, IProductCatalog productCatalog, ITransactionService transactionService)
        {
            _coinValidator = coinValidator;
            _productCatalog = productCatalog;
            _transactionService = transactionService;
        }

        public void AcceptCoin(string coin)
        {
            if (_coinValidator.IsValidCoin(coin))
            {
                _transactionService.AddAmount(_coinValidator.GetValue(coin));
                CoinReturn = string.Empty;
                DisplayMessage = _transactionService.GetCurrentAmount();
            }
            else
            {
                CoinReturn = coin;
            }
        }

        public string SelectProduct(string productName)
        {
            if (!_productCatalog.IsValidProduct(productName))
            {
                return "INVALID PRODUCT";
            }

            var price = _productCatalog.GetPrice(productName);
            if (_transactionService.HasSufficientFunds(price))
            {
                _transactionService.DeductAmount(price);
                DisplayMessage = "INSERT COIN";
                return "THANK YOU";
            }
            else
            {
                DisplayMessage = _transactionService.GetCurrentAmount();
                return $"PRICE ${price:F2}";
            }
        }
    }

    public interface ICoinValidator
    {
        bool IsValidCoin(string coin);
        decimal GetValue(string coin);
    }

    public class CoinValidator : ICoinValidator
    {
        private readonly Dictionary<string, decimal> _validCoins = new()
        {
            { "nickel", 0.05m },
            { "dime", 0.10m },
            { "quarter", 0.25m }
        };

        public bool IsValidCoin(string coin) => _validCoins.ContainsKey(coin);
        public decimal GetValue(string coin) => _validCoins[coin];
    }

    public interface IProductCatalog
    {
        bool IsValidProduct(string product);
        decimal GetPrice(string product);
    }

    public class ProductCatalog : IProductCatalog
    {
        private readonly Dictionary<string, decimal> _products = new()
        {
            { "Cola", 1.00m },
            { "Chips", 0.50m },
            { "Candy", 0.65m }
        };

        public bool IsValidProduct(string product) => _products.ContainsKey(product);
        public decimal GetPrice(string product) => _products[product];
    }

    public interface ITransactionService
    {
        void AddAmount(decimal amount);
        bool HasSufficientFunds(decimal price);
        void DeductAmount(decimal price);
        string GetCurrentAmount();
    }

    public class TransactionService : ITransactionService
    {
        private decimal _amount = 0m;

        public void AddAmount(decimal amount) => _amount += amount;

        public bool HasSufficientFunds(decimal price) => _amount >= price;

        public void DeductAmount(decimal price) => _amount -= price;

        public string GetCurrentAmount() => _amount == 0m ? "INSERT COIN" : _amount.ToString("C2");
    }

    class Program
    {
        public static void Main(string[] args)
        {
            var vendingMachine = new VendingMachine(
                new CoinValidator(),
                new ProductCatalog(),
                new TransactionService()
            );

            Console.WriteLine("Welcome to the Vending Machine!");
            string input;

            while (true)
            {
                Console.WriteLine("\nCurrent Balance: " + vendingMachine.DisplayMessage);
                Console.WriteLine("1. Insert Coin (nickel, dime, quarter)");
                Console.WriteLine("2. Select Product (Cola, Chips, Candy)");
                Console.WriteLine("3. Exit");

                Console.Write("\nEnter your choice: ");
                input = Console.ReadLine()?.Trim().ToLower();

                switch (input)
                {
                    case "1":
                        Console.Write("Insert coin: ");
                        string coin = Console.ReadLine()?.Trim().ToLower();
                        vendingMachine.AcceptCoin(coin);

                        if (!string.IsNullOrEmpty(vendingMachine.CoinReturn))
                        {
                            Console.WriteLine($"Returned coin: {vendingMachine.CoinReturn}");
                        }
                        break;

                    case "2":
                        Console.Write("Enter product name: ");
                        string product = Console.ReadLine()?.Trim();
                        string result = vendingMachine.SelectProduct(product);
                        Console.WriteLine(result);
                        break;

                    case "3":
                        Console.WriteLine("Thank you! Exiting.");
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
    }
}


