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

namespace st_distributions
{
    class ReportGenerator
    {
        private static readonly string PdfFileName = $"{StatisticsManager.OutputFolder}/Report.pdf";

        public static void GeneratePdf(Dictionary<string, ReportDistributionInfo> info)
        {
            Document document = new();
            Section section = document.AddSection();
            section.PageSetup.Orientation = Orientation.Landscape;

            Paragraph title = section.AddParagraph("Анализ распределений");
            title.Format.Font.Size = 18;
            title.Format.Font.Bold = true;
            title.Format.SpaceAfter = "10pt";

            foreach (var infoItem in info)
            {
                AddDistributionSection(section, infoItem);
            }

            AddStatisticsTable(section);

            PdfDocumentRenderer renderer = new()
            {
                Document = document
            };
            renderer.RenderDocument();
            renderer.PdfDocument.Save(PdfFileName);

            Console.WriteLine($"PDF-отчет сохранен: {PdfFileName}");
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
                string formulaPath = $"{StatisticsManager.OutputFolder}/{infoItem.Key}/{infoItem.Key}_formula.png";
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

        private static void AddStatisticsTable(Section section)
        {
            string csvPath = $"{StatisticsManager.OutputFolder}/Statistics_1000.csv";
            if (!File.Exists(csvPath)) return;

            var table = section.AddTable();
            table.Borders.Width = 0.75;

            // Читаем все строки CSV-файла
            var lines = File.ReadAllLines(csvPath);
            if (lines.Length < 2) return; // Если в файле нет данных, выходим

            // Читаем заголовки из первой строки
            var headers = lines[0].Split(';');

            // Добавляем столбцы (ширина будет динамической)
            foreach (var header in headers)
            {
                table.AddColumn("3.5cm");
            }

            // Создаём заголовок таблицы
            Row headerRow = table.AddRow();
            headerRow.Shading.Color = Colors.LightGray;

            for (int i = 0; i < headers.Length; i++)
            {
                headerRow.Cells[i].AddParagraph(headers[i]); // Устанавливаем заголовок из CSV
                headerRow.Cells[i].Format.Font.Bold = true;
                headerRow.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                headerRow.Cells[i].VerticalAlignment = VerticalAlignment.Center;
            }

            // Читаем и заполняем строки таблицы, начиная со второй строки CSV
            foreach (var line in lines.Skip(1))
            {
                var values = line.Split(';');
                if (values.Length < headers.Length) continue; // Пропускаем строки с недостаточными данными

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
                        paragraph.AddText(values[i]); // Если текст, просто добавляем его
                    }

                    // Форматирование ячеек для лучшего отображения
                    row.Cells[i].Format.Alignment = ParagraphAlignment.Center;
                    row.Cells[i].VerticalAlignment = VerticalAlignment.Center;
                    row.Cells[i].Format.Font.Size = 10; // Уменьшаем шрифт, чтобы избежать выхода текста
                }
            }
        }

    }
}
