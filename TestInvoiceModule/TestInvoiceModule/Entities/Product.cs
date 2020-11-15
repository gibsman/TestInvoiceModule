using System;
using System.Collections.Generic;
using System.Text;

namespace TestInvoiceModule
{
    public class Product
    {
        public string name;
        public decimal unitCost;

        public Product(string name, decimal unitCost)
        {
            this.name = name;
            this.unitCost = unitCost;
        }
    }
}
