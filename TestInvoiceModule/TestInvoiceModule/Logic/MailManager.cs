using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TestInvoiceModule
{
    public static class MailManager
    {
        public static void SendMailBatch(List<Order> orders)
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
            string filename = orderNum + ".pdf";
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("File " + filename + " does not exist.");
            }
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("Placeholder Company", ConfigurationManager.AppSettings["UserMail"]));
            mailMessage.To.Add(new MailboxAddress(clientName, recipientMail));
            mailMessage.Subject = "Invoice For Order №" + orderNum;
            var attachment = new MimePart("application", "pdf")
            {
                Content = new MimeContent(File.OpenRead(filename), ContentEncoding.Default),
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
                await client.SendAsync(options, mailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
