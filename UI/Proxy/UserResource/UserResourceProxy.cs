using Model;

namespace Proxy
{
    public class UserResourceProxy : IUserResources
    {
        private readonly IUserResources _userResources;

        public WalletProxy Wallet { get; }
        public ShoppingCartProxy ShoppingCart { get; }
        public OrderHistoryProxy OrderHistory { get; }
        public TransactionsProxy Transactions { get; }
        
        IWallet IUserResources.Wallet => Wallet;
        IShoppingCart IUserResources.ShoppingCart => ShoppingCart;
        IOrderHistory IUserResources.OrderHistory => OrderHistory;
        ITransactions IUserResources.Transactions => Transactions;

        public UserResourceProxy(IUserResources userResources)
        {
            _userResources = userResources;
            Wallet = new WalletProxy(_userResources.Wallet);
            ShoppingCart = new ShoppingCartProxy(_userResources.ShoppingCart);
            OrderHistory = new OrderHistoryProxy(_userResources.OrderHistory);
            Transactions = new TransactionsProxy(_userResources.Transactions);
        }
    }
}