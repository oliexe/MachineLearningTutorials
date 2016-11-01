using MAD.Data;
using MAD.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MAD
{
    public class Operations
    {
        ///<summary>
        ///Calculate median of values in a list.
        ///</summary>
        public double Median(List<double> source)
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
        public double Average(List<double> source)
        {
            double[] sourceNumbers = source.ToArray();
            return sourceNumbers.Take(sourceNumbers.Count()).Average();
        }

        ///<summary>
        ///Calculate standart deviation of list.
        ///</summary>
        public double StandartDeviation(List<double> source)
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
        public double Variance(List<double> source)
        {
            double variance = 0;

            for (int i = 0; i < source.Count; i++)
            {
                variance += Math.Pow((source[i] - Average(source)), 2);
            }

            return variance / (source.Count);
        }

        ///<summary>
        ///Cosine similarity between two specific points in the dataset.
        ///</summary>
        public double CosineSimilarity(Iris a, Iris b)
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
        public double[,] GenerateElucid(List<Iris> source)
        {
            ElucidanDistance elucid = new ElucidanDistance();
            double[,] x = new double[source.Count, source.Count];

            for (int i = 0; i < source.Count(); i++)
            {
                for (int z = 0; z < source.Count(); z++)
                {
                    x[i, z] = elucid.Get(source[i], source[z]);
                }
            }
            return x;
        }

        ///<summary>
        ///Generate cosine similarity for the whole list of points.
        ///</summary>
        public double[,] GenerateCosine(List<Iris> source)
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

        ///<summary>
        ///Generate CSV of occurences of different values inside a list.
        ///</summary>
        public void Occurence(List<double> source, string filename)
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
                    writer.Write(count + ",");
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
        public void isNormalDistribution(List<double> source, double mean, double deviation)
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
                Console.WriteLine("IS a normal distrubution");
            }
            else
            {
                Console.WriteLine("NOT a normal distribution");
            }
        }

        /// <summary>
        /// Calculate cumulative distribution function (CDF)
        /// </summary>
        public double CDF(double score, double average, double standardDeviation)
        {
            if (standardDeviation == 0) return 0;

            return (score - average) / standardDeviation;
        }

        /// <summary>
        /// Calculate probability density function (PDF)
        /// </summary>
        public double PDF(double StandartDistribution)
        {
            var exponent = -1 * (0.5 * Math.Pow(StandartDistribution, 2));
            var numerator = Math.Pow(Math.E, exponent);
            var denominator = Math.Sqrt(2 * Math.PI);
            return numerator / denominator;
        }
    }
}