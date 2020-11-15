using System;
using System.Collections.Generic;
using System.Text;

namespace TestInvoiceModule
{
    public class Client
    {
        public string name;
        public string mail;
        public string address;
        public string phone;

        public Client(string name, string mail, string address, string phone)
        {
            this.name = name;
            this.mail = mail;
            this.address = address;
            this.phone = phone;
        }
    }
}
