using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestInvoiceModule
{
    /// <summary>
    /// Class <see cref="Bindings`1"/> binds interfaces with their specific implementaions using Ninject
    /// for DI (dependenpcy injection).
    /// </summary>
    public class Bindings : NinjectModule
    {
        /// <summary>
        /// Loads bound interfaces and implementaions into kernel.
        /// </summary>
        public override void Load()
        {
            //this means that whenever Ninject encounters interface, 
            //it will try to resolve it with bound class object
            Bind<IMailConfiguration>().To<MailConfiguration>();
            Bind<IInvoiceGenerator>().To<InvoiceGenerator>();
            Bind<IMailManager>().To<MailManager>();
            Bind<ITestData>().To<RandomTestData>();
            Bind<IOrderProcessor>().To<OrderProcessor>();
        }
    }
}
