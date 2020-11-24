using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestInvoiceModule
{
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            //this means that whenever Ninject encounters interface, 
            //it will try to resolve it with bound class object
            Bind<IInvoiceGenerator>().To<InvoiceGenerator>();
            Bind<IMailManager>().To<MailManager>();
            Bind<ITestData>().To<TestData>();
            Bind<IOrderProcessor>().To<OrderProcessor>();
        }
    }
}
