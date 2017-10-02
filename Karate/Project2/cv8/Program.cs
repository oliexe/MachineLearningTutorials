using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cv8.Graph;
using System.IO;

namespace cv8
{
    class Program
    {
        static void Main(string[] args)
        {
            Graph<int> undirectedGraph = GenerateUndirectedGraph( "KarateClub.csv" );
            Graph<int> directedGraph = GenerateDirectedGraph( "KarateClub.csv" );

            Console.WriteLine(undirectedGraph.VertexDegree(34));
            Console.WriteLine(undirectedGraph.MinimalVertexDegree());
            Console.WriteLine(undirectedGraph.MaximalVertexDegree());
            Console.WriteLine(undirectedGraph.AverageVertexDegree());
            var absoluteFreq = undirectedGraph.AbsoluteFrequencyOfDegrees();
            var relativeFreq = undirectedGraph.RelativeFrequencyOfDegrees();
            WriteToCSV(absoluteFreq, "absoluteFreq.csv");
            WriteToCSV(relativeFreq, "relativeFreq.csv");

            int[,] undirectedMatrix = undirectedGraph.CreateUndirectedAdjacencyMatrix();
            int[,] directedMatrix = undirectedGraph.CreateDirectedAdjacencyMatrix();

            //PrintMatrix( undirectedMatrix );
            //Console.WriteLine();
            //PrintMatrix( directedMatrix );


            //najdlhsia najkratsia cesta je 5
            //int [,] distanceMatrix = undirectedGraph.FloydWarshallAlgorithm(undirectedMatrix);
            //PrintMatrix(distanceMatrix);

            //Console.WriteLine(undirectedGraph.MeanDistance(distanceMatrix, 0));
            //for (int i = 0; i < distanceMatrix.GetLength(0); i++)
            //{
            //    Console.WriteLine(undirectedGraph.MeanDistance(distanceMatrix, i));
            //}
            //Console.WriteLine(undirectedGraph.GlobalMeanDistance(distanceMatrix));

            //for (int i = 0; i < distanceMatrix.GetLength(0); i++)
            //{
            //    Console.WriteLine(undirectedGraph.ClossnessCentrality(distanceMatrix, i));
            //}
            //var absFreqOfDst = undirectedGraph.AbsoluteFrequencyOfDistances(distanceMatrix);
            //var relFreqOfDst = undirectedGraph.RelativeFrequencyOfDistances(distanceMatrix);


            //foreach (var item in undirectedGraph.Values)
            //{
            //    double c = undirectedGraph.ClusterCoeficient(item.Key);
            //    Console.WriteLine(c);
            //}


            //WriteToCSV(absFreqOfDst, "absoluteFreqOfDst.csv");
            //WriteToCSV(relFreqOfDst, "relativeFreqOfDst.csv");


            double[] probabilityArray = { 0.3, 0.5, 0.7};
            /*
            foreach (var p in probabilityArray)
            {
                Console.WriteLine("probability: {0}", p.ToString());

                var randGraph = GenerateRandomGraph(34, p);

                var randMatrix = randGraph.CreateUndirectedAdjacencyMatrix();

                Console.WriteLine("Adjacency Matrix: ");
                PrintMatrix(randMatrix);
                Console.WriteLine();

                int vertex = 34;
                Console.WriteLine("Degree of vertex {0}: {1}", vertex, randGraph.VertexDegree(34).ToString());
                Console.WriteLine();

                Console.WriteLine("Minimal vertex degree: {0}", randGraph.MinimalVertexDegree().ToString());
                Console.WriteLine();

                Console.WriteLine("Maximal vertex degre: {0}", randGraph.MaximalVertexDegree().ToString());
                Console.WriteLine();

                Console.WriteLine("Average vertex degree: {0}", randGraph.AverageVertexDegree().ToString());
                Console.WriteLine();

                Console.WriteLine("Cluster coeficient of vertex {0} : {1}", vertex, randGraph.ClusterCoeficient(34).ToString());
                Console.WriteLine();

                var absoluteFreq = randGraph.AbsoluteFrequencyOfDegrees();

                var relativeFreq = randGraph.RelativeFrequencyOfDegrees();

                //WriteToCSV(absoluteFreq, "randGraphAbsoluteFreq.csv");
                //WriteToCSV(relativeFreq, "randGraphRelativeFreq.csv");

                var globalMean = undirectedGraph.GlobalMeanDistance(randMatrix);
                Console.WriteLine("Global Mean distance: {0}", globalMean.ToString());
                Console.WriteLine();

                var absFreqOfDst = randGraph.AbsoluteFrequencyOfDistances(randMatrix);
                var relFreqOfDst = randGraph.RelativeFrequencyOfDistances(randMatrix);
                
                int[,] distanceRandMatrix = randGraph.FloydWarshallAlgorithm(randMatrix);
                Console.WriteLine("Distance Matrix: ");
                PrintMatrix(distanceRandMatrix);
                Console.WriteLine();

                
                string path = String.Format(@"C:\Users\maros\Desktop\graph_{0}.csv", p.ToString());
                WriteGraphToCSV(randGraph, path);

                Console.WriteLine("--------------------------------------");
            }*/

            Graph<int> input = new Graph<int>();
            input.AddVertex();
            input.AddVertex();

            Vertex<int> A = new Vertex<int>(1);
            Vertex<int> B = new Vertex<int>(2);
            Edge<int> edge = new Edge<int>(A, B);
            input.AddEdge(A.ID, edge);
            input.AddEdge(B.ID, edge);

            var freeScale = GenerateFreeScaleGraph(input, 34);
            Console.ReadLine();
        }

        private static void WriteGraphToCSV(Graph<int> graph, string path)
        {
            Dictionary<int, List<double>> tuples = new Dictionary<int, List<double>>();
            foreach (var tuple in graph.Values)
            {
                tuples[tuple.Key] = new List<double>();
                foreach (var edge in tuple.Value)
                {
                    var id = -1;

                    if (edge.VertexA.ID == tuple.Key)
                        id = edge.VertexB.ID;
                    else
                        id = edge.VertexA.ID;

                    tuples[tuple.Key].Add(id);
                }
            }

            StringBuilder csv = new StringBuilder();
            foreach (var tuple in tuples)
            {
                foreach (var item in tuple.Value)
                {
                    //Console.WriteLine(tuple.Key + ";" + item);
                    csv.AppendLine(string.Format("{0};{1}", tuple.Key.ToString(), item.ToString()));
                }
            }

            System.IO.File.WriteAllText(path, csv.ToString());
        }

        private static Graph<int> GenerateUndirectedGraph(string fileName)
        {
            Graph<int> graph = new Graph<int>();
            var lines = File.ReadLines( fileName );
            foreach ( var line in lines )
            {
                var vertexes = line.Split( ';' );
                Vertex<int> v1 = new Vertex<int>( int.Parse(vertexes[0]) );
                Vertex<int> v2 = new Vertex<int>( int.Parse( vertexes[1] ) );
                Edge<int> edge = new Edge<int>( v1, v2 );
                graph.AddEdge( v1.ID, edge );
                graph.AddEdge( v2.ID, edge );
            }
            return graph;
        }

        private static Graph<int> GenerateDirectedGraph( string fileName )
        {
            Graph<int> graph = new Graph<int>();
            var lines = File.ReadLines( fileName );
            foreach ( var line in lines )
            {
                var vertexes = line.Split( ';' );
                Vertex<int> v1 = new Vertex<int>( int.Parse( vertexes[0] ) );
                Vertex<int> v2 = new Vertex<int>( int.Parse( vertexes[1] ) );
                Edge<int> edge = new Edge<int>( v1, v2 );
                graph.AddEdge( v1.ID, edge );
            }
            return graph;
        }

        private static void PrintMatrix(int[,] matrix)
        {
            for ( int i = 0; i < matrix.GetLength( 0 ); i++ )
            {
                for ( int j = 0; j < matrix.GetLength( 0 ); j++ )
                {
                    Console.Write( matrix[i, j] + " " );
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Erdos Renyi model
        /// </summary>
        /// <param name="n"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private static Graph<int> GenerateRandomGraph(int n, double p)
        {
            Graph<int> randGraph = new Graph<int>();
            int m = (int)(n * (n - 1) / 2 * p);

            for (int i = 1; i < n + 1; i++)
            {
                randGraph.Values[i] = new List<Edge<int>>();
            }
            
            Random rand = new Random();
            int e = 0;
            while (e < m)
            {
                var x1 = rand.Next(1, n + 1);
                var x2 = rand.Next(1, n + 1);
                if( x1 != x2) 
                {

                    Vertex<int> v1 = new Vertex<int>(x1);
                    Vertex<int> v2 = new Vertex<int>(x2);
                    Edge<int> edge = new Edge<int>(v1, v2);
                    Edge<int> edge2 = new Edge<int>(v2, v1);

                    if (!randGraph[x1].Contains(edge) && !randGraph[x1].Contains(edge2))
                    {
                        randGraph.AddEdge(v1.ID, edge);
                        //randGraph.AddEdge(v2.ID, edge);
                        e++;
                    }

                }
            }
            
            return randGraph;
        }

        /// <summary>
        /// Barabasi-Albert model, preferential attachment
        /// </summary>
        /// <param name="n"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        private static Graph<int> GenerateFreeScaleGraph(Graph<int> inputGraph, int n, int m = 2 )
        {
            Graph<int> freeScaleGraph = inputGraph;

            for (int i = inputGraph.Values.Count; i < n; i++)
            {
                //inputGraph.AddVertex();
                //int idLastAdded = inputGraph.Values.Keys.Last();
                //Vertex<int> toBeAdded = new Vertex<int>(idLastAdded);
                //Vertex<int> joinTo = new Vertex<int>(X);
                //Edge<int> edge = new Edge<int>(toBeAdded, joinTo);
                //freeScaleGraph.AddEdge(idLastAdded, edge);
                //freeScaleGraph.AddEdge(X, edge);
            }

            return freeScaleGraph;
        }

        /**
        *  n = max vertex count
        *  m = edge count, default 2
        */
        private static Graph<int> PreferentialAttachment(Graph<int> inputGraph, int n, int m = 2)
        {

            List<int> probabilityArray = new List<int>();

            for (int i = 1; i <= inputGraph.Values.Count; i++)
            {
                int degree = inputGraph.VertexDegree(i);

                for (int x = 0; x < degree; x++)
                {
                    probabilityArray.Add(i);
                }
            }


            Random rnd = new Random();
            for (int i = inputGraph.Values.Count; i < n; i++)
            {

                // pick random
                int rand1 = probabilityArray[rnd.Next(probabilityArray.Count)];
                int rand2 = probabilityArray[rnd.Next(probabilityArray.Count)];

                inputGraph.AddVertex();
                Vertex<int> v1 = new Vertex<int>(i);


                Vertex<int> v2 = new Vertex<int>(rand2);
                Edge<int> edge = new Edge<int>(v1, v2);
                inputGraph.AddEdge(v1.ID, edge);
                inputGraph.AddEdge(v2.ID, edge);

                Vertex<int> v3 = new Vertex<int>(rand1);
                Edge<int> edge2 = new Edge<int>(v1, v3);
                inputGraph.AddEdge(v1.ID, edge2);
                inputGraph.AddEdge(v3.ID, edge2);



            }


            return inputGraph;
        }

        private static void WriteToCSV(Dictionary<int, double> freqTable, string fileName)
        {
            string[] lines = new string[freqTable.Count];
            int i = 0;
            foreach (var row in freqTable)
            {
                lines[i] = row.Key + "," + row.Value;
                i++;
            }
            File.WriteAllLines(fileName, lines);
        }
    }
}
