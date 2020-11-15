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

namespace TestInvoiceModule
{
    class Program
    {
        static void Main(string[] args)
        {
            SmtpClient client = InitializeSmtpClient();
            Document invoiceDoc = InitializeInvoiceDocument();
            ProcessOrders(client, invoiceDoc);
        }

        private static SmtpClient InitializeSmtpClient()
        {
            var client = new SmtpClient();
            client.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
            client.Authenticate(ConfigurationManager.AppSettings["UserMail"],
                ConfigurationManager.AppSettings["Password"]);
            return client;
        }

        private static Document InitializeInvoiceDocument()
        {
            Document document = new Document();
            document.LoadFromFile("invoice_template.docx");
            return document;
        }

        private static void ProcessOrders(SmtpClient client, Document invoiceDoc)
        {
            double totalTime = 0;
            for (int i = 0; i < 100; i++)
            {
                //FillInInvoice(invoiceDoc, curOrder);
                var watch = System.Diagnostics.Stopwatch.StartNew();
                string pdfName = i + ".pdf";
                SendMail(pdfName, ConfigurationManager.AppSettings["TestMail"], client);
                watch.Stop();
                double oneClientTime = (double)watch.ElapsedMilliseconds / 1000;
                Console.WriteLine("Order " + i + " processed in " + oneClientTime + " sec. Waiting 0.5 seconds...");
                Thread.Sleep(500);
                totalTime += oneClientTime + 1;
            }
            client.Disconnect(true);
            Console.WriteLine("All clients processed!");
            Console.WriteLine("Total time elapsed: " + totalTime + " sec");
        }

        private static void FillInInvoice(Document originalInvoice, Order order)
        {
            Document invoiceCopy = originalInvoice.Clone();

            invoiceCopy.Replace("order_id", order.id.ToString(), true, true);
            invoiceCopy.Replace("order_date", order.orderDate.ToString(), true, true);
            invoiceCopy.Replace("order_due_date", order.dueDate.ToString(), true, true);
            invoiceCopy.Replace("total_amount", "$ " + order.totalAmount.ToString(), true, true);

            Client client = order.client;
            invoiceCopy.Replace("client_name", client.name, true, true);
            invoiceCopy.Replace("client_mail", client.mail, true, true);
            invoiceCopy.Replace("client_address", client.address, true, true);
            invoiceCopy.Replace("client_phone", client.phone, true, true);

            List<OrderProduct> orderProducts = order.orderProducts;
            for (int i = 0; i < 9; i++)
            {
                string productName = "";
                string productUnitCost = "";
                string productQuantity = "";
                string productTotalAmount= "";
                if (i < orderProducts.Count)
                {
                    OrderProduct orderProduct = orderProducts[i];
                    productName = orderProduct.product.name;
                    productUnitCost = "$ " + orderProduct.product.unitCost.ToString();
                    productQuantity = orderProduct.quantity.ToString();
                    productTotalAmount = "$ " + orderProduct.totalAmount.ToString();
                }
                invoiceCopy.Replace("product_name" + (i + 1), productName, true, true);
                invoiceCopy.Replace("product_ucost" + (i + 1), productUnitCost, true, true);
                invoiceCopy.Replace("product_qty" + (i + 1), productQuantity, true, true);
                invoiceCopy.Replace("product_ocost" + (i + 1), productTotalAmount, true, true);
            }
            invoiceCopy.SaveToFile(order.id + ".PDF", FileFormat.PDF);
        }

        private static void SendMail(string pdfPath, string recipientMail, SmtpClient client)
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("Placeholder Company", ConfigurationManager.AppSettings["UserMail"]));
            mailMessage.To.Add(new MailboxAddress("Placeholder Guy", recipientMail));
            mailMessage.Subject = "Invoice For Order";
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
