using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestInvoiceModule
{
    public class InvoiceGenerator
    {
        Document templateDoc;

        public InvoiceGenerator()
        {
            CreateTemplate();
        }

        private void CreateTemplate()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            templateDoc = new Document();
            templateDoc.Info.Subject = "Invoice for several items purchased from Placeholder Company";
            templateDoc.Info.Author = "Placeholder Company";
        }
    }
}
