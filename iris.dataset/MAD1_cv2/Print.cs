using MAD.Data;
using MAD.Enum;
using MAD.Graph;
using MAD.Helpers;
using System;
using System.Collections.Generic;
using System.IO;

namespace MAD
{
    internal class ConsolePrint
    {
        private static CSV csv = new CSV();
        private static MakeGraph graph = new MakeGraph();
        private static Operations functions = new Operations();

        private static List<Iris> list = new List<Iris>();

        private static List<double> sepallen_list = new List<double>();
        private static List<double> sepalwid_list = new List<double>();

        private static List<double> petallen_list = new List<double>();
        private static List<double> petalwid_list = new List<double>();

        private static List<string> petal_species = new List<string>();

        public List<Iris> InitData(string filename)
        {
            var reader = new StreamReader(File.OpenRead(filename));
            List<Iris> output = new List<Iris>();
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                line = line.Replace(",", ".");
                var values = line.Split(';');
                Iris result_line = new Iris(Convert.ToDouble(values[0]), Convert.ToDouble(values[1]), Convert.ToDouble(values[2]), Convert.ToDouble(values[3]), values[4]);
                output.Add(result_line);
            }

            return output;
        }

        public void InitStruct(List<Iris> input)
        {
            list = input;

            foreach (Iris line in list)
            {
                sepallen_list.Add(line.sepallen);
                sepalwid_list.Add(line.sepalwid);
                petallen_list.Add(line.petallen);
                petalwid_list.Add(line.petalwid);
                petal_species.Add(line.species);
            }

            System.IO.Directory.CreateDirectory("output");
            System.IO.Directory.CreateDirectory("output/sepal_lenght");
            System.IO.Directory.CreateDirectory("output/sepal_width");
            System.IO.Directory.CreateDirectory("output/petal_lenght");
            System.IO.Directory.CreateDirectory("output/petal_width");
        }

        //Implemetujte aplikaci pro data z tabulky na 9. snímku (iris.data) ze 3. přednášky, která určí Eeukleidovskou vzdálenost, průměr (mean), median, rozptyl (total variance), standardni odchylku a kosinovou podobnost.
        public void CV4()
        {
            Console.WriteLine("+++SEPAL LENGTH+++");
            Console.WriteLine("Average: " + functions.Average(sepallen_list));
            Console.WriteLine("Variance: " + functions.Variance(sepallen_list));
            Console.WriteLine("Deviation: " + functions.StandartDeviation(sepallen_list));
            Console.WriteLine("Median: " + functions.Median(sepallen_list));
            functions.isNormalDistribution(sepallen_list, functions.Average(sepallen_list), functions.StandartDeviation(petalwid_list));
            Console.WriteLine();

            Console.WriteLine("+++SEPAL WIDTH+++");
            Console.WriteLine("Average: " + functions.Average(sepalwid_list));
            Console.WriteLine("Variance: " + functions.Variance(sepalwid_list));
            Console.WriteLine("Deviation: " + functions.StandartDeviation(sepalwid_list));
            Console.WriteLine("Median: " + functions.Median(sepalwid_list));
            functions.isNormalDistribution(sepalwid_list, functions.Average(sepalwid_list), functions.StandartDeviation(sepalwid_list));
            Console.WriteLine();

            Console.WriteLine("+++PETAL LENGTH+++");
            Console.WriteLine("Average: " + functions.Average(petallen_list));
            Console.WriteLine("Variance: " + functions.Variance(petallen_list));
            Console.WriteLine("Deviation: " + functions.StandartDeviation(petallen_list));
            Console.WriteLine("Median: " + functions.Median(petallen_list));
            functions.isNormalDistribution(petallen_list, functions.Average(petallen_list), functions.StandartDeviation(petallen_list));
            Console.WriteLine();

            Console.WriteLine("+++PETAL WIDTH+++");
            Console.WriteLine("Average: " + functions.Average(petalwid_list));
            Console.WriteLine("Variance: " + functions.Variance(petalwid_list));
            Console.WriteLine("Deviation: " + functions.StandartDeviation(petalwid_list));
            Console.WriteLine("Median: " + functions.Median(petalwid_list));
            functions.isNormalDistribution(petalwid_list, functions.Average(petalwid_list), functions.StandartDeviation(petalwid_list));

            Console.ReadKey();
        }

        //Pro datovou kolekci Iris dataset (iris.csv) určete pro každý numerický atribut:
        //pokud jste ještě minule neurčili: průměr, rozptyl, medián, modus a směrodatnou odchylku,
        //četnost (absolutní)jednotlivých hodnot atributů, jejich relativní a kumulativní četnost.
        //Ověřte, zda rozdělení pravděpodobnosti všech atributů odpovídá normálnímu rozdělení(za pomoci průměru a směrodatné odchylky).
        //Dále graficky znázorněte histogram, hustotu(PDF) a distribuční funkci(CDF)(nikoliv pomocí R, ale např.pomocí Excelu - plugin Analýza dat, nebo Weky nebo např.pomocí jiného nástroje(např.gnuplot)).
        public void CV5()
        {
            Console.Clear();

            Console.Write("Elucidian distance + Cosine similarity? (y/n)");
            string response = Console.ReadLine();
            if (response.Equals("y"))
            {
                double[,] euclid = functions.GenerateElucid(list);
                csv.generate(euclid, "output/euclid_distance.csv");

                double[,] cosine = functions.GenerateCosine(list);
                csv.generate(cosine, "output/cosine_similarity.csv");
            }

            Console.Write("Save occurences, CDF, PDF to CSV? (y/n)");
            response = Console.ReadLine();
            if (response.Equals("y"))
            {
                functions.Occurence(sepallen_list, "output/sepal_lenght/occurences.csv");
                functions.Occurence(sepalwid_list, "output/sepal_width/occurences.csv");
                functions.Occurence(petallen_list, "output/petal_lenght/occurences.csv");
                functions.Occurence(petalwid_list, "output/petal_width/occurences.csv");
            }

            Console.Write("Graph CDF, PDF, Occurences ? (y/n)");
            response = Console.ReadLine();
            if (response.Equals("y"))
            {
                graph.CDFPDF(sepallen_list, "output/sepal_lenght/graph");
                graph.CDFPDF(sepalwid_list, "output/sepal_width/graph");
                graph.CDFPDF(petallen_list, "output/petal_lenght/graph");
                graph.CDFPDF(petalwid_list, "output/petal_width/graph");
            }
        }

        //Pro dataset iris.data naimplementujte shlukovací metodu K-means:
        //Zvolte počáteční k=3 náhodné objekty, počáteční centroidy.Proveďte první shlukování datasetu do k shluků(na základě Eukl.vzdálenosti).
        //Pro nalezené shluky najděte nové centoridy.Proveďte shlukování podle nových centroidů.
        //Opakujte dokud "nastávají změny".
        //Vyhodnoťte účelovou funkci (SSE) pro různá k; nalzeněte hodnotu k, které minimalizuje její hodnotu.
        //Proveďte experimenty s volbou počátečních centroidů a porovnání dosažených výsledků.
        public void CV6()
        {
            Console.Clear();
            Console.Write("SSE for K-means? (y/n)");
            string response = Console.ReadLine();
            if (response.Equals("y"))
            {
                KMeans k_means_sse = new KMeans();
                k_means_sse.GenerateSSE(sepallen_list, sepalwid_list, petallen_list, petalwid_list);
                Console.WriteLine();
            }

            Console.Write("K-means? (y/n)");
            response = Console.ReadLine();
            if (response.Equals("y"))
            {
                Console.Write("How many clusters ?");
                int res_clusters = Int32.Parse(Console.ReadLine());
                KMeans k_means2 = new KMeans();
                k_means2.InitData(petallen_list, petalwid_list);
                k_means2.Execute(res_clusters, true);
                Console.ReadKey();
            }
        }

        //Pro dataset iris.data naimplementujte klasifikační metodu kNN.
        //Vytvořte testovací sadu např. 120 instancí, a tréningovou sadu zbývajících 30 instancí a ty klasifikujte. Porovnejte s jejich "originální" klasifikací.
        //Vytvořte nové "náhodné instance" bez určení třídy (Species) a zkuste je klasifikovat.Velikost tréningové sady vhodně zvolte.
        public void CV7()
        {
            Console.Clear();
            Console.Write("k-nearest neighbours? (y/n)");
            string response = Console.ReadLine();
            if (response.Equals("y"))
            {
                Console.Write("How many neighbours ?");
                int res_neigh = Int32.Parse(Console.ReadLine());

                List<Iris> training_dataset = InitData(@"C:\iris.csv");
                List<Iris> test_dataset = InitData(@"C:\iris_test.csv");
                KNearestNeighbors knem = new KNearestNeighbors();
                knem.InitData(training_dataset, DataType.TRAINING);
                knem.InitData(test_dataset, DataType.TEST);
                knem.Classify(res_neigh);
            }

            Console.ReadKey();
        }
    }
}