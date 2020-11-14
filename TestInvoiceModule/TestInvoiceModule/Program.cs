using System;
using System.Configuration;
using MimeKit;
using System.IO;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace TestInvoiceModule
{
    class Program
    {
        static void Main(string[] args)
        {
            SmtpClient client = InitializeClient();
            for (int i = 0; i < 10; i++)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                SendMail("starkovmaximdev@gmail.com", client);
                watch.Stop();
                Console.WriteLine((double)watch.ElapsedMilliseconds / 1000);
            }
            client.Disconnect(true);
            Console.WriteLine("DONE");
        }

        private static SmtpClient InitializeClient()
        {
            var client = new SmtpClient();
            client.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
            client.Authenticate(ConfigurationManager.AppSettings["UserMail"], 
                ConfigurationManager.AppSettings["Password"]);
            return client;
        }

        private static void SendMail(string recipientMail, SmtpClient client)
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("Starkov Maxim", ConfigurationManager.AppSettings["UserMail"]));
            mailMessage.To.Add(new MailboxAddress("Placeholder Guy", recipientMail));
            mailMessage.Subject = "Test Message";
            string path = AppDomain.CurrentDomain.BaseDirectory + "blank.pdf";
            var attachment = new MimePart("application", "pdf")
            {
                Content = new MimeContent(File.OpenRead(path), ContentEncoding.Default),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName(path)
            };
            var multipart = new Multipart("mixed");
            multipart.Add(attachment);
            mailMessage.Body = multipart;

            var options = FormatOptions.Default.Clone();
            client.Send(options, mailMessage);
        }
    }
}
