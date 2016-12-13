using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REH0063_MAD1
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
        ///Generate CSV of occurences of different values inside a list.
        ///</summary>
        public void Occurence(List<double> source, string filename)
        {
            int count = 0;
            List<double> workData = new List<double>();

            foreach (var game in source)
            {
                workData.Add(game);
            }

            using (StreamWriter writer =
        new StreamWriter(filename))
            {
                //Rozdělení hodnot do grup podle hodnot
                workData.Sort();
                var groups = workData.GroupBy(i => i);

                writer.WriteLine("Hodnota,Pocet Vyskytu,Kumulativni cetnost,Relativni cetnost,CDF,PDF");
                //Sečist vyskyty v jednotlivých grupách

                foreach (var games in groups)
                {
                    writer.Write("{0},{1},", games.Key, games.Count());
                    count = count + games.Count();
                    writer.Write(count + ",");
                    writer.Write(count / 1.5 + ",");
                    writer.Write(games.Count() / 1.5 + ",");
                    writer.Write(PDF(CDF(games.Key, Average(workData), StandartDeviation(workData))));
                    writer.WriteLine();
                }
            }
            Console.WriteLine(filename + " generated!");
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
