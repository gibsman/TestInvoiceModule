using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using NLog;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TestInvoiceModule
{
    public interface IInvoiceGenerator
    {
        void Generate(Order order);
    }

    public class InvoiceGenerator : IInvoiceGenerator
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public InvoiceGenerator()
        {

        }

        public void Generate(Order order)
        {
            string filename = order.id + ".pdf";
            if (File.Exists(filename))
            {
                FileInfo info = new FileInfo(filename);
                if (info.IsReadOnly)
                {
                    throw new UnauthorizedAccessException("File " + filename + " already exists and is read only.");
                }
            }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Document invoice = new Document();
            invoice.Info.Title = "Invoice № " + order.id;
            invoice.Info.Subject = "Invoice for several items purchased from Placeholder Company";
            invoice.Info.Author = "Placeholder Company";
            DefineStyles(invoice);
            AddCompanyInfo(invoice);
            AddClientInfo(invoice, order.client);
            TextFrame orderInfoFrame = AddOrderInfo(order.id.ToString(), order.orderDate.ToString("dd.MM.yyyy"));
            TextFrame tableFrame = AddProductsTable(order.orderProducts);
            PlaceOrderInfoAndProductsTableOnSameLine(invoice, orderInfoFrame, tableFrame, order.totalAmount.ToString());
            AddTerms(invoice, order.dueDate.ToString("dd.MM.yyyy"));
            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true)
            {
                Document = invoice
            };
            renderer.RenderDocument();
            DrawRectangles(renderer.PdfDocument);
            renderer.PdfDocument.Save(filename);
            logger.Debug("Invoice № {0} file saved", order.id);
        }
        private void DefineStyles(Document invoice)
        {
            //"Normal" defines default style
            Style style = invoice.Styles["Normal"];
            style.Font.Name = "Arial";
            style.Font.Size = 11;
            style.ParagraphFormat.SpaceAfter = 1.2;

            style = invoice.Styles.AddStyle("MainHeader", "Normal");
            style.Font.Size = 40;
            style.Font.Color = new Color(78, 83, 92);

            style = invoice.Styles.AddStyle("SectionHeader", "Normal");
            style.ParagraphFormat.SpaceBefore = 20;
            style.Font.Size = 10;
            style.Font.Bold = true;
            style.Font.Color = new Color(153, 153, 153);

            style = invoice.Styles.AddStyle("TableContent", "Normal");
            style.Font.Bold = true;
            style.Font.Color = Colors.White;

            style = invoice.Styles.AddStyle("ClientPadding", "Normal");
            style.ParagraphFormat.SpaceAfter = 20;
        }

        private void AddCompanyInfo(Document invoice)
        {
            invoice.AddSection();
            invoice.LastSection.AddParagraph("INVOICE", "MainHeader");

            Paragraph companyName = new Paragraph();
            companyName.Format.SpaceBefore = 10;
            companyName.Format.SpaceAfter = 10;
            companyName.AddText("Placeholder Company");
            companyName.Format.Font.Size = 20;
            invoice.LastSection.Add(companyName);

            //places two paragraphs on the same line
            Table table = new Table();
            table.AddColumn(Unit.FromCentimeter(4));
            table.AddColumn(Unit.FromCentimeter(4));
            Row row = table.AddRow();
            row[0].AddParagraph("12 Threefour st, New Bork");
            row[1].AddParagraph("123-456-7890");
            row[1].AddParagraph("placeholderco@net.com");
            invoice.LastSection.Add(table);
        }

        private void AddClientInfo(Document invoice, Client client)
        {
            invoice.LastSection.AddParagraph("BILLED TO", "SectionHeader");
            invoice.LastSection.AddParagraph(client.name);
            invoice.LastSection.AddParagraph(client.mail);
            invoice.LastSection.AddParagraph(client.address);
            invoice.LastSection.AddParagraph(client.phone, "ClientPadding");
        }

        private void PlaceOrderInfoAndProductsTableOnSameLine(Document invoice, TextFrame orderInfoFrame,
            TextFrame tableFrame, string totalAmount)
        {
            Table table = new Table();
            table.AddColumn(Unit.FromCentimeter(4));
            table.AddColumn(Unit.FromCentimeter(20));
            Row row = table.AddRow();
            row[0].Add(orderInfoFrame);
            row[1].Add(tableFrame);
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

        private TextFrame AddOrderInfo(string orderId, string orderDate)
        {
            TextFrame orderInfoFrame = new TextFrame();
            Paragraph invoiceNumPar = new Paragraph();
            invoiceNumPar.Style = "SectionHeader";
            invoiceNumPar.Format.SpaceBefore = 0;
            invoiceNumPar.AddText("INVOICE NUMBER");
            orderInfoFrame.Add(invoiceNumPar);
            orderInfoFrame.AddParagraph(orderId);
            Paragraph invoiceOrdDate = new Paragraph();
            invoiceOrdDate.Style = "SectionHeader";
            invoiceOrdDate.AddText("DATE OF ISSUE");
            orderInfoFrame.Add(invoiceOrdDate);
            orderInfoFrame.AddParagraph(orderDate);
            return orderInfoFrame;
        }

        private TextFrame AddProductsTable(List<OrderProduct> orderProducts)
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
            row.Shading.Color = Colors.DodgerBlue;
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
            TextFrame tableFrame = new TextFrame();
            tableFrame.Height = Unit.FromCentimeter(0.8) * table.Rows.Count;
            tableFrame.Add(table);
            return tableFrame;
        }

        private void AddTerms(Document invoice, string dueDate)
        {
            TextFrame termFrame = new TextFrame();
            Paragraph termParagraph = new Paragraph();
            termParagraph.AddFormattedText("TERMS\n", "SectionHeader");
            termParagraph.AddText("E.g. please pay invoice by " + dueDate);
            termFrame.Add(termParagraph);
            termFrame.Width = 400;
            termFrame.WrapFormat.DistanceTop = 700;
            termFrame.RelativeVertical = RelativeVertical.Page;
            termFrame.WrapFormat.Style = WrapStyle.Through;
            invoice.LastSection.Add(termFrame);
        }

        private static void DrawRectangles(PdfDocument document)
        {
            int recHeight = 35;
            int margin = 10;
            for (int i = 0; i < document.PageCount; i++)
            {
                PdfPage page = document.Pages[i];
                using (XGraphics gfx = XGraphics.FromPdfPage(page))
                {
                    gfx.DrawRectangle(XBrushes.DodgerBlue, 0, margin, page.Width, recHeight);
                    gfx.DrawRectangle(XBrushes.DodgerBlue, 0, page.Height - margin - recHeight, page.Width, recHeight);
                }
            }
        }
    }
}
