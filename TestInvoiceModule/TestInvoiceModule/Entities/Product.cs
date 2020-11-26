namespace TestInvoiceModule
{
    /// <summary>
    /// Class <see cref="Product`1"/> models product data.
    /// </summary>
    public class Product
    {
        public string name;
        public decimal unitCost;

        /// <summary>
        /// Initializes a new instance of the <see cref="Product`1"/> class.
        /// </summary>
        public Product(string name, decimal unitCost)
        {
            this.name = name;
            this.unitCost = unitCost;
        }
    }
}
