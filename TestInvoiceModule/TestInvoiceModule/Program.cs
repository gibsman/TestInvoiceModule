using NLog;
using System;
using System.Collections.ObjectModel;

namespace TestInvoiceModule
{
    class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

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
                logger.Info("Error! There was a problem generating random orders. " + e.Message);
                return;
            }
            logger.Info("Random order batch generated");
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
            logger.Info("Invoices generated!");
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
            logger.Info("Invoices sent!");
            orderProcessor.RemoveTemporaryFiles();
            logger.Info("Generated invoice files successfully deleted");
            logger.Info("Time spent on PDF generation: " + pdfGenerationTime + " sec");
            logger.Info("Time spent on mail sending: " + mailSentTime + " sec");
            logger.Info("Total time elapsed: " + (pdfGenerationTime + mailSentTime) + " sec");
        }

        //returns true if all invoices failed to generate/send
        static bool HandleMultipleExceptions(AggregateException exceptions, string mainErrorMessage, int orderCount)
        {
            ReadOnlyCollection<Exception> flattenedExceptions = exceptions.Flatten().InnerExceptions;
            foreach (Exception e in flattenedExceptions)
            {
                logger.Info(mainErrorMessage + e.Message);
            }
            bool allInvoicesFailed = (flattenedExceptions.Count == orderCount);
            return allInvoicesFailed;
        }
    }
}