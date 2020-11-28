# Test Invoice Module
This module is used for generating multiple random invoices in form of .pdf files and sending them to specified recipient mail address. 
PDF files are created in the folder with module executable and deleted after mailing. An example of generated invoice is shown below:
<p align="center">
  <img style="box-shadow:10px;" src="../master/TestInvoiceModule/TestInvoiceModule/Images/sample.png" width="450" title="Sample invoice">
</p>

# Usage:
(number) - generates a number of random invoices in the current folder, after which sends invoices to unspecified test mail address and then deletes them from the folder.

--h - displays help information.

Recipient mail address, as well as sender's mail address and password must be provided through assignment of the following environmental variables:
* CLIENT_MAIL - recipient email address.
* SMTP_USER_NAME - sender email address.
* SMTP_PASSWORD - password for sender email address.

For more information on how to set up environmental variables via Windows Control Panel visit following page: 
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-5.0#windows

# Credits

This module uses the following open source packages:
- [MailKit](https://github.com/jstedfast/MailKit)
- [MigraDoc](http://www.pdfsharp.net/migradocoverview.ashx)
- [NLog](https://nlog-project.org/)
- [Ninject](http://www.ninject.org/)
