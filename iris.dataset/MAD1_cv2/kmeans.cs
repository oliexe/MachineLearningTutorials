using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System.IO;

namespace MAD1_cv2
{
    public class kmeans
    {
        List<meansPoint> _rawDataToCluster = new List<meansPoint>();
        List<meansPoint> _normalizedDataToCluster = new List<meansPoint>();
        List<meansPoint> _clusters = new List<meansPoint>();
        private int _numberOfClusters = 0;
        PlotModel graf = new PlotModel();

        //Grafování rozložení x,y
        public void AddToGraph(List<double> x, List<double> y, byte r, byte g, byte b)
        {
            var scatterSeries = new ScatterSeries { MarkerType = MarkerType.Cross, MarkerStroke = OxyColor.FromRgb(r, g, b) };

            for (int i = 0; i < x.Count; i++)
            {
                scatterSeries.Points.Add(new ScatterPoint(x[i], y[i]));
            }

            graf.Series.Add(scatterSeries);
        }

        public void GenerateGraph()
        {
            using (var stream = File.Create("kmeans.pdf"))
            {
                var pdfExporter = new PdfExporter { Width = 1000, Height = 400 };
                pdfExporter.Export(graf, stream);
                Console.WriteLine("k-means graph generated!");
            }
        }

        public byte[] GetRandomColor()
        {
            Random r = new Random(DateTime.Now.Millisecond);

            return new byte[] { (byte)r.Next(0, 255), (byte)r.Next(0, 255), (byte)r.Next(0, 255) };
        }

        //Přidat data (Width X Lenght) do k-means
        public void InitData(List<double> Width, List<double> Lenght)
        {
            for (int i = 0; i < Lenght.Count(); i++)
            {
                _rawDataToCluster.Add(new meansPoint(Width[i], Lenght[i]));
            }
        }

        //Normalizace bodů
        private void NormalizeData()
        {
            double widthSum = 0.0;
            double lengthSum = 0.0;

            foreach (meansPoint dataPoint in _rawDataToCluster)
            {
                widthSum += dataPoint.Width;
                lengthSum += dataPoint.Length;
            }

            double widthMean = widthSum / _rawDataToCluster.Count;
            double lengthMean = lengthSum / _rawDataToCluster.Count;

            double sumWidth = 0.0;
            double sumLength = 0.0;
            foreach (meansPoint dataPoint in _rawDataToCluster)
            {
                sumWidth += Math.Pow(dataPoint.Width - widthMean, 2);
                sumLength += Math.Pow(dataPoint.Length- lengthMean, 2);
            }

            double widthSD = sumWidth / _rawDataToCluster.Count;
            double lengthSD = sumLength / _rawDataToCluster.Count;        
            foreach (meansPoint dataPoint in _rawDataToCluster)
            {
                _normalizedDataToCluster.Add(new meansPoint()
                {
                    Width = (dataPoint.Width - widthMean) / widthSD,
                    Length = (dataPoint.Length - lengthMean) / lengthSD
                }
                );
            }
        }

        //Nastavit počet clusetru
        private void SetClusters(int num)
        {
            _numberOfClusters = num;
        }

        //Helper.: Test zda má cluster alespoň jeden přiřazený bod
        private bool IsClusterEmpty(List<meansPoint> data)
        {
            var emptyCluster =
            data.GroupBy(s => s.Cluster).OrderBy(s => s.Key).Select(g => new { Cluster = g.Key, Count = g.Count() });

            foreach (var item in emptyCluster)
            {
                if (item.Count == 0)
                {
                    return true;
                }
            }
            return false;
        }

        //Inicializovat centroidy náhodně
        private void InitializeCentroids()
        {
            Random random = new Random(_numberOfClusters);

            //Přiřadime nahodný DataPoint zvolenému clusteru (Aspon jeden centroid má jeden DatatPoint)
            for (int i = 0; i < _numberOfClusters; ++i)
            {
                _normalizedDataToCluster[i].Cluster = _rawDataToCluster[i].Cluster = i;
            }

            //Zbytku DataPointu přiřadíme náhodné centroidy 
            //Nějaké "aribtary clusters" u kterých dojde k přehodnocení
            for (int i = _numberOfClusters; i < _normalizedDataToCluster.Count; i++)
            {
                _normalizedDataToCluster[i].Cluster = _rawDataToCluster[i].Cluster = random.Next(0, _numberOfClusters);
            }
        }

        //Nalezení nejvhodnějšího centroidu
        //Počítáme průměr pro cluster
        private bool UpdateDataPointMeans()
        {
            if (IsClusterEmpty(_normalizedDataToCluster)) return false;

            var groupToComputeMeans = _normalizedDataToCluster.GroupBy(s => s.Cluster).OrderBy(s => s.Key);

            int clusterIndex = 0;
            double width = 0.0;
            double length = 0.0;

            foreach (var item in groupToComputeMeans)
            {
                foreach (var value in item)
                {
                    width += value.Width;
                    length += value.Length;
                }

                _clusters[clusterIndex].Width = width / item.Count();
                _clusters[clusterIndex].Length = length / item.Count();

                clusterIndex++;
                width = 0.0;
                length = 0.0;
            }
            return true;
        }

        //Výpočet euclidovy vzdálenosti
        private double ElucidanDistance(meansPoint dataPoint, meansPoint mean)
        {
            double _diffs = 0.0;
            _diffs = Math.Pow(dataPoint.Width - mean.Width, 2);
            _diffs += Math.Pow(dataPoint.Length - mean.Length, 2);
            return Math.Sqrt(_diffs);
        }

        //Přesunout MeansPointy do správných clusterů
        private bool UpdateClusterMembership()
        {
            bool changed = false;

            double[] distances = new double[_numberOfClusters];

            for (int i = 0; i < _normalizedDataToCluster.Count; ++i)
            {

                for (int k = 0; k < _numberOfClusters; ++k)
                    distances[k] = ElucidanDistance(_normalizedDataToCluster[i], _clusters[k]);

                int newClusterId = MinIndex(distances);
                if (newClusterId != _normalizedDataToCluster[i].Cluster)
                {
                    changed = true;
                    _normalizedDataToCluster[i].Cluster = _rawDataToCluster[i].Cluster = newClusterId;

                    Console.WriteLine("Data Point >> Width: " + _rawDataToCluster[i].Width + ", Lenght: " +
                    _rawDataToCluster[i].Length + " moved to Cluster # " + newClusterId);
                }
            }
            if (changed == false)
                return false;
            if (IsClusterEmpty(_normalizedDataToCluster)) return false;
            return true;
        }

        //Helper.: Nejmenší hodnota v poli
        private int MinIndex(double[] distances)
        {
            int _indexOfMin = 0;
            double _smallDist = distances[0];
            for (int k = 0; k < distances.Length; ++k)
            {
                if (distances[k] < _smallDist)
                {
                    _smallDist = distances[k];
                    _indexOfMin = k;
                }
            }
            return _indexOfMin;
        }

        //Spustit k-means
        public void Execute(int NumberOfClusters, bool print)
        {
            Console.WriteLine();
            Console.WriteLine("K-means...");
            SetClusters(NumberOfClusters);

            for (int i = 0; i < _numberOfClusters; i++)
            {
                _clusters.Add(new meansPoint() { Cluster = i });
            }

            bool _changed = true;
            bool _success = true;

            NormalizeData();
            InitializeCentroids();
            

            int maxIteration = _normalizedDataToCluster.Count * 10;
            int _threshold = 0;
            while (_success == true && _changed == true && _threshold < maxIteration)
            {
                ++_threshold;
                _success = UpdateDataPointMeans();
                _changed = UpdateClusterMembership();
            }

            if (print)
            {
                var group = _rawDataToCluster.GroupBy(s => s.Cluster).OrderBy(s => s.Key);

                foreach (var g in group)
                {
                    Console.WriteLine("Cluster # " + g.Key + ":");

                    List<double> Wid = new List<double>();
                    List<double> Len = new List<double>();


                    foreach (var value in g)
                    {
                        Console.WriteLine(value.ToString());
                        Wid.Add(value.Width);
                        Len.Add(value.Length);
                    }

                    byte[] color = GetRandomColor();

                    AddToGraph(Wid, Len, color[0], color[1], color[2]);
                    Console.WriteLine("------------------------------");
                }
                GenerateGraph();
            }
        }
        
    }



    public class meansPoint
    {
        public double Width { get; set; }
        public double Length { get; set; }
        public int Cluster { get; set; }
        public meansPoint(double width, double lenght)
        {
            Width = width;
            Length = lenght;
            Cluster = 0;
        }

        public meansPoint()
        {

        }

        public override string ToString()
        {
            return string.Format("{{{0},{1}}}", Width.ToString("f" + 1), Length.ToString("f" + 1));
        }
    }

}
