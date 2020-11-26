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
            int orderCount = 0;
            if (args.Length == 0)
            {
                Console.Write("No commands provided. Use command '--help' to get instructions on how to use this module.");
                logger.Debug("No commands provided. Program shutdown.");
                return;
            }
            else if (args[0].Equals("--help"))
            {
                Console.Write("insert help instructions here");
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