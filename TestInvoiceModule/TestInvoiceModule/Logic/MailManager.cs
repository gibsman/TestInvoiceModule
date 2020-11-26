using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace TestInvoiceModule
{
    /// <summary>
    /// Sends generated invoice PDF files to clients
    /// </summary>
    public interface IMailManager
    {
        void SendMailBatch(List<Order> orders);
    }

    /// <summary>
    /// Class <see cref="MailManager`1"/> sends generated invoice PDF files to one predetermined email address (for test purposes).
    /// </summary>
    public class MailManager : IMailManager
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="MailManager`1"/> class.
        /// </summary>
        public MailManager()
        {

        }

        /// <summary>
        /// Sends all generated files to test email address asynchronously.
        /// </summary>
        /// <param name="orders">List of generated orders.</param>
        public void SendMailBatch(List<Order> orders)
        {
            Task[] mailTasks = new Task[orders.Count];
            for (int i = 0; i < orders.Count; i++)
            {
                mailTasks[i] = SendMail(orders[i].id.ToString(), orders[i].client.name, ConfigurationManager.AppSettings["TestMail"]);
            }
            Task.WaitAll(mailTasks);
        }

        /// <summary>
        /// Sends one generated PDF file to test mail address using SMTP client.
        /// </summary>
        /// <param name="orderNum">Order identifier.</param>
        /// <param name="clientName">Name of a client.</param>
        /// <param name="recipientMail">Test email address.</param>
        /// <returns></returns>
        private async Task SendMail(string orderNum, string clientName, string recipientMail)
        {
            string filename = orderNum + ".pdf";
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("File " + filename + " does not exist.");
            }
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("Placeholder Company", ConfigurationManager.AppSettings["UserMail"]));
            mailMessage.To.Add(new MailboxAddress(clientName, recipientMail));
            mailMessage.Subject = "Invoice For Order №" + orderNum;
            var pdfStream = File.OpenRead(filename);
            var attachment = new MimePart(MimeTypes.GetMimeType(filename))
            {
                Content = new MimeContent(pdfStream, ContentEncoding.Default),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName(filename)
            };
            var multipart = new Multipart("mixed");
            multipart.Add(attachment);
            mailMessage.Body = multipart;
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.mail.ru", 465, SecureSocketOptions.SslOnConnect);
                try
                {
                    await client.AuthenticateAsync(ConfigurationManager.AppSettings["UserMail"],
                        ConfigurationManager.AppSettings["Password"]);
                }
                catch
                {
                    throw new AuthenticationException("Authenticaction error. Please check your username and password.");
                }
                var options = FormatOptions.Default.Clone();
                try
                {
                    await client.SendAsync(options, mailMessage);
                }
                catch
                {
                    throw new Exception("SMTP client failed to send mail.");
                }
                await client.DisconnectAsync(true);
            }
            await pdfStream.DisposeAsync();
            logger.Debug("Invoice № {0} sent", orderNum);
        }
    }
}