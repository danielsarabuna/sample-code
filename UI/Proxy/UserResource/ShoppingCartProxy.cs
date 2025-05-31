using System.Collections.Generic;
using Model;

namespace Proxy
{
    public class ShoppingCartProxy : IShoppingCart
    {
        private readonly IShoppingCart _shoppingCart;
        public ICollection<string> Items => throw new System.NotImplementedException();

        public decimal TotalPrice => throw new System.NotImplementedException();

        public ShoppingCartProxy(IShoppingCart shoppingCart)
        {
            _shoppingCart = shoppingCart;
        }
    }
}