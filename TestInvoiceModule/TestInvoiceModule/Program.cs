using System;
using System.Configuration;
using MimeKit;
using System.IO;
using MailKit.Net.Smtp;
using MailKit.Security;
using Spire.Doc;
using Spire.Doc.Documents;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TestInvoiceModule
{
    class Program
    {
        static void Main(string[] args)
        {
            TestData testData = new TestData();
            //this used to speed up future pdf generation
            InvoiceGenerator.Generate(testData.GenerateRandomTestOrders(1)[0]);
            ProcessOrders();
        }

        private static void ProcessOrders()
        {
            TestData testData = new TestData();
            int orderCount = 100;
            List<Order> orders = testData.GenerateRandomTestOrders(orderCount);
            Console.WriteLine("Random test order batch generated (Batch length = " + orders.Count + ")");
            var watch = System.Diagnostics.Stopwatch.StartNew();
            //generates all PDF invoices concurrently
            Parallel.ForEach(orders, order => InvoiceGenerator.Generate(order));
            watch.Stop();
            double pdfGenerationTime = (double)watch.ElapsedMilliseconds / 1000;
            watch.Reset();
            watch.Start();
            MailManager.SendMailBatch(orders);
            watch.Stop();
            double mailSentTime = (double)watch.ElapsedMilliseconds / 1000;
            Console.WriteLine("Order batch processed!");
            Console.WriteLine("Time spent on PDF generation: " + pdfGenerationTime + " sec");
            Console.WriteLine("Time spent on mail sending: " + mailSentTime + " sec");
            Console.WriteLine("Total time elapsed: " + (pdfGenerationTime + mailSentTime) + " sec");
        }
    }
}
