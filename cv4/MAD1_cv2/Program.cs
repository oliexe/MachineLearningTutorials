using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Console.WriteLine();

            Console.WriteLine("SEPAL WIDTH");
            Console.WriteLine("Average: " + GetAverage(sepalwid_list));
            Console.WriteLine("Variance: " + GetVariance(sepalwid_list));
            Console.WriteLine("Deviation: " + GetStdDev(sepalwid_list));
            Console.WriteLine("Median: " + GetMedian(sepalwid_list));
            Console.WriteLine();

            Console.WriteLine("PETAL LENGTH");
            Console.WriteLine("Average: " + GetAverage(petallen_list));
            Console.WriteLine("Variance: " + GetVariance(petallen_list));
            Console.WriteLine("Deviation: " + GetStdDev(petallen_list));
            Console.WriteLine("Median: " + GetMedian(petallen_list));
            Console.WriteLine();

            Console.WriteLine("PETAL WIDTH");
            Console.WriteLine("Average: " + GetAverage(petalwid_list));
            Console.WriteLine("Variance: " + GetVariance(petalwid_list));
            Console.WriteLine("Deviation: " + GetStdDev(petalwid_list));
            Console.WriteLine("Median: " + GetMedian(petalwid_list));


            double[,] lol = GenerateEuclid(list);
            CSVgenerator(lol, "euklid.csv");

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

        //Generovat euklidovské vzalenosti
        public static double[,] GenerateEuclid(List<iris> source)
        {

            Console.WriteLine("Vypocet euklidovskych vzdalenosti....");

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
                        writer.Write(x[i,z] + ",");
                    }
                    writer.WriteLine();
                }
            }
            Console.WriteLine(filename + " generated!");

        
        }

    }
}
