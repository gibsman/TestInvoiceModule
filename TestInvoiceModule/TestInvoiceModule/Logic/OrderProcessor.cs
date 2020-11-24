using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace TestInvoiceModule
{
    public interface IOrderProcessor
    {
        void GenerateOrders(int orderCount);

        void GenerateInvoices();

        void SendInvoices();

        void RemoveTemporaryFiles();
    }

    public class OrderProcessor : IOrderProcessor
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ITestData testData;
        private readonly IInvoiceGenerator invoiceGenerator;
        private readonly IMailManager mailManager;

        List<Order> generatedOrders;

        public OrderProcessor(ITestData testData, IInvoiceGenerator invoiceGenerator, 
            IMailManager mailManager)
        {
            this.testData = testData;
            this.invoiceGenerator = invoiceGenerator;
            this.mailManager = mailManager;
        }

        public void GenerateOrders(int orderCount)
        {
            generatedOrders = testData.GenerateRandomTestOrders(orderCount);
        }

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

        public void SendInvoices()
        {
            mailManager.SendMailBatch(generatedOrders);
        }

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