using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.IO;

namespace REH0063_MAD1
{
    internal class Graph
    {
        private PlotModel _graph = new PlotModel();

        /// <summary>
        /// Adding of points into graph
        /// </summary>
        public void AddToGraph(List<double> x, List<double> y, int ClusterNum, byte r, byte g, byte b, MarkerType marker)
        {
            var scatterSeries = new ScatterSeries { MarkerType = marker, MarkerStroke = OxyColor.FromRgb(r, g, b) };

            for (int i = 0; i < x.Count; i++)
            {
                scatterSeries.Points.Add(new ScatterPoint(x[i], y[i]));
            }

            _graph.Series.Add(scatterSeries);
        }
        public void AddToGraph(List<double> x, List<double> y, int ClusterNum, byte r, byte g, byte b)
        {
            AddToGraph(x, y, ClusterNum, r, g, b, MarkerType.Circle);
        }

        /// <summary>
        /// Generate PDF out of this instance of graph (800x600)
        /// </summary>
        public void GenerateGraph(string name)
        {
            var scatterSeries2 = new ScatterSeries { MarkerType = MarkerType.Diamond, MarkerStroke = OxyColor.FromRgb(0, 0, 0) };

            using (var stream = File.Create("Output/" + name + "Clustering.pdf"))
            {
                var pdfExporter = new PdfExporter { Width = 800, Height = 600 };
                pdfExporter.Export(_graph, stream);
            }
        }

        /// <summary>
        /// Get random color RGB for cluster separation
        /// </summary>
        public byte[] GetRandomColor()
        {
            System.Threading.Thread.Sleep(50);
            Random r = new Random(DateTime.UtcNow.Millisecond);

            return new byte[] { (byte)r.Next(0, 255), (byte)r.Next(0, 255), (byte)r.Next(0, 255) };
        }
    }
}