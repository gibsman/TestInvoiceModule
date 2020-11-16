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
            AddProductsTable(invoice, order.orderProducts);
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true)
            {
                Document = invoice
            };
            renderer.RenderDocument();
            string filename = order.id + ".pdf";
            renderer.PdfDocument.Save(filename);
        }

        private static void AddCompanyInfo(Document invoice)
        {

        }

        private static void AddClientInfo(Document invoice)
        {

        }
        private static void AddOrderInfo(Document invoice)
        {

        }

        private static void AddProductsTable(Document invoice, List<OrderProduct> orderProducts)
        {
            Table table = new Table();
            table.Format.Font = new Font("Arial", Unit.FromPica(0.8));
            table.TopPadding = Unit.FromCentimeter(0.2);
            table.Shading.Color = new Color(243, 243, 243);
            table.Borders.Width = 0.5;

            table.AddColumn(Unit.FromCentimeter(6));
            table.AddColumn(Unit.FromCentimeter(2.5));
            table.AddColumn(Unit.FromCentimeter(2.5));
            table.AddColumn(Unit.FromCentimeter(2.5));

            Row row = table.AddRow();
            row.Shading.Color = new Color(13, 131, 221);
            row.Format.Font = new Font("Arial", Unit.FromPica(0.75));
            row.Format.Font.Bold = true;
            row.Format.Font.Color = Colors.White;
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

            invoice.AddSection();
            invoice.LastSection.Add(table);
        }
    }
}
