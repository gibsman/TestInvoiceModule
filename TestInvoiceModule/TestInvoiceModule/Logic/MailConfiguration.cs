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

        }
    }
}
