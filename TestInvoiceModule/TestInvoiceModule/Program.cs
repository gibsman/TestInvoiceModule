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
            //SendMailBatch(orders);
            watch.Stop();
            Console.WriteLine("Order batch processed!");
            Console.WriteLine("Total time elapsed: " + (double)watch.ElapsedMilliseconds / 1000 + " sec");
        }

        private static void SendMailBatch(List<Order> orders)
        {
            Task[] mailTasks = new Task[orders.Count];
            for (int i = 0; i < orders.Count; i++)
            {
                mailTasks[i] = SendMail(orders[i].id.ToString(), orders[i].client.name, ConfigurationManager.AppSettings["TestMail"]);
            }
            Task.WaitAll(mailTasks);
        }

        private static async Task SendMail(string orderNum, string clientName, string recipientMail)
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("Placeholder Company", ConfigurationManager.AppSettings["UserMail"]));
            mailMessage.To.Add(new MailboxAddress(clientName, recipientMail));
            mailMessage.Subject = "Invoice For Order №" + orderNum;
            var attachment = new MimePart("application", "pdf")
            {
                Content = new MimeContent(File.OpenRead(orderNum + ".pdf"), ContentEncoding.Default),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName(orderNum + ".pdf")
            };
            var multipart = new Multipart("mixed");
            multipart.Add(attachment);
            mailMessage.Body = multipart;

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.mail.ru", 465, SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync(ConfigurationManager.AppSettings["UserMail"],
                    ConfigurationManager.AppSettings["Password"]);
                var options = FormatOptions.Default.Clone();
                await client.SendAsync(options, mailMessage);
            }
        }
    }
}
