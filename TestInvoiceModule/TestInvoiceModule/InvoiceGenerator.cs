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
    public static class InvoiceGenerator
    {
        public static void Generate(Order order)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Document invoice = new Document();
            invoice.Info.Title = "Invoice №" + order.id;
            invoice.Info.Subject = "Invoice for several items purchased from Placeholder Company";
            invoice.Info.Author = "Placeholder Company";
            DefineStyles(invoice);
            AddCompanyInfo(invoice);
            AddClientInfo(invoice, order.client);
            AddProductsTable(invoice, order.orderProducts, order.totalAmount.ToString());
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true)
            {
                Document = invoice
            };
            renderer.RenderDocument();
            string filename = order.id + ".pdf";
            renderer.PdfDocument.Save(filename);
        }
        private static void DefineStyles(Document invoice)
        {
            //"Normal" defines default style
            Style style = invoice.Styles["Normal"];
            style.Font.Name = "Arial";
            style.Font.Size = 9;

            style = invoice.Styles.AddStyle("MainHeader", "Normal");
            style.Font.Size = 28;
            style.Font.Color = new Color(78, 83, 92);

            style = invoice.Styles.AddStyle("SectionHeader", "Normal");
            style.Font.Bold = true;
            style.Font.Color = new Color(153, 153, 153);

            style = invoice.Styles.AddStyle("TableContent", "Normal");
            style.Font.Bold = true;
            style.Font.Color = Colors.White;
        }


        private static void AddCompanyInfo(Document invoice)
        {
            invoice.AddSection();
            invoice.LastSection.AddParagraph("INVOICE", "MainHeader");

            Paragraph companyName = new Paragraph();
            companyName.AddText("Placeholder Company");
            companyName.Format.Font.Size = 20;
            invoice.LastSection.Add(companyName);

            invoice.LastSection.AddParagraph("12 Threefour st, New Bork");
            invoice.LastSection.AddParagraph("123-456-7890");
            invoice.LastSection.AddParagraph("placeholderco@net.com");

        }

        private static void AddClientInfo(Document invoice, Client client)
        {
            invoice.LastSection.AddParagraph("BILLED TO", "SectionHeader");
            invoice.LastSection.AddParagraph(client.name);
            invoice.LastSection.AddParagraph(client.mail);
            invoice.LastSection.AddParagraph(client.address);
            invoice.LastSection.AddParagraph(client.phone);
        }

        private static void AddOrderInfo(Document invoice)
        {

        }

        private static void AddProductsTable(Document invoice, List<OrderProduct> orderProducts, string totalAmount)
        {
            Table table = new Table();
            table.Format.Font.Size = 10;
            table.TopPadding = Unit.FromCentimeter(0.2);
            table.Shading.Color = new Color(243, 243, 243);
            table.Borders.Width = 0.5;

            table.AddColumn(Unit.FromCentimeter(6));
            table.AddColumn(Unit.FromCentimeter(2.5));
            table.AddColumn(Unit.FromCentimeter(2.5));
            table.AddColumn(Unit.FromCentimeter(2.5));

            Row row = table.AddRow();
            row.Style = "TableContent";
            row.Shading.Color = new Color(13, 131, 221);
            row.Height = Unit.FromCentimeter(0.8);
            string[] tableHeaders = new string[] { "DESCRIPTION", "UNIT COST", "QUANTITY", "AMOUNT" };
            for (int i = 0; i < tableHeaders.Length; i++)
            {
                row.Cells[i].AddParagraph(tableHeaders[i]);
            }
            for (int i = 0; i < orderProducts.Count; i++)
            {
                row = table.AddRow();
                row.Height = Unit.FromCentimeter(0.7);
                row.Cells[0].AddParagraph(orderProducts[i].product.name);
                row.Cells[1].AddParagraph("$" + orderProducts[i].product.unitCost.ToString());
                row.Cells[2].AddParagraph(orderProducts[i].quantity.ToString());
                row.Cells[3].AddParagraph("$" + orderProducts[i].totalAmount.ToString());
            }
            table.Rows.LeftIndent = 100;

            invoice.LastSection.Add(table);
            Paragraph invoiceTotalPar = new Paragraph();
            invoiceTotalPar.Format.Alignment = ParagraphAlignment.Right;
            invoiceTotalPar.Style = "SectionHeader";
            invoiceTotalPar.AddText("INVOICE TOTAL");
            invoice.LastSection.Add(invoiceTotalPar);
            Paragraph totalNumPar = new Paragraph();
            totalNumPar.Format.Alignment = ParagraphAlignment.Right;
            totalNumPar.Format.Font.Size = 18;
            totalNumPar.AddText("$" + totalAmount);
            invoice.LastSection.Add(totalNumPar);
        }
    }
}
