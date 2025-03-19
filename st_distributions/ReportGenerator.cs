using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using st_distributions.Distributions;
using MigraDoc.DocumentObjectModel.Shapes;
using PdfSharp.Drawing;
using System.Drawing;
using XamlMath;
using WpfMath.Parsers;
using XamlMath.Exceptions;
using WpfMath;
using System.IO;
using System.Windows.Shapes;

namespace st_distributions
{
    class ReportGenerator
    {
        private static readonly string PdfFileName = $"{StatisticsManager.OutputFolderHist}/Lab1_UrtemeevSA_5030102_20101.pdf";
        private static readonly string PdfFileNameLab2 = $"{StatisticsManager.OutputFolderBox}/Lab2_UrtemeevSA_5030102_20101.pdf";

        public static void GeneratePdf(Dictionary<string, ReportDistributionInfo> info)
        {
            Document document = new();
            Section section = document.AddSection();
            section.PageSetup.Orientation = Orientation.Landscape;

            Paragraph mn = section.AddParagraph("Отчет по лабораторной работе 1");
            mn.Format.Font.Size = 24;
            mn.Format.Font.Bold = true;
            mn.Format.SpaceAfter = "10pt";

            Paragraph title = section.AddParagraph("Анализ распределений");
            title.Format.Font.Size = 18;
            title.Format.Font.Bold = true;
            title.Format.SpaceAfter = "10pt";

            foreach (var infoItem in info)
            {
                AddDistributionSection(section, infoItem);
            }

            string dir = $"{StatisticsManager.OutputFolderHist}/Statistics";
            string[] files = Directory.GetFiles(dir, "*.csv");
            foreach (var item in files)
            {
                Section tableSection = document.AddSection();
                AddStatisticsTable(tableSection, item);
            }

            PdfDocumentRenderer renderer = new()
            {
                Document = document
            };
            renderer.RenderDocument();
            renderer.PdfDocument.Save(PdfFileName);

            Console.WriteLine($"PDF-отчет сохранен: {PdfFileName}");
        }
        public static void GeneratePdfLab2()
        {
            Document document = new();

            Section section = document.AddSection();
            section.PageSetup.Orientation = Orientation.Landscape;

            Paragraph mn = section.AddParagraph("Отчет по лабораторной работе 2");
            mn.Format.Font.Size = 24;
            mn.Format.Font.Bold = true;
            mn.Format.SpaceAfter = "10pt";

            Paragraph title = section.AddParagraph("Бокс-плоты");
            title.Format.Font.Size = 18;
            title.Format.Font.Bold = true;
            title.Format.SpaceAfter = "10pt";

            string dir = $"{StatisticsManager.OutputFolderBox}";
            string[] files = Directory.GetFiles(dir, "*.png");
            foreach (var item in files)
            {
                //Section tableSection = document.AddSection();
                string fileName = System.IO.Path.GetFileNameWithoutExtension(item);
                section.AddParagraph(fileName).Format.Font.Size = 10;
                MigraDoc.DocumentObjectModel.Shapes.Image image = section.AddImage(item);
                image.Width = "12cm";
            }

            files = Directory.GetFiles(dir, "*.csv");
            foreach (var item in files)
            {
                //Section tableSection = document.AddSection();
                var ttl = section.AddParagraph("Количество выбросов");
                var table = section.AddTable();
                table.Borders.Width = 0.75;

                var lines = File.ReadAllLines(item);
                Console.WriteLine(lines.Length);
                if (lines.Length < 2) return;

                var headers = lines[0].Split(';');

                foreach (var header in headers)
                {
                    table.AddColumn("5cm");
                }

                Row headerRow = table.AddRow();
                headerRow.Shading.Color = Colors.LightGray;

                for (int i = 0; i < headers.Length; i++)
                {
                    headerRow.Cells[i].AddParagraph(headers[i]);
                    headerRow.Cells[i].Format.Font.Bold = true;
                    headerRow.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                    headerRow.Cells[i].VerticalAlignment = VerticalAlignment.Center;
                }

                foreach (var line in lines.Skip(1))
                {
                    var values = line.Split(';');
                    if (values.Length < headers.Length) continue;

                    Row row = table.AddRow();
                    row.TopPadding = 2;
                    row.BottomPadding = 2;

                    for (int i = 0; i < headers.Length; i++)
                    {
                        Paragraph paragraph = row.Cells[i].AddParagraph();

                        if (double.TryParse(values[i], out double number))
                        {
                            paragraph.AddText(Math.Round(number, 4).ToString("F4"));
                        }
                        else
                        {
                            paragraph.AddText(values[i]);
                        }

                        row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                        row.Cells[i].VerticalAlignment = VerticalAlignment.Center;
                        row.Cells[i].Format.Font.Size = 10;
                    }
                }
            }
            //var sgn = document.AddSection();
            section.AddParagraph("Уртемеев С.А.").Format.Alignment = ParagraphAlignment.Right;
            section.AddParagraph("5030102/20101").Format.Alignment = ParagraphAlignment.Right;


            PdfDocumentRenderer renderer = new()
            {
                Document = document
            };
            renderer.RenderDocument();
            renderer.PdfDocument.Save(PdfFileNameLab2);

            Console.WriteLine($"PDF-отчет сохранен: {PdfFileNameLab2}");
        }
        private static void AddDistributionSection(Section section, KeyValuePair<string, ReportDistributionInfo> infoItem)
        {
            Paragraph header = section.AddParagraph($"{infoItem.Key} Distribution");
            header.Format.Font.Size = 14;
            header.Format.Font.Bold = true;
            header.Format.SpaceBefore = "10pt";
            header.Format.SpaceAfter = "5pt";

            foreach (Tuple<int, string> sample in infoItem.Value.Samples)
            {
                string imgPath = sample.Item2;
                if (File.Exists(imgPath))
                {
                    MigraDoc.DocumentObjectModel.Shapes.Image image = section.AddImage(imgPath);
                    image.Width = "10cm";
                    section.AddParagraph($"Выборка: {sample.Item1} элементов").Format.Font.Size = 10;
                }
            }

            string latexFormula = infoItem.Value.LatexFormula;
            if (!string.IsNullOrEmpty(latexFormula))
            {
                string formulaPath = $"{StatisticsManager.OutputFolderHist}/{infoItem.Key}/{infoItem.Key}_formula.png";
                RenderLatexFormula(latexFormula, formulaPath);
                MigraDoc.DocumentObjectModel.Shapes.Image formulaImage = section.AddImage(formulaPath);
                formulaImage.Width = "5cm";
                section.AddParagraph().Format.SpaceAfter = "10pt";
            }
        }
        private static void RenderLatexFormula(string latex, string outputPath)
        {
            try
            {
                var parser = WpfTeXFormulaParser.Instance;
                var formula = parser.Parse(latex);
                var pngBytes = formula.RenderToPng(20.0, 0.0, 0.0, "Arial");
                File.WriteAllBytes(outputPath, pngBytes);
            }
            catch (TexException e)
            {
                Console.Error.WriteLine("Error when parsing formula: " + latex + "; " + e.Message);
            }
        }
        private static void AddStatisticsTable(Section section, string file)
        {
            if (!File.Exists(file)) return;

            string fileName = System.IO.Path.GetFileNameWithoutExtension(file);
            string[] parts = fileName.Split('_');
            if (parts.Length > 1)
            {
                string result = parts[1];
                section.AddParagraph(result).Format.Font.Size = 10;
            }

            var table = section.AddTable();
            table.Borders.Width = 0.75;

            var lines = File.ReadAllLines(file);
            if (lines.Length < 2) return;

            var headers = lines[0].Split(';');

            foreach (var header in headers)
            {
                table.AddColumn("5cm");
            }

            Row headerRow = table.AddRow();
            headerRow.Shading.Color = Colors.LightGray;

            for (int i = 0; i < headers.Length; i++)
            {
                headerRow.Cells[i].AddParagraph(headers[i]);
                headerRow.Cells[i].Format.Font.Bold = true;
                headerRow.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                headerRow.Cells[i].VerticalAlignment = VerticalAlignment.Center;
            }

            foreach (var line in lines.Skip(1))
            {
                var values = line.Split(';');
                if (values.Length < headers.Length) continue;

                Row row = table.AddRow();
                row.TopPadding = 2;
                row.BottomPadding = 2;

                for (int i = 0; i < headers.Length; i++)
                {
                    Paragraph paragraph = row.Cells[i].AddParagraph();

                    if (double.TryParse(values[i], out double number))
                    {
                        paragraph.AddText(Math.Round(number, 4).ToString("F4"));
                    }
                    else
                    {
                        paragraph.AddText(values[i]);
                    }

                    row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                    row.Cells[i].VerticalAlignment = VerticalAlignment.Center;
                    row.Cells[i].Format.Font.Size = 10;
                }
            }
        }

    }
}
