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
            ITestData testData = new TestData();
            IInvoiceGenerator invoiceGenerator = new InvoiceGenerator();
            IMailManager mailManager = new MailManager();
            OrderProcessor orderProcessor = new OrderProcessor(testData, invoiceGenerator, mailManager);
            logger.Info("Generating random orders...");
            try
            {
                orderProcessor.GenerateOrders(orderCount);
            }
            catch (ArgumentOutOfRangeException e)
            {
                logger.Error(e, "Error! There was a problem generating random orders. {0}", e.Message);
                return;
            }
            logger.Info("Order batch generated!");
            logger.Info("Generating invoices...");
            try
            {
                orderProcessor.GenerateInvoices();
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
            logger.Info("Sending invoices...");
            try
            {
                orderProcessor.SendInvoices();
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
            logger.Info("Deleting generated invoices...");
            orderProcessor.RemoveTemporaryFiles();
            logger.Info("All generated invoice files deleted!");
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