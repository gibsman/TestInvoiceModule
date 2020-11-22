using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace TestInvoiceModule
{
    public class OrderProcessor
    {
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
                    InvoiceGenerator.Generate(order);
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
            MailManager.SendMailBatch(generatedOrders);
            //this is unreachable if exceptions are thrown
            watch.Stop();
            double mailSentTime = (double)watch.ElapsedMilliseconds / 1000;
            return mailSentTime;
        }
    }
}
