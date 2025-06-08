using VendingMachineApp;

namespace VendingMachineApp.Tests
{
    [TestFixture]
    public class VendingMachineTests
    {
        private VendingMachine _vendingMachine;

        [SetUp]
        public void Setup()
        {
            _vendingMachine = new VendingMachine(
                new CoinValidator(),
                new ProductCatalog(),
                new TransactionService()
            );
        }

        [TestCase("nickel", "$0.05")]
        [TestCase("dime", "$0.10")]
        [TestCase("quarter", "$0.25")]
        public void AcceptCoin_ValidCoin_UpdatesAmount(string coin, string expectedDisplay)
        {
            _vendingMachine.AcceptCoin(coin);
            Assert.AreEqual(expectedDisplay, _vendingMachine.DisplayMessage);
        }

        [TestCase("penny")]
        [TestCase("fakecoin")]
        public void AcceptCoin_InvalidCoin_ReturnsToCoinReturn(string coin)
        {
            _vendingMachine.AcceptCoin(coin);
            Assert.AreEqual(coin, _vendingMachine.CoinReturn);
        }

        [Test]
        public void SelectProduct_WithSufficientFunds_ReturnsThankYouAndResets()
        {
            _vendingMachine.AcceptCoin("quarter");
            _vendingMachine.AcceptCoin("quarter");
            _vendingMachine.AcceptCoin("quarter");
            _vendingMachine.AcceptCoin("quarter");

            string result = _vendingMachine.SelectProduct("Cola");

            Assert.AreEqual("THANK YOU", result);
            Assert.AreEqual("INSERT COIN", _vendingMachine.DisplayMessage);
        }

        [Test]
        public void SelectProduct_InsufficientFunds_ShowsPrice()
        {
            _vendingMachine.AcceptCoin("quarter");
            string result = _vendingMachine.SelectProduct("Candy");

            Assert.AreEqual("PRICE $0.65", result);
            Assert.AreEqual("$0.25", _vendingMachine.DisplayMessage);
        }

        [Test]
        public void SelectProduct_InvalidProduct_ReturnsError()
        {
            string result = _vendingMachine.SelectProduct("Juice");
            Assert.AreEqual("INVALID PRODUCT", result);
        }

        [Test]
        public void DisplayMessage_Initially_ShowsInsertCoin()
        {
            Assert.AreEqual("INSERT COIN", _vendingMachine.DisplayMessage);
        }
    }
}