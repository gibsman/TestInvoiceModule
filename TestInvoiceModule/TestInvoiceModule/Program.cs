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
using System.Collections.ObjectModel;

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
            try
            {
                orderProcessor.GenerateOrders(orderCount);
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Critical error! Order list count is less than zero.");
                return;
            }
            Console.WriteLine("Random order batch generated");
            double pdfGenerationTime = 0;
            try
            {
                pdfGenerationTime = orderProcessor.GenerateInvoices();
            }
            catch (AggregateException exceptions)
            {
                ReadOnlyCollection<Exception> flattenedExceptions = exceptions.Flatten().InnerExceptions;
                foreach (Exception e in flattenedExceptions)
                {
                    Console.WriteLine("Error! Invoice couldn't get generated. " + e.Message);
                }
                //all invoices failed to generate, so termination is needed
                if (flattenedExceptions.Count == orderCount)
                {
                    return;
                }
            }
            Console.WriteLine("Invoices generated!");
            double mailSentTime = 0;
            try
            {
                mailSentTime = orderProcessor.SendInvoices();
            }
            catch (AggregateException exceptions)
            {
                ReadOnlyCollection<Exception> flattenedExceptions = exceptions.Flatten().InnerExceptions;
                foreach (Exception e in flattenedExceptions)
                {
                    Console.WriteLine("Error! Invoice couldn't get sent. " + e.Message);
                }
                //all invoices failed to send, so termination is needed
                if (flattenedExceptions.Count == orderCount)
                {
                    return;
                }
            }
            Console.WriteLine("Invoices sent!");
            Console.WriteLine("Time spent on PDF generation: " + pdfGenerationTime + " sec");
            Console.WriteLine("Time spent on mail sending: " + mailSentTime + " sec");
            Console.WriteLine("Total time elapsed: " + (pdfGenerationTime + mailSentTime) + " sec");
        }
    }
}
