using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace MAD1_cv2
{
    class Program
    {
        static void Main(string[] args)
        {
            var reader = new StreamReader(File.OpenRead(@"C:\iris.csv"));
            List<Iris> list = new List<Iris>();
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                line = line.Replace(",", ".");
                var values = line.Split(';');
                Iris result_line = new Iris(Convert.ToDouble(values[0]), Convert.ToDouble(values[1]), Convert.ToDouble(values[2]), Convert.ToDouble(values[3]), values[4]);
                list.Add(result_line);
            }

            List<double> sepallen_list = new List<double>();
            List<double> sepalwid_list = new List<double>();
            List<double> petallen_list = new List<double>();
            List<double> petalwid_list = new List<double>();

            foreach (Iris line in list)
            {
                sepallen_list.Add(line.sepallen);
                sepalwid_list.Add(line.sepalwid);
                petallen_list.Add(line.petallen);
                petalwid_list.Add(line.petalwid);
            }

            System.IO.Directory.CreateDirectory("sepal_lenght");
            System.IO.Directory.CreateDirectory("sepal_width");
            System.IO.Directory.CreateDirectory("petal_lenght");
            System.IO.Directory.CreateDirectory("petal_width");

            Console.WriteLine("SEPAL LENGTH");           
            Console.WriteLine("Average: " + GetAverage(sepallen_list));
            Console.WriteLine("Variance: " + GetVariance(sepallen_list));
            Console.WriteLine("Deviation: " + GetStdDev(sepallen_list));
            Console.WriteLine("Median: " + GetMedian(sepallen_list));
            isNoramalDistribution(sepallen_list, GetAverage(sepallen_list), GetStdDev(petalwid_list));
            Console.WriteLine();

            Console.WriteLine("SEPAL WIDTH");
            Console.WriteLine("Average: " + GetAverage(sepalwid_list));
            Console.WriteLine("Variance: " + GetVariance(sepalwid_list));
            Console.WriteLine("Deviation: " + GetStdDev(sepalwid_list));
            Console.WriteLine("Median: " + GetMedian(sepalwid_list));
            isNoramalDistribution(sepalwid_list, GetAverage(sepalwid_list), GetStdDev(sepalwid_list));
            Console.WriteLine();

            Console.WriteLine("PETAL LENGTH");
            Console.WriteLine("Average: " + GetAverage(petallen_list));
            Console.WriteLine("Variance: " + GetVariance(petallen_list));
            Console.WriteLine("Deviation: " + GetStdDev(petallen_list));
            Console.WriteLine("Median: " + GetMedian(petallen_list));
            isNoramalDistribution(petallen_list, GetAverage(petallen_list), GetStdDev(petallen_list));
            Console.WriteLine();

            Console.WriteLine("PETAL WIDTH");
            Console.WriteLine("Average: " + GetAverage(petalwid_list));
            Console.WriteLine("Variance: " + GetVariance(petalwid_list));
            Console.WriteLine("Deviation: " + GetStdDev(petalwid_list));
            Console.WriteLine("Median: " + GetMedian(petalwid_list));
            isNoramalDistribution(petalwid_list, GetAverage(petalwid_list), GetStdDev(petalwid_list));


            Console.WriteLine();
            Console.WriteLine();
            Console.ReadKey();

            double[,] euclid = GenerateEuclid(list);
            CSVgenerator(euclid, "euclid_distance.csv");

            double[,] cosine = GenerateCosine(list);
            CSVgenerator(cosine, "cosine_similarity.csv");

            Occurence(sepallen_list, "sepal_lenght/occurences.csv");
            Occurence(sepalwid_list, "sepal_width/occurences.csv");
            Occurence(petallen_list, "petal_lenght/occurences.csv");
            Occurence(petalwid_list, "petal_width/occurences.csv");

            MakeGraphs(sepallen_list, "sepal_lenght/graph");
            MakeGraphs(sepalwid_list, "sepal_width/graph");
            MakeGraphs(petallen_list, "petal_lenght/graph");
            MakeGraphs(petalwid_list, "petal_width/graph");

            Console.WriteLine();
            Console.WriteLine();
            Console.ReadKey();

            dokmeansSSE(sepallen_list, sepalwid_list, petallen_list, petalwid_list);

            Console.WriteLine();
            Console.ReadKey();

            Kmeans k_means2 = new Kmeans();
            k_means2.InitData(petalwid_list, petallen_list);
            k_means2.InitData(sepalwid_list, sepallen_list);
            k_means2.Execute(2, true);

            Console.ReadKey();

        }

        //Median (na double poli)
        public static double GetMedian(List<double> source)
        {
            double[] sourceNumbers = source.ToArray();
            double[] sortedPNumbers = (double[])sourceNumbers.Clone();
            Array.Sort(sortedPNumbers);
            int size = sortedPNumbers.Length;
            int mid = size / 2;
            double median = (size % 2 != 0) ? (double)sortedPNumbers[mid] : ((double)sortedPNumbers[mid] + (double)sortedPNumbers[mid - 1]) / 2;
            return median;
        }

        //Pruměr (na double poli)
        public static double GetAverage(List<double> source)
        {
            double[] sourceNumbers = source.ToArray();
            return sourceNumbers.Take(sourceNumbers.Count()).Average();
        }

        //Standartní odchylka (na double poli)
        public static double GetStdDev(List<double> source)
        {
            double ret = 0;
            if (source.Count() > 0)
            {    
                double avg = GetAverage(source);     
                double sum = source.Sum(d => Math.Pow(d - avg, 2));    
                ret = Math.Sqrt((sum) / (source.Count() - 1));
            }
            return ret;
        }

        //Rozptyl (na double poli)
        public static double GetVariance(List<double> source)
        {
            double variance = 0;

            for (int i = 0; i < source.Count; i++)
            {
                variance += Math.Pow((source[i] - GetAverage(source)), 2);
            }

            return variance / (source.Count);
        }

        //Euklidovská vzdálenost
        public static double GetEuclidDist(Iris a, Iris b)
        {
            double dist = Math.Sqrt(((a.sepallen - b.sepallen) *  (a.sepallen - b.sepallen) + 
                (a.sepalwid - b.sepalwid) * (a.sepalwid - b.sepalwid) +
                (a.petallen - b.petallen) * (a.petallen - b.petallen) +
                (a.petalwid - b.petalwid) * (a.petalwid - b.petalwid)
                ));
            return dist;
        }

        //https://bioinformatics.oxfordjournals.org/content/suppl/2009/10/24/btp613.DC1/bioinf-2008-1835-File004.pdf
        //Cosinova podobnost
        public static double GetCosineDist(Iris a, Iris b)
        {
            double dist1 = (a.sepallen * b.sepallen) + (a.sepalwid * b.sepalwid) + (a.petallen * b.petallen) + (a.petalwid * b.petalwid);
            double dist2 = Math.Sqrt(
                (a.sepallen * a.sepallen) + 
                (a.sepalwid * a.sepalwid) + 
                (a.petallen * a.petallen) + 
                (a.petalwid * a.petalwid)
                ) * Math.Sqrt(
                (b.sepallen * b.sepallen) +
                (b.sepalwid * b.sepalwid) +
                (b.petallen * b.petallen) +
                (b.petalwid * b.petalwid)
                );

            return dist1 / dist2;
        }

        //Generovat euklidovské vzalenosti pro cely dataset
        public static double[,] GenerateEuclid(List<Iris> source)
        {

            double[,] x = new double[source.Count,source.Count];

            for (int i = 0; i < source.Count(); i++)
                {
                    for (int z = 0; z < source.Count(); z++)
                    {
                    x[i,z] = GetEuclidDist(source[i], source[z]);
                    }
                }
            return x;
        }

        //Generovat cos podobnost pro cely dataset
        public static double[,] GenerateCosine(List<Iris> source)
        {

            double[,] x = new double[source.Count, source.Count];

            for (int i = 0; i < source.Count(); i++)
            {
                for (int z = 0; z < source.Count(); z++)
                {
                    x[i, z] = GetCosineDist(source[i], source[z]);
                }
            }
            return x;
        }

        //CV4

        //četnost - relativni+kumulativni jednotlivých hodnot atributů
        public static void Occurence(List<double> source , string filename)
        {
            int count = 0;
            List<double> workData = new List<double>();

            foreach (var petal in source)
            {
                workData.Add(petal);
            }

         
            using (StreamWriter writer =
        new StreamWriter(filename))
            {
                //Rozdělení hodnot do grup podle hodnot
                workData.Sort();
                var groups = workData.GroupBy(i => i);

            writer.WriteLine("Hodnota,Pocet Vyskytu,Kumulativni cetnost,Relativni cetnost,CDF,PDF");
                //Sečist vyskyty v jednotlivých grupách

                foreach (var iris in groups)
            {
                writer.Write("{0},{1},", iris.Key, iris.Count());
                count = count + iris.Count();
                writer.Write(count+",");
                writer.Write(count / 1.5 + ",");
                writer.Write(iris.Count() / 1.5 + ",");
                writer.Write(PDFfce(StandartDistribution(iris.Key, GetAverage(workData), GetStdDev(workData))));
                writer.WriteLine();
            }
            }
            Console.WriteLine(filename + " generated!");
        }

        //Je normální distribucí ?
        public static void isNoramalDistribution(List<double> source, double mean, double deviation)
        {
            int cOfIsTrue = 0;
            double p1 = mean - deviation;
            double p2 = mean + deviation;
            double numberOfSatisElem = 0;
            double[] array = source.ToArray();

            // 1 * o 
            for (int i = 0; i < array.Length; i++)
            {
                if ((array[i] >= p1) && (array[i] <= p2))
                {
                    numberOfSatisElem++;
                }
            }

            if (numberOfSatisElem >= ((array.Length / 100.0) * 68.0))
            {
                cOfIsTrue++;
            }

            numberOfSatisElem = 0;
            p1 = mean - 2 * deviation;
            p2 = mean + 2 * deviation;

            // 2 * o
            for (int i = 0; i < array.Length; i++)
            {
                if ((array[i] >= p1) && (array[i] <= p2))
                {
                    numberOfSatisElem++;
                }
            }

            if (numberOfSatisElem >= ((array.Length / 100.0) * 95.0))
            {
                cOfIsTrue++;
            }

            numberOfSatisElem = 0;
            p1 = mean - 3 * deviation;
            p2 = mean + 3 * deviation;

            // 3 * o
            for (int i = 0; i < array.Length; i++)
            {
                if ((array[i] >= p1) && (array[i] <= p2))
                {
                    numberOfSatisElem++;
                }
            }

            if (numberOfSatisElem >= ((array.Length / 100.0) * 99.7))
            {
                cOfIsTrue++;
            }

            if (cOfIsTrue == 3)
            {
                Console.WriteLine("Je normalne rozlozeno");
            }
            else
            {
                Console.WriteLine("!! Neni normalne rozlozeno !!");
            }
        }

        //Výpočet CDF
        public static double StandartDistribution(double score, double average, double standardDeviation)
        {
            if (standardDeviation == 0) return 0;

            return (score - average) / standardDeviation;
        }

        //Výpočet PDF
        public static double PDFfce(double StandartDistribution)
        {
            var exponent = -1 * (0.5 * Math.Pow(StandartDistribution, 2));
            var numerator = Math.Pow(Math.E, exponent);
            var denominator = Math.Sqrt(2 * Math.PI);
            return numerator / denominator;
        }

        //Generovat grafy pro atribut
        public static void MakeGraphs(List<double> source, string filename)
        {
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
                PDF.Points.Add(new DataPoint(z, PDFfce(StandartDistribution(iris.Key, GetAverage(workData), GetStdDev(workData)))));
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

        //zapis do CSV
        public static void CSVgenerator(double[,] source, string filename)
        {
            using (StreamWriter writer =
            new StreamWriter(filename))
            {
                double[,] x = source;

                for (int i = 0; i < 150; i++)
                {
                    for (int z = 0; z < 150; z++)
                    {
                        writer.Write(x[i, z] + ",");
                    }
                    writer.WriteLine();
                }
            }
            Console.WriteLine(filename + " generated!");
        }

        //Spustit k-means pro 1-6 clusteru
        public static void dokmeansSSE(List<double> petalwid_list, List<double> petallen_list, List<double> sepalwid_list, List<double> sepallen_list)
        {
            Kmeans k_means = new Kmeans();
            k_means.InitData(petalwid_list, petallen_list);
            k_means.InitData(sepalwid_list, sepallen_list);
            k_means.Execute(1, false);

            Kmeans k_means2 = new Kmeans();
            k_means2.InitData(petalwid_list, petallen_list);
            k_means2.InitData(sepalwid_list, sepallen_list);
            k_means2.Execute(2, false);

            Kmeans k_means3 = new Kmeans();
            k_means3.InitData(petalwid_list, petallen_list);
            k_means3.InitData(sepalwid_list, sepallen_list);
            k_means3.Execute(3, false);

            Kmeans k_means4 = new Kmeans();
            k_means4.InitData(petalwid_list, petallen_list);
            k_means4.InitData(sepalwid_list, sepallen_list);
            k_means4.Execute(4, false);

            Kmeans k_means5 = new Kmeans();
            k_means5.InitData(petalwid_list, petallen_list);
            k_means5.InitData(sepalwid_list, sepallen_list);
            k_means5.Execute(5, false);

            Kmeans k_means6 = new Kmeans();
            k_means6.InitData(petalwid_list, petallen_list);
            k_means6.InitData(sepalwid_list, sepallen_list);
            k_means6.Execute(6, false);
        }
    }
}
