using Ninject;
using NLog;
using System;
using System.Collections.ObjectModel;
using System.Reflection;

namespace TestInvoiceModule
{
    class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>This is the entry point for test invoice module</summary>
        static void Main(string[] args)
        {
            logger.Debug("Program initialized");
            int orderCount;
            if (args.Length == 0)
            {
                logger.Info("No commands provided. Use --help to get instructions on how to use this module.");
                logger.Debug("Program shutdown.");
                return;
            }
            else if (args[0].Equals("--help"))
            {
                logger.Info("This module generates multiple random invoices in form of files in .pdf format in the current folder and sends them to the recipient mail address. " +
                    "Said mail address, as well as sender's mail address and password must be provided through assignment of following environmental variables:");
                logger.Info("CLIENT_MAIL - recipient email address.");
                logger.Info("SMTP_USER_NAME - sender email address.");
                logger.Info("SMTP_PASSWORD - password for sender email address.");
                logger.Info("For more information on how to set up these environmental variables visit following page:");
                logger.Info("https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-5.0#windows \n");
                logger.Info("Usage:");
                logger.Info("[number-without-brackets] - Generates a specified number of random invoices in the current folder. " +
                    "After generation sends invoices to test mail address and then deletes them from the folder.");
                logger.Info("--h - Displays this help information.\n");
                logger.Debug("Help information printed. Program shutdown.");
                return;
            }
            else if (!int.TryParse(args[0], out orderCount))
            {
                Console.Write("Unknown command. Use --help in order to get information for usage.");
                logger.Debug("Unknown command. Program shutdown.");
                return;
            }
            logger.Debug("Number {0} is accepted as order count", orderCount);
            IKernel kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            //gets OrderProcessor object with all dependencies resolved
            //which is accomplished through type bindings in Bindings class
            var orderProcessor = kernel.Get<IOrderProcessor>();
            logger.Info("Generating {0} random orders...", orderCount);
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

        /// <summary> 
        /// Outputs description for multiple exceptions that can appear while generating or sending invoices.
        /// </summary>
        /// <param name="exceptions">Multiple exceptions that occured during runtime.</param>
        /// <param name="mainErrorMessage">Main message that preceeds occured exception's messages.</param>
        /// <param name="orderCount">Number of generated orders.</param>
        /// <returns>True if all invoices failed to generate/send, false otherwise.</returns>
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