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

        }

        private static void ProcessOrders()
        {
            TestData testData = new TestData();
            List<Order> orders = testData.GenerateRandomTestOrders(100);
            Console.WriteLine("Random test order batch generated (Batch length = " + orders.Count + ")");
            double totalTime = 0;
            Task[] mailTasks = new Task[orders.Count];
            for (int i = 0; i < orders.Count; i++)
            {
                Order curOrder = orders[i];
                var watch = System.Diagnostics.Stopwatch.StartNew();
                string pdfName = curOrder.id + ".pdf";
                mailTasks[i] = SendMail(pdfName, curOrder.client.name, ConfigurationManager.AppSettings["TestMail"], 
                    curOrder.id.ToString());
                watch.Stop();
                double oneClientTime = (double)watch.ElapsedMilliseconds / 1000;
                Console.WriteLine("Order " + curOrder.id + " processed in " + oneClientTime + " sec.");
                totalTime += oneClientTime;
            }
            Task.WaitAll(mailTasks);
            Console.WriteLine("Order batch processed!");
            Console.WriteLine("Total time elapsed: " + totalTime + " sec");
        }

        private static async Task SendMail(string pdfPath, string clientName, string recipientMail, string orderNum)
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("Placeholder Company", ConfigurationManager.AppSettings["UserMail"]));
            mailMessage.To.Add(new MailboxAddress(clientName, recipientMail));
            mailMessage.Subject = "Invoice For Order №" + orderNum;
            var attachment = new MimePart("application", "pdf")
            {
                Content = new MimeContent(File.OpenRead(pdfPath), ContentEncoding.Default),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName(pdfPath)
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
