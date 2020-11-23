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
            logger.Debug("Program initialized");
            logger.Debug("Waiting for user input for order count");
            Console.Write("Enter number of random orders to generate:");
            int orderCount;
            while (!int.TryParse(Console.ReadLine(), out orderCount))
            {
                Console.Write("Incorrect input! Please enter a valid number:");
                logger.Debug("Input string is non numerical. Waiting for another input");
            }
            logger.Debug("Number {0} is accepted as order count", orderCount);
            OrderProcessor orderProcessor = new OrderProcessor();
            try
            {
                orderProcessor.GenerateOrders(orderCount);
            }
            catch (ArgumentOutOfRangeException e)
            {
                logger.Error(e, "Error! There was a problem generating random orders. {0}", e.Message);
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
                    logger.Fatal("All invoices failed to generate. Program shutdown.");
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
                    logger.Fatal("All invoices failed to send. Program shutdown.");
                    return;
                }
            }
            logger.Info("Invoices sent!");
            orderProcessor.RemoveTemporaryFiles();
            logger.Info("Generated invoice files successfully deleted");
            logger.Info("Time spent on PDF generation: {0} sec", pdfGenerationTime);
            logger.Info("Time spent on mail sending: {0} sec", mailSentTime);
            logger.Info("Total time elapsed: {0} sec", pdfGenerationTime + mailSentTime);
            logger.Debug("Program shutdown");
        }

        //returns true if all invoices failed to generate/send
        static bool HandleMultipleExceptions(AggregateException exceptions, string mainErrorMessage, int orderCount)
        {
            ReadOnlyCollection<Exception> flattenedExceptions = exceptions.Flatten().InnerExceptions;
            foreach (Exception e in flattenedExceptions)
            {
                logger.Error(exceptions, mainErrorMessage + e.Message);
            }
            bool allInvoicesFailed = (flattenedExceptions.Count == orderCount);
            return allInvoicesFailed;
        }
    }
}