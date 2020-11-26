using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace TestInvoiceModule
{
    /// <summary>
    /// Manages whole process of invoice generation and sending.
    /// </summary>
    public interface IOrderProcessor
    {
        void GenerateOrders(int orderCount);

        void GenerateInvoices();

        void SendInvoices();

        void RemoveTemporaryFiles();
    }

    /// <summary>
    /// Class <see cref="OrderProcessor`1"/> manages all steps of invoice processing: order generation, invoice files generation,
    /// sending and removal.
    /// </summary>
    public class OrderProcessor : IOrderProcessor
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ITestData testData;
        private readonly IInvoiceGenerator invoiceGenerator;
        private readonly IMailManager mailManager;

        List<Order> generatedOrders;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderProcessor`1"/> class.
        /// </summary>
        /// <param name="testData">An instance of <see cref="ITestData`1"/> for order generation.</param>
        /// <param name="invoiceGenerator">An instance of <see cref="IInvoiceGenerator`1"/> for invoice files generation.</param>
        /// <param name="mailManager">An instance of <see cref="IMailManager`1"/> for invoice sending.</param>
        public OrderProcessor(ITestData testData, IInvoiceGenerator invoiceGenerator, 
            IMailManager mailManager)
        {
            this.testData = testData;
            this.invoiceGenerator = invoiceGenerator;
            this.mailManager = mailManager;
        }

        /// <summary>
        /// Generates random orders using instance of <see cref="ITestData`1"/>.
        /// </summary>
        /// <param name="orderCount">Number of orders to generate.</param>
        public void GenerateOrders(int orderCount)
        {
            generatedOrders = testData.GenerateTestOrders(orderCount);
        }

        /// <summary>
        /// Concurrently generates invoice files using instance of <see cref="IInvoiceGenerator`1"/>.
        /// </summary>
        public void GenerateInvoices()
        {
            var exceptions = new ConcurrentQueue<Exception>();
            //generates all PDF invoices concurrently
            Parallel.ForEach(generatedOrders, order =>
            {
                try
                {
                    invoiceGenerator.Generate(order);
                }
                catch (Exception e)
                {
                    exceptions.Enqueue(e);
                }
            });
            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }

        /// <summary>
        /// Sends all generated invoices with the help of <see cref="IMailManager`1"/>.
        /// </summary>
        public void SendInvoices()
        {
            mailManager.SendMailBatch(generatedOrders);
        }

        /// <summary>
        /// Deletes all generated invoice files.
        /// </summary>
        public void RemoveTemporaryFiles()
        {
            for (int i = 0; i < generatedOrders.Count; i++)
            {
                File.Delete(generatedOrders[i].id + ".pdf");
                logger.Debug("Invoice № {0} file deleted", generatedOrders[i].id);
            }
        }
    }
}