using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace karate.dataset
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Node> data = InitData(@"C:\KarateClub.csv");

            foreach (Node item in data)
            {
                item.Print();
            }

            GetDegrees(data);
            Console.ReadKey();
        }

        public static void GetDegrees(List<Node> input)
        {
            int[] degrees = new int[input.Count()];
            int i = 0;
            int count = 0;

            foreach (Node item in input)
            {
                degrees[i] = item.GetDegree();
                i++;
            }

            Array.Sort(degrees);

            var groups = degrees.GroupBy(item => item);

            foreach (var group in groups)
            {
                count = count + group.Count();
            }

            Console.WriteLine();
           Console.WriteLine("--------------------------------------------------------------");
            Console.WriteLine();
            foreach (var group in groups)
            {
                Console.Write(string.Format("STUPEN: {1}  VYSKYT:{0}", group.Count(), group.Key));
                double vysledek = (double)group.Count() / (double)count;
                Console.WriteLine("  RELATIVNI CETNOST:" + Math.Round(vysledek, 4));
            }
            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------------------");
            Console.WriteLine();

            Console.WriteLine("Prumer : " + Average(degrees));
            Console.WriteLine("Min : " + degrees.Min());
            Console.WriteLine("Max : " + degrees.Max());
        }

        public static double Average(int[] source)
        {
            int[] sourceNumbers = source.ToArray();
            return sourceNumbers.Take(sourceNumbers.Count()).Average();
        }

        public static void ConstructMatrix()
        {


        }

        public static List<Node> InitData(string filename)
        {
            var reader = new StreamReader(File.OpenRead(filename));
            reader.ReadLine();
            List<Node> Nodes = new List<Node>();
            List<Node> Output = new List<Node>();

            //Blank nodes
            for (int i = 0; i < 100; i++)
            {
                Nodes.Add(new Node(i));
            }
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');
                Nodes[Convert.ToInt32(values[0])].AddNeighbour(Convert.ToInt32(values[1]));
                Nodes[Convert.ToInt32(values[1])].AddNeighbour(Convert.ToInt32(values[0]));
            }
            for (int i = 0; i < 100; i++)
            {
                if (Nodes[i].sousedi.Count != 0)
                {
                    Output.Add(Nodes[i]);
                }
            }
            return Output;
        }
    }
}
