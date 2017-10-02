using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace REH0063_MAD1
{
    internal static class DBSCAN
    {
        
        //Separate lists (Not necessary, but just for convenience)
        private static List<double> _naSales_list = new List<double>();
        private static List<double> _euSales_list = new List<double>();
        private static List<double> _jpSales_list = new List<double>();
        private static List<double> _otherSales_list = new List<double>();
        private static List<double> _globalSales_list = new List<double>();

        private static void Main(string[] args)
        {
            var watch = Stopwatch.StartNew();
            Console.WriteLine("Working on it...");
            //Load CSV into data structure Videogames
            Loader load = new Loader();
            List<Videogame> data = load.csv(@"C:\vgsales.csv");

            //Separate sales into separate lists (For separate classification)
            foreach (Videogame game in data)
            {
                _naSales_list.Add(game._naSales);
                _euSales_list.Add(game._euSales);
                _jpSales_list.Add(game._jpSales);
                _otherSales_list.Add(game._otherSales);
                _globalSales_list.Add(game._globalSales);
            }

            //Print basic info for double based atributes...
            BasicInfo();

            //DBSCAN
            //Nejlepší nastaveni: Region = 1 milion, Neighbours= 2hry
            db dbscanClustering = new db(data, 1.0, 2);

            //QT TEST
            //Nejlepší nastavení Diameter = 3
            //qt qtClustering = new qt(data, 3);

            watch.Stop();
            Console.WriteLine("Completed in: " + watch.ElapsedMilliseconds + "ms");
            Console.WriteLine("Done, press enter to quit.");
            Console.ReadKey();
        }

        /// <summary>
        /// Print out basic information for double based attributes
        /// </summary>
        public static void BasicInfo()
        {
            System.IO.Directory.CreateDirectory("Output");

            using (StreamWriter writetext = new StreamWriter("output/output.txt"))
            {
                Operations functions = new Operations();

            writetext.WriteLine("+++NORTH AMERICA+++");
            writetext.WriteLine("Variance: " + functions.Variance(_naSales_list));
            writetext.WriteLine("Deviation: " + functions.StandartDeviation(_naSales_list));
            writetext.WriteLine("Median: " + functions.Median(_naSales_list));
            writetext.WriteLine("Average: " + functions.Average(_naSales_list));
            writetext.WriteLine();

            writetext.WriteLine("+++EUROPE+++");
            writetext.WriteLine("Variance: " + functions.Variance(_euSales_list));
            writetext.WriteLine("Deviation: " + functions.StandartDeviation(_euSales_list));
            writetext.WriteLine("Median: " + functions.Median(_euSales_list));
            writetext.WriteLine("Average: " + functions.Average(_euSales_list));
            writetext.WriteLine();

            writetext.WriteLine("+++JAPAN+++");
            writetext.WriteLine("Variance: " + functions.Variance(_jpSales_list));
            writetext.WriteLine("Deviation: " + functions.StandartDeviation(_jpSales_list));
            writetext.WriteLine("Median: " + functions.Median(_jpSales_list));
            writetext.WriteLine("Average: " + functions.Average(_jpSales_list));
            writetext.WriteLine();

            writetext.WriteLine("+++OTHER REGIONS+++");
            writetext.WriteLine("Variance: " + functions.Variance(_otherSales_list));
            writetext.WriteLine("Deviation: " + functions.StandartDeviation(_otherSales_list));
            writetext.WriteLine("Median: " + functions.Median(_otherSales_list));
            writetext.WriteLine("Average: " + functions.Average(_otherSales_list));
            writetext.WriteLine();

            writetext.WriteLine("+++GLOBAL+++");
            writetext.WriteLine("Variance: " + functions.Variance(_globalSales_list));
            writetext.WriteLine("Deviation: " + functions.StandartDeviation(_globalSales_list));
            writetext.WriteLine("Median: " + functions.Median(_globalSales_list));
            writetext.WriteLine("Average: " + functions.Average(_globalSales_list));
            writetext.WriteLine();

            functions.Occurence(_naSales_list, "Output/naOccurences.csv");
            functions.Occurence(_euSales_list, "Output/euOccurences.csv");
            functions.Occurence(_jpSales_list, "Output/jpOccurences.csv");
            functions.Occurence(_otherSales_list, "Output/otherOccurences.csv");
            functions.Occurence(_globalSales_list, "Output/globalOccurences.csv");

            }
        }
    }
}