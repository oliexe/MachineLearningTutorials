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

            System.IO.Directory.CreateDirectory("output");
            System.IO.Directory.CreateDirectory("output/sepal_lenght");
            System.IO.Directory.CreateDirectory("output/sepal_width");
            System.IO.Directory.CreateDirectory("output/petal_lenght");
            System.IO.Directory.CreateDirectory("output/petal_width");


            //CV3

            Console.WriteLine("SEPAL LENGTH");           
            Console.WriteLine("Average: " + Average(sepallen_list));
            Console.WriteLine("Variance: " + Variance(sepallen_list));
            Console.WriteLine("Deviation: " + StandartDeviation(sepallen_list));
            Console.WriteLine("Median: " + Median(sepallen_list));
            isNoramalDistribution(sepallen_list, Average(sepallen_list), StandartDeviation(petalwid_list));
            Console.WriteLine();

            Console.WriteLine("SEPAL WIDTH");
            Console.WriteLine("Average: " + Average(sepalwid_list));
            Console.WriteLine("Variance: " + Variance(sepalwid_list));
            Console.WriteLine("Deviation: " + StandartDeviation(sepalwid_list));
            Console.WriteLine("Median: " + Median(sepalwid_list));
            isNoramalDistribution(sepalwid_list, Average(sepalwid_list), StandartDeviation(sepalwid_list));
            Console.WriteLine();

            Console.WriteLine("PETAL LENGTH");
            Console.WriteLine("Average: " + Average(petallen_list));
            Console.WriteLine("Variance: " + Variance(petallen_list));
            Console.WriteLine("Deviation: " + StandartDeviation(petallen_list));
            Console.WriteLine("Median: " + Median(petallen_list));
            isNoramalDistribution(petallen_list, Average(petallen_list), StandartDeviation(petallen_list));
            Console.WriteLine();

            Console.WriteLine("PETAL WIDTH");
            Console.WriteLine("Average: " + Average(petalwid_list));
            Console.WriteLine("Variance: " + Variance(petalwid_list));
            Console.WriteLine("Deviation: " + StandartDeviation(petalwid_list));
            Console.WriteLine("Median: " + Median(petalwid_list));
            isNoramalDistribution(petalwid_list, Average(petalwid_list), StandartDeviation(petalwid_list));

            //CV4

            Console.WriteLine();
            Console.WriteLine();
            Console.ReadKey();

            double[,] euclid = GenerateElucid(list);
            CSVgenerator(euclid, "output/euclid_distance.csv");

            double[,] cosine = GenerateCosine(list);
            CSVgenerator(cosine, "output/cosine_similarity.csv");

            Occurence(sepallen_list, "output/sepal_lenght/occurences.csv");
            Occurence(sepalwid_list, "output/sepal_width/occurences.csv");
            Occurence(petallen_list, "output/petal_lenght/occurences.csv");
            Occurence(petalwid_list, "output/petal_width/occurences.csv");

            MakeGraphs(sepallen_list, "output/sepal_lenght/graph");
            MakeGraphs(sepalwid_list, "output/sepal_width/graph");
            MakeGraphs(petallen_list, "output/petal_lenght/graph");
            MakeGraphs(petalwid_list, "output/petal_width/graph");


            //CV5
            Console.WriteLine();
            Console.WriteLine();
            Console.ReadKey();

            GenerateSSE(sepallen_list, sepalwid_list, petallen_list, petalwid_list);

            Kmeans k_means2 = new Kmeans();
            //k_means2.InitData(sepallen_list, sepalwid_list);
            k_means2.InitData(petallen_list, petalwid_list);
            k_means2.Execute(5, true);

            Console.ReadKey();

        }

        //CV3

        ///<summary>
        ///Calculate median of values in list.
        ///</summary>
        public static double Median(List<double> source)
        {
            double[] sourceNumbers = source.ToArray();
            double[] sortedPNumbers = (double[])sourceNumbers.Clone();
            Array.Sort(sortedPNumbers);
            int size = sortedPNumbers.Length;
            int mid = size / 2;
            double median = (size % 2 != 0) ? (double)sortedPNumbers[mid] : ((double)sortedPNumbers[mid] + (double)sortedPNumbers[mid - 1]) / 2;
            return median;
        }

        ///<summary>
        ///Calculate average of values in a list.
        ///</summary>
        public static double Average(List<double> source)
        {
            double[] sourceNumbers = source.ToArray();
            return sourceNumbers.Take(sourceNumbers.Count()).Average();
        }

        ///<summary>
        ///Calculate standart deviation of list.
        ///</summary>
        public static double StandartDeviation(List<double> source)
        {
            double ret = 0;
            if (source.Count() > 0)
            {    
                double avg = Average(source);     
                double sum = source.Sum(d => Math.Pow(d - avg, 2));    
                ret = Math.Sqrt((sum) / (source.Count() - 1));
            }
            return ret;
        }

        ///<summary>
        ///Calculate variance of a list.
        ///</summary>
        public static double Variance(List<double> source)
        {
            double variance = 0;

            for (int i = 0; i < source.Count; i++)
            {
                variance += Math.Pow((source[i] - Average(source)), 2);
            }

            return variance / (source.Count);
        }

        ///<summary>
        ///Elucidan Distance between two specific points in the dataset.
        ///</summary>
        public static double ElucidanDistance(Iris a, Iris b)
        {
            double dist = Math.Sqrt(((a.sepallen - b.sepallen) *  (a.sepallen - b.sepallen) + 
                (a.sepalwid - b.sepalwid) * (a.sepalwid - b.sepalwid) +
                (a.petallen - b.petallen) * (a.petallen - b.petallen) +
                (a.petalwid - b.petalwid) * (a.petalwid - b.petalwid)
                ));
            return dist;
        }

        ///<summary>
        ///Cosine similarity between two specific points in the dataset.
        ///</summary>
        public static double CosineSimilarity(Iris a, Iris b)
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

        ///<summary>
        ///Generate elucidan distance for the whole list of points.
        ///</summary>
        public static double[,] GenerateElucid(List<Iris> source)
        {

            double[,] x = new double[source.Count,source.Count];

            for (int i = 0; i < source.Count(); i++)
                {
                    for (int z = 0; z < source.Count(); z++)
                    {
                    x[i,z] = ElucidanDistance(source[i], source[z]);
                    }
                }
            return x;
        }

        ///<summary>
        ///Generate cosine similarity for the whole list of points.
        ///</summary>
        public static double[,] GenerateCosine(List<Iris> source)
        {

            double[,] x = new double[source.Count, source.Count];

            for (int i = 0; i < source.Count(); i++)
            {
                for (int z = 0; z < source.Count(); z++)
                {
                    x[i, z] = CosineSimilarity(source[i], source[z]);
                }
            }
            return x;
        }

        //CV4

        ///<summary>
        ///Generate CSV of occurences of different values inside a list.
        ///</summary>
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
                writer.Write(PDF(CDF(iris.Key, Average(workData), StandartDeviation(workData))));
                writer.WriteLine();
            }
            }
            Console.WriteLine(filename + " generated!");
        }

        /// <summary>
        /// Check if the source list is distributed normally.
        /// </summary>
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

        /// <summary>
        /// Calculate cumulative distribution function (CDF)
        /// </summary>
        public static double CDF(double score, double average, double standardDeviation)
        {
            if (standardDeviation == 0) return 0;

            return (score - average) / standardDeviation;
        }

        /// <summary>
        /// Calculate probability density function (PDF)
        /// </summary>
        public static double PDF(double StandartDistribution)
        {
            var exponent = -1 * (0.5 * Math.Pow(StandartDistribution, 2));
            var numerator = Math.Pow(Math.E, exponent);
            var denominator = Math.Sqrt(2 * Math.PI);
            return numerator / denominator;
        }

        /// <summary>
        /// "helper" funtion to generate CDF,PDF and histogram for a list.
        /// </summary>
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
                PDF.Points.Add(new DataPoint(z, Program.PDF(CDF(iris.Key, Average(workData), StandartDeviation(workData)))));
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

        /// <summary>
        /// CSV generator "helper" funtion.
        /// </summary>
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

        //CV5
        //Kmeans.cs

        /// <summary>
        /// Run k-means algorithm for multiple cluster values and generate SSE value.
        /// </summary>
        public static void GenerateSSE(List<double> petalwid_list, List<double> petallen_list, List<double> sepalwid_list, List<double> sepallen_list)
        {
            Kmeans k_means = new Kmeans();
            //k_means.InitData(petalwid_list, petallen_list);
            k_means.InitData(sepalwid_list, sepallen_list);
            k_means.Execute(1, false);

            Kmeans k_means2 = new Kmeans();
            //k_means2.InitData(petalwid_list, petallen_list);
            k_means2.InitData(sepalwid_list, sepallen_list);
            k_means2.Execute(2, false);

            Kmeans k_means3 = new Kmeans();
            //k_means3.InitData(petalwid_list, petallen_list);
            k_means3.InitData(sepalwid_list, sepallen_list);
            k_means3.Execute(3, false);

            Kmeans k_means4 = new Kmeans();
            //k_means4.InitData(petalwid_list, petallen_list);
            k_means4.InitData(sepalwid_list, sepallen_list);
            k_means4.Execute(4, false);

            Kmeans k_means5 = new Kmeans();
            //k_means5.InitData(petalwid_list, petallen_list);
            k_means5.InitData(sepalwid_list, sepallen_list);
            k_means5.Execute(5, false);

            Kmeans k_means6 = new Kmeans();
            //k_means6.InitData(petalwid_list, petallen_list);
            k_means6.InitData(sepalwid_list, sepallen_list);
            k_means6.Execute(6, false);
        }
    }
}
