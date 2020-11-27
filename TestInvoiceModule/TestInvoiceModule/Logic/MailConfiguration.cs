using System;

namespace TestInvoiceModule
{
    /// <summary>
    /// Contains mailing information.
    /// </summary>
    public interface IMailConfiguration
    {
        string ClientMail { get; set; }

        string SmtpUserName { get; set; }

        string SmtpPassword { get; set; }
    }

    /// <summary>
    /// Class <see cref="MailConfiguration`1"/> contains properties with information for sending mail.
    /// </summary>
    class MailConfiguration : IMailConfiguration
    {
        public string ClientMail { get; set; }

        public string SmtpUserName { get; set; }

        public string SmtpPassword { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MailConfiguration`1"/> class 
        /// by loading environment variables into properties.
        /// </summary>
        public MailConfiguration()
        {
            ClientMail = Environment.GetEnvironmentVariable("CLIENT_MAIL", EnvironmentVariableTarget.Machine);
            SmtpUserName = Environment.GetEnvironmentVariable("SMTP_USER_NAME", EnvironmentVariableTarget.Machine);
            SmtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD", EnvironmentVariableTarget.Machine);
        }
    }
}
