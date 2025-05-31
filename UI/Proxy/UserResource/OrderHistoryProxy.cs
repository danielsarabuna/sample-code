using System.Collections.Generic;
using Model;

namespace Proxy
{
    public class OrderHistoryProxy : IOrderHistory
    {
        private readonly IOrderHistory _orderHistory;
        public ICollection<string> Orders => throw new System.NotImplementedException();

        public OrderHistoryProxy(IOrderHistory orderHistory)
        {
            _orderHistory = orderHistory;
        }
    }
}