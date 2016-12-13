using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.IO;

namespace REH0063_MAD1
{
    internal class Graph
    {
        private PlotModel graf = new PlotModel();

        //Grafování rozložení x,y
        public void AddToGraph(List<double> x, List<double> y, int ClusterNum, byte r, byte g, byte b, MarkerType marker)
        {
            var scatterSeries = new ScatterSeries { MarkerType = marker, MarkerStroke = OxyColor.FromRgb(r, g, b) };

            for (int i = 0; i < x.Count; i++)
            {
                scatterSeries.Points.Add(new ScatterPoint(x[i], y[i]));
            }

            graf.Series.Add(scatterSeries);
        }

        public void AddToGraph(List<double> x, List<double> y, int ClusterNum, byte r, byte g, byte b)
        {
            AddToGraph(x, y, ClusterNum, r, g, b, MarkerType.Circle);
        }

        public void GenerateGraph(string name)
        {
            var scatterSeries2 = new ScatterSeries { MarkerType = MarkerType.Diamond, MarkerStroke = OxyColor.FromRgb(0, 0, 0) };

            using (var stream = File.Create("Output/"+ name +"Clustering.pdf"))
            {
                var pdfExporter = new PdfExporter { Width = 1000, Height = 1000 };
                pdfExporter.Export(graf, stream);
            }
        }

        public byte[] GetRandomColor()
        {
            System.Threading.Thread.Sleep(50);
            Random r = new Random(DateTime.UtcNow.Millisecond);

            return new byte[] { (byte)r.Next(0, 255), (byte)r.Next(0, 255), (byte)r.Next(0, 255) };
        }
    }
}