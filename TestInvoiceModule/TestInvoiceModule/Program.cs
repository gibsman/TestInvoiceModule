using System;
using System.Configuration;
using MimeKit;
using System.IO;
using MailKit.Net.Smtp;
using MailKit.Security;
using Spire.Doc;
using Spire.Doc.Documents;

namespace TestInvoiceModule
{
    class Program
    {
        static void Main(string[] args)
        {
            SmtpClient client = InitializeClient();
            Document invoiceDoc = InitializeInvoice();
            for (int id = 0; id < 10; id++)
            {
                ConvertDocToPdf(invoiceDoc, id.ToString());
                var watch = System.Diagnostics.Stopwatch.StartNew();
                string pdfName = id + ".pdf";
                SendMail(pdfName, ConfigurationManager.AppSettings["TestMail"], client);
                watch.Stop();
                Console.WriteLine((double)watch.ElapsedMilliseconds / 1000);
                File.Delete(pdfName);
            }
            client.Disconnect(true);
            Console.WriteLine("DONE");
        }

        private static Document InitializeInvoice()
        {
            Document document = new Document();
            document.LoadFromFile("invoice_template.docx");
            return document;
        }

        private static void ConvertDocToPdf(Document originalInvoice, string id)
        {
            Document invoiceCopy = originalInvoice.Clone();
            invoiceCopy.Replace("order_id", id, true, true);
            invoiceCopy.SaveToFile(id + ".PDF", FileFormat.PDF);
        }

        private static SmtpClient InitializeClient()
        {
            var client = new SmtpClient();
            client.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
            client.Authenticate(ConfigurationManager.AppSettings["UserMail"], 
                ConfigurationManager.AppSettings["Password"]);
            return client;
        }

        private static void SendMail(string pdfPath, string recipientMail, SmtpClient client)
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("Placeholder Company", ConfigurationManager.AppSettings["UserMail"]));
            mailMessage.To.Add(new MailboxAddress("Placeholder Guy", recipientMail));
            mailMessage.Subject = "Invoce For Order";
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

            var options = FormatOptions.Default.Clone();
            client.Send(options, mailMessage);
        }
    }
}
