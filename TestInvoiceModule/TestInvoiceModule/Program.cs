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
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("Error! There was a problem generating random orders. " + e.Message);
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
                string errorMessage = "Error! Invoice couldn't get generated. ";
                if (HandleMultipleExceptions(exceptions, errorMessage, orderCount))
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
                string errorMessage = "Error! Invoice couldn't get sent. ";
                if (HandleMultipleExceptions(exceptions, errorMessage, orderCount))
                {
                    return;
                }
            }
            Console.WriteLine("Invoices sent!");
            orderProcessor.RemoveTemporaryFiles();
            Console.WriteLine("Generated invoice files successfully deleted");
            Console.WriteLine("Time spent on PDF generation: " + pdfGenerationTime + " sec");
            Console.WriteLine("Time spent on mail sending: " + mailSentTime + " sec");
            Console.WriteLine("Total time elapsed: " + (pdfGenerationTime + mailSentTime) + " sec");
        }

        //returns true if all invoices failed to generate/send
        static bool HandleMultipleExceptions(AggregateException exceptions, string mainErrorMessage, int orderCount)
        {
            ReadOnlyCollection<Exception> flattenedExceptions = exceptions.Flatten().InnerExceptions;
            foreach (Exception e in flattenedExceptions)
            {
                Console.WriteLine(mainErrorMessage + e.Message);
            }
            bool allInvoicesFailed = (flattenedExceptions.Count == orderCount);
            return allInvoicesFailed;
        }
    }
}