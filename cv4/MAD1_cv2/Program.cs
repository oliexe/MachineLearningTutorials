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
            List<iris> list = new List<iris>();
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                line = line.Replace(",", ".");
                var values = line.Split(';');
                iris result_line = new iris(Convert.ToDouble(values[0]), Convert.ToDouble(values[1]), Convert.ToDouble(values[2]), Convert.ToDouble(values[3]), values[4]);
                list.Add(result_line);
            }

            List<double> sepallen_list = new List<double>();
            List<double> sepalwid_list = new List<double>();
            List<double> petallen_list = new List<double>();
            List<double> petalwid_list = new List<double>();

            foreach (iris line in list)
            {
                sepallen_list.Add(line.sepallen);
                sepalwid_list.Add(line.sepalwid);
                petallen_list.Add(line.petallen);
                petalwid_list.Add(line.petalwid);
            }

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

            double[,] euclid = GenerateEuclid(list);
            CSVgenerator(euclid, "euclid_distance.csv");

            double[,] cosine = GenerateCosine(list);
            CSVgenerator(cosine, "cosine_similarity.csv");


            Occurence(sepallen_list, "sepal_lenght_occurences.csv");
            Occurence(sepalwid_list, "sepal_width_occurences.csv");
            Occurence(petallen_list, "petal_lenght_occurences.csv");
            Occurence(petalwid_list, "petal_width_occurences.csv");

            MakeHistogram(petallen_list, "histogram.pdf");


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
        public static double GetEuclidDist(iris a, iris b)
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
        public static double GetCosineDist(iris a, iris b)
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
        public static double[,] GenerateEuclid(List<iris> source)
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
        public static double[,] GenerateCosine(List<iris> source)
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

        //četnost - relativni+kumulativni jednotlivých hodnot atributů
        public static void Occurence(List<double> source , string filename)
        {
            int count = 0;
            using (StreamWriter writer =
        new StreamWriter(filename))
            {
                //Rozdělení hodnot do grup podle hodnot
                source.Sort();
                var groups = source.GroupBy(i => i);

            writer.WriteLine("Hodnota,Pocet Vyskytu,Kumulativni cetnost,Kumulativni relativni cetnost,Relativni cetnost");
                //Sečist vyskyty v jednotlivých grupách

                foreach (var iris in groups)
            {
                writer.Write("{0},{1},", iris.Key, iris.Count());
                count = count + iris.Count();
                writer.Write(count+",");
                writer.Write(count / 1.5 + ",");
                writer.Write(iris.Count() / 1.5);
                writer.WriteLine();
            }
            }
            Console.WriteLine(filename + " generated!");
        }

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
                Console.WriteLine("Neni normalne rozlozeno");
            }
        }

        public static void MakeHistogram(List<double> source, string filename)
        {
            PlotModel graf = new PlotModel();
            source.Sort();
            double[] values = source.ToArray();
            var bucketeer = new Dictionary<double, double>();
            var groups = source.GroupBy(i => i);
            List<string> kek = new List<string>();

            foreach (var iris in groups)
            {
                bucketeer.Add(iris.Key, iris.Count());
                kek.Add(iris.Key.ToString());
            }

            ColumnSeries lol = new ColumnSeries();
            CategoryAxis lol2 = new CategoryAxis();
            
            foreach (var pair in bucketeer.OrderBy(x => x.Key))
            {
                lol.Items.Add(new ColumnItem(pair.Value));
                lol.LabelFormatString = "{0:.00}%";
            }
            lol2.ItemsSource = kek;
            graf.Series.Add(lol);
            graf.Axes.Add(lol2);
            using (var stream = File.Create(filename))
            {
                var pdfExporter = new PdfExporter { Width = 1000, Height = 400 };
                pdfExporter.Export(graf, stream);
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

    }
}
