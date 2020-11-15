namespace TestInvoiceModule.Entities
{
    public class OrderProduct
    {
        public Product product;
        public int quantity;
        public decimal totalAmount;

        public OrderProduct(Product product, int quantity)
        {
            this.product = product;
            this.quantity = quantity;
            GetTotalAmount();
        }

        private void GetTotalAmount()
        {
            totalAmount = product.unitCost * quantity;
        }
    }
}