using System;
using System.Collections.Generic;
using System.Text;

using StripePlayground.Stripe.Models;

namespace StripePlayground.Stripe
{
    public class OrderClient
    {
        private Dictionary<string, Order> Orders;

        public OrderClient()
        {
            this.Orders = new Dictionary<string, Order>();
        }

        public Order CreateOrder(Order order)
        {
            if (order == null)
            {
                throw new Exception($"Order cannot be null");
            }

            Orders.Add(order.Id, order);

            return Orders[order.Id];
        }

        public Order GetOrderById(string id)
        {
            return Orders[id];
        }

        public Order UpdateOrder(string id, Order order)
        {
            if (order == null)
            {
                throw new Exception($"Order cannot be null.");
            }

            if (id != order.Id)
            {
                throw new Exception($"OrderId's do not match.");
            }

            Orders[id] = order;

            return Orders[id];
        }
    }
}
