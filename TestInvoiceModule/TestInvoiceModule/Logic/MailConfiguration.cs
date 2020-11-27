using System;

namespace TestInvoiceModule
{
    public interface IMailConfiguration
    {
        string ClientMail { get; set; }

        string SmtpUserName { get; set; }

        string SmtpPassword { get; set; }
    }

    class MailConfiguration : IMailConfiguration
    {
        public string ClientMail { get; set; }

        public string SmtpUserName { get; set; }

        public string SmtpPassword { get; set; }

        public MailConfiguration()
        {
            ClientMail = Environment.GetEnvironmentVariable("CLIENT_MAIL", EnvironmentVariableTarget.Machine);
            SmtpUserName = Environment.GetEnvironmentVariable("SMTP_USER_NAME", EnvironmentVariableTarget.Machine);
            SmtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD", EnvironmentVariableTarget.Machine);
        }
    }
}
