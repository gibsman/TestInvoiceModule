using System;
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
            //generates all PDF invoices concurrently
            Parallel.ForEach(generatedOrders, order => InvoiceGenerator.Generate(order));
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
            watch.Stop();
            double mailSentTime = (double)watch.ElapsedMilliseconds / 1000;
            return mailSentTime;
        }
    }
}
