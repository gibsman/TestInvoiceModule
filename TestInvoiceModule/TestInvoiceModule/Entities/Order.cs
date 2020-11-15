using System;
using System.Collections.Generic;
using System.Text;

namespace TestInvoiceModule.Entities
{
    public class Order
    {
        public int id;
        public Client client;
        public DateTime orderDate;
        public DateTime dueDate;
        public List<OrderProduct> orderProducts;
        public decimal totalAmount;

        public Order(int id, Client client, DateTime orderDate, List<OrderProduct> orderProducts)
        {
            this.id = id;
            this.client = client;
            this.orderDate = orderDate;
            this.orderProducts = orderProducts;
            dueDate = orderDate.AddDays(30);
            GetTotalAmount();
        }

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
