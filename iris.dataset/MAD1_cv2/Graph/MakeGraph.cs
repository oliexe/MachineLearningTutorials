using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MAD.Graph
{
    internal class MakeGraph
    {
        /// <summary>
        /// "helper" funtion to generate CDF,PDF and histogram for a list.
        /// </summary>
        public void CDFPDF(List<double> source, string filename)
        {
            Operations functions = new Operations();

            int count = 0;
            PlotModel graf = new PlotModel { Title = "Histogram " + filename };
            PlotModel graf_cdf = new PlotModel { Title = "CDF " + filename };
            PlotModel graf_pdf = new PlotModel { Title = "PDF " + filename };

            List<double> workData = new List<double>();

            foreach (var petal in source)
            {
                workData.Add(petal);
            }

            workData.Sort();

            double[] values = workData.ToArray();
            var bucketeer = new Dictionary<double, double>();
            var groups = workData.GroupBy(i => i);
            List<string> kek = new List<string>();

            var overlayData = new LineSeries();
            var PDF = new LineSeries();

            int z = 0;
            foreach (var iris in groups)
            {
                bucketeer.Add(iris.Key, iris.Count());
                kek.Add(iris.Key.ToString());
                count = count + iris.Count();
                z++;

                overlayData.Points.Add(new DataPoint(z, count / 1.5));
                PDF.Points.Add(new DataPoint(z, functions.PDF(functions.CDF(iris.Key, functions.Average(workData), functions.StandartDeviation(workData)))));
            }

            ColumnSeries ColSer = new ColumnSeries();
            CategoryAxis Axis = new CategoryAxis();

            foreach (var pair in bucketeer.OrderBy(x => x.Key))
            {
                ColSer.Items.Add(new ColumnItem(pair.Value));
            }

            Axis.ItemsSource = kek;
            CategoryAxis Axis2 = new CategoryAxis();
            Axis2.ItemsSource = kek;
            CategoryAxis Axis3 = new CategoryAxis();
            Axis3.ItemsSource = kek;

            graf.Series.Add(ColSer);
            graf.Axes.Add(Axis);

            graf_cdf.Series.Add(overlayData);
            graf_cdf.Axes.Add(Axis2);

            graf_pdf.Series.Add(PDF);
            graf_pdf.Axes.Add(Axis3);

            using (var stream = File.Create(filename + "_histogram.pdf"))
            {
                var pdfExporter = new PdfExporter { Width = 1000, Height = 400 };
                pdfExporter.Export(graf, stream);
                Console.WriteLine("Graph " + filename + "_histogram.pdf" + " generated!");
            }

            using (var stream = File.Create(filename + "_CDF.pdf"))
            {
                var pdfExporter = new PdfExporter { Width = 1000, Height = 400 };
                pdfExporter.Export(graf_cdf, stream);
                Console.WriteLine("Graph " + filename + "_CDF.pdf" + " generated!");
            }

            using (var stream = File.Create(filename + "_PDF.pdf"))
            {
                var pdfExporter = new PdfExporter { Width = 1000, Height = 400 };
                pdfExporter.Export(graf_pdf, stream);
                Console.WriteLine("Graph " + filename + "_PDF.pdf" + " generated!");
            }
        }
    }
}