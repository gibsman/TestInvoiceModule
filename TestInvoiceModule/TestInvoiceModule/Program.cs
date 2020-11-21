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
            Console.Write("Enter number of random orders to generate:");
            int orderCount;
            while (!int.TryParse(Console.ReadLine(), out orderCount))
            {
                Console.Write("Incorrect input! Please enter a valid number:");
            }
            OrderProcessor orderProcessor = new OrderProcessor();
            orderProcessor.GenerateOrders(orderCount);
            Console.WriteLine("Random order batch generated");

            double pdfGenerationTime = orderProcessor.GenerateInvoices();
            Console.WriteLine("Invoices generated!");
            double mailSentTime = orderProcessor.SendInvoices();
            Console.WriteLine("Invoices sent!");
            Console.WriteLine("Time spent on PDF generation: " + pdfGenerationTime + " sec");
            Console.WriteLine("Time spent on mail sending: " + mailSentTime + " sec");
            Console.WriteLine("Total time elapsed: " + (pdfGenerationTime + mailSentTime) + " sec");
        }
    }
}
