namespace TestInvoiceModule
{
    /// <summary>
    /// Class <see cref="OrderProduct`1"/> models ordered product data.
    /// </summary>
    public class OrderProduct
    {
        public Product product;
        public int quantity;
        public decimal totalAmount;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderProduct`1"/> class.
        /// </summary>
        public OrderProduct(Product product, int quantity)
        {
            this.product = product;
            this.quantity = quantity;
            GetTotalAmount();
        }

        /// <summary>
        /// Calculates total cost of one ordered product type.
        /// </summary>
        private void GetTotalAmount()
        {
            totalAmount = product.unitCost * quantity;
        }
    }
}