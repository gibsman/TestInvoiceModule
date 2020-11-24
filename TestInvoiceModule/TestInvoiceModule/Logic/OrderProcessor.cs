using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace TestInvoiceModule
{
    public class OrderProcessor
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        List<Order> generatedOrders;

        public OrderProcessor()
        {

        }

        public void GenerateOrders(int orderCount)
        {
            TestData data = new TestData();
            generatedOrders = data.GenerateRandomTestOrders(orderCount);
        }

        //returns time spent on generating invoices (in seconds)
        public double GenerateInvoices()
        {
            var watch = Stopwatch.StartNew();
            var exceptions = new ConcurrentQueue<Exception>();

            //generates all PDF invoices concurrently
            Parallel.ForEach(generatedOrders, order =>
            {
                try
                {
                    InvoiceGenerator invoiceGenerator = new InvoiceGenerator();
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
            //this is unreachable if exceptions are thrown
            watch.Stop();
            double pdfGenerationTime = (double)watch.ElapsedMilliseconds / 1000;
            return pdfGenerationTime;
        }

        //returns time spent on sending invoices (in seconds)
        public double SendInvoices()
        {
            var watch = Stopwatch.StartNew();
            watch.Start();
            MailManager manager = new MailManager();
            manager.SendMailBatch(generatedOrders);
            //this is unreachable if exceptions are thrown
            watch.Stop();
            double mailSentTime = (double)watch.ElapsedMilliseconds / 1000;
            return mailSentTime;
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