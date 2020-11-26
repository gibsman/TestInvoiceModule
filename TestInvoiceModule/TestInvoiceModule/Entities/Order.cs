using System;
using System.Collections.Generic;

namespace TestInvoiceModule
{
    /// <summary>
    /// Class <see cref="Order`1"/> models order data.
    /// </summary>
    public class Order
    {
        public int id;
        public Client client;
        public DateTime orderDate;
        public DateTime dueDate;
        public List<OrderProduct> orderProducts;
        public decimal totalAmount;

        /// <summary>
        /// Initializes a new instance of the <see cref="Order`1"/> class.
        /// </summary>
        public Order(int id, Client client, DateTime orderDate, List<OrderProduct> orderProducts)
        {
            this.id = id;
            this.client = client;
            this.orderDate = orderDate;
            this.orderProducts = orderProducts;
            dueDate = orderDate.AddDays(30);
            GetTotalAmount();
        }

        /// <summary>
        /// Calculates total cost of the whole order.
        /// </summary>
        private void GetTotalAmount()
        {
            totalAmount = 0;
            foreach (OrderProduct orderProduct in orderProducts)
            {
                totalAmount += orderProduct.totalAmount;
            }
        }
    }
}
