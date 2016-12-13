using System;
using System.Collections.Generic;

namespace REH0063_MAD1
{
    static class DBSCAN
    {
        //Separate lists
        private static List<double> _naSales_list = new List<double>();
        private static List<double> _euSales_list = new List<double>();
        private static List<double> _jpSales_list = new List<double>();
        private static List<double> _otherSales_list = new List<double>();
        private static List<double> _globalSales_list = new List<double>();

        private static void Main(string[] args)
        {
            //Load CSV into data structure Videogames
            Loader load = new Loader();
            List<Videogame> data = load.csv(@"C:\vgsales.csv");

            //Separate sales into separate lists
            foreach (Videogame game in data)
            {
                _naSales_list.Add(game.naSales);
                _euSales_list.Add(game.euSales);
                _jpSales_list.Add(game.jpSales);
                _otherSales_list.Add(game.otherSales);
                _globalSales_list.Add(game.globalSales);
            }

            //Print basic info for number based atributes
            BasicInfo();

            //dbscan clustering
            dbscan sbscanClustering = new dbscan(data);

            //Quality treshold clustering
            qt qtClustering = new qt(data);

            //wait
            Console.WriteLine("Done");
            Console.ReadKey();
        }

        public static void BasicInfo()
        {
            System.IO.Directory.CreateDirectory("Output");
            Operations functions = new Operations();

            Console.WriteLine("+++NORTH AMERICA+++");
            Console.WriteLine("Variance: " + functions.Variance(_naSales_list));
            Console.WriteLine("Deviation: " + functions.StandartDeviation(_naSales_list));
            Console.WriteLine("Median: " + functions.Median(_naSales_list));
            Console.WriteLine("Average: " + functions.Average(_naSales_list));
            Console.WriteLine();

            Console.WriteLine("+++EUROPE+++");
            Console.WriteLine("Variance: " + functions.Variance(_euSales_list));
            Console.WriteLine("Deviation: " + functions.StandartDeviation(_euSales_list));
            Console.WriteLine("Median: " + functions.Median(_euSales_list));
            Console.WriteLine("Average: " + functions.Average(_euSales_list));
            Console.WriteLine();

            Console.WriteLine("+++JAPAN+++");
            Console.WriteLine("Variance: " + functions.Variance(_jpSales_list));
            Console.WriteLine("Deviation: " + functions.StandartDeviation(_jpSales_list));
            Console.WriteLine("Median: " + functions.Median(_jpSales_list));
            Console.WriteLine("Average: " + functions.Average(_jpSales_list));
            Console.WriteLine();

            Console.WriteLine("+++GLOBAL+++");
            Console.WriteLine("Variance: " + functions.Variance(_otherSales_list));
            Console.WriteLine("Deviation: " + functions.StandartDeviation(_otherSales_list));
            Console.WriteLine("Median: " + functions.Median(_otherSales_list));
            Console.WriteLine("Average: " + functions.Average(_otherSales_list));
            Console.WriteLine();

            Console.WriteLine("+++GLOBAL+++");
            Console.WriteLine("Variance: " + functions.Variance(_globalSales_list));
            Console.WriteLine("Deviation: " + functions.StandartDeviation(_globalSales_list));
            Console.WriteLine("Median: " + functions.Median(_globalSales_list));
            Console.WriteLine("Average: " + functions.Average(_globalSales_list));
            Console.WriteLine();

            functions.Occurence(_naSales_list, "Output/naOccurences.csv");
            functions.Occurence(_euSales_list, "Output/euOccurences.csv");
            functions.Occurence(_jpSales_list, "Output/jpOccurences.csv");
            functions.Occurence(_otherSales_list, "Output/otherOccurences.csv");
            functions.Occurence(_globalSales_list, "Output/globalOccurences.csv");

            Console.ReadKey();
            Console.Clear();
        }
    }
}
