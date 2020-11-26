namespace TestInvoiceModule
{
    /// <summary>
    /// Class <see cref="Client`1"/> models client data.
    /// </summary>
    public class Client
    {
        public string name;
        public string mail;
        public string address;
        public string phone;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client`1"/> class.
        /// </summary>
        public Client(string name, string mail, string address, string phone)
        {
            this.name = name;
            this.mail = mail;
            this.address = address;
            this.phone = phone;
        }
    }
}
