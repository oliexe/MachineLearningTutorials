using System.Collections.Generic;
using System.Linq;

namespace cv8.Graph
{
    public class Graph<T>
    {
        /// <summary>
        /// Dictionary containing ID of vertex as key and list of its edges as value
        /// </summary>
        public Dictionary<int, List<Edge<T>>> Values { get; }
        public List<Edge<T>> this[int i]
        {
            get
            {
                return Values[i];
            }
        }

        public Graph()
        {
            this.Values = new Dictionary<int, List<Edge<T>>>();
        }

        public void AddEdge( int vertexID, Edge<T> edge )
        {
            if ( !Values.Keys.Contains( vertexID ) )
            {
                List<Edge<T>> edges = new List<Edge<T>>();
                edges.Add( edge );
                Values.Add( vertexID, edges );
            }
            else
                Values[vertexID].Add( edge );
        }

        public void AddVertex()
        {
            var index = this.Values.Count + 1;
            this.Values[index] = new List<Edge<T>>();
        }

        public int VertexDegree( int vertexID )
        {
            if ( Values.Keys.Contains( vertexID ) )
                return Values[vertexID].Count;
            else return -1;
        }

        public int MinimalVertexDegree()
        {
            List<int> degrees = new List<int>();
            foreach ( var vertex in Values )
            {
                degrees.Add( VertexDegree( vertex.Key ) );
            }
            return degrees.Min();
        }

        public int MaximalVertexDegree()
        {
            List<int> degrees = new List<int>();
            foreach ( var vertex in Values )
            {
                degrees.Add( VertexDegree( vertex.Key ) );
            }
            return degrees.Max();
        }

        public int AverageVertexDegree()
        {
            List<int> degrees = new List<int>();
            foreach ( var vertex in Values )
            {
                degrees.Add( VertexDegree( vertex.Key ) );
            }
            return degrees.Sum() / degrees.Count;
        }

        public Dictionary<int,double> AbsoluteFrequencyOfDegrees()
        {
            Dictionary<int, int> frequencyTable = new Dictionary<int, int>();
            foreach ( var vertex in Values )
            {
                frequencyTable[vertex.Key] = VertexDegree( vertex.Key );
            }
            var r = ( from row in frequencyTable
                      select row ).GroupBy( x => x.Value );
            Dictionary<int, double> freq = new Dictionary<int, double>();
            foreach ( var item in r )
            {
                freq[item.Key] = item.Count();
            }
            return freq;
        }

        public Dictionary<int,double> RelativeFrequencyOfDegrees()
        {
            Dictionary<int, int> frequencyTable = new Dictionary<int, int>();
            foreach ( var vertex in Values )
            {
                frequencyTable[vertex.Key] = VertexDegree( vertex.Key );
            }
            var r = ( from row in frequencyTable
                      select row ).GroupBy( x => x.Value );
            Dictionary<int, double> freq = new Dictionary<int, double>();
            foreach ( var item in r )
            {
                freq[item.Key] = (double)item.Count()/(double)frequencyTable.Count;
            }
            return freq;
        }

        /// <summary>
        /// Creates an matrix with size of undirected graph
        /// </summary>
        /// <returns>Adjacency matrix</returns>
        public int[,] CreateUndirectedAdjacencyMatrix()
        {
            int[,] matrix = new int[Values.Count, Values.Count];
            foreach ( var item in Values )
            {
                foreach ( var edge in item.Value )
                {
                    matrix[edge.VertexA.ID - 1, edge.VertexB.ID - 1] = 1;
                    matrix[edge.VertexB.ID - 1, edge.VertexA.ID - 1] = 1;
                }
            }
            return matrix;
        }

        public int[,] CreateDirectedAdjacencyMatrix()
        {
            int[,] matrix = new int[Values.Count, Values.Count];
            foreach ( var item in Values )
            {
                foreach ( var edge in item.Value )
                {
                    matrix[edge.VertexA.ID - 1, edge.VertexB.ID - 1] = 1;
                    //matrix[edge.VertexB.ID - 1, edge.VertexA.ID - 1] = 1;
                }
            }

            return matrix;
        }

        public int[,] FloydWarshallAlgorithm(int [,] incidencyMatrix)
        {
            int[,] distanceMatrix = (int[,])incidencyMatrix.Clone();
            InicializeDistanceMatrix(distanceMatrix);

            for (int k = 0; k < distanceMatrix.GetLength(0); k++)
            {
                for (int i = 0; i < distanceMatrix.GetLength(0); i++)
                {
                    for (int j = 0; j < distanceMatrix.GetLength(0); j++)
                    {
                        if (distanceMatrix[i, j] > distanceMatrix[i, k] + distanceMatrix[k, j])
                            distanceMatrix[i, j] = distanceMatrix[i, k] + distanceMatrix[k, j];
                    }
                }
            }
            return distanceMatrix;
        }

        private int[,] InicializeDistanceMatrix(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    if(matrix[i,j] == 0)
                        matrix[i, j] = int.MaxValue/2;
                    if (i == j)
                        matrix[i, j] = 0;
                }
            }
            return matrix;
        }
        
        //cv9
        public double MeanDistance(int[,] distanceMatrix, int i)
        {
            double distance = 0;
            int n = distanceMatrix.GetLength(0);
            for (int j = 0; j < n; j++)
            {
                distance += distanceMatrix[i, j];
            }
            return (double)distance/n;
        }

        public double GlobalMeanDistance(int[,] distanceMatrix)
        {
            int n = distanceMatrix.GetLength(0);
            int sum = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = i+1; j < n; j++)
                {
                    sum += distanceMatrix[i, j];
                }
            }
            return (double)2*sum/(n*(n-1));
        }

        public Dictionary<int, double> AbsoluteFrequencyOfDistances(int[,] distanceMatrix)
        {
            Dictionary<int, double> frequencies = new Dictionary<int, double>();
            int n = distanceMatrix.GetLength(0);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (!frequencies.Keys.Contains(distanceMatrix[i, j]))
                        frequencies[distanceMatrix[i, j]] = 1;
                    else
                        frequencies[distanceMatrix[i, j]]++;
                }
            }
            return frequencies;
        }

        public Dictionary<int, double> RelativeFrequencyOfDistances(int[,] distanceMatrix)
        {
            Dictionary<int, double> frequencies = new Dictionary<int, double>();
            int n = distanceMatrix.GetLength(0);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (!frequencies.Keys.Contains(distanceMatrix[i, j]))
                        frequencies[distanceMatrix[i, j]] = 1;
                    else
                        frequencies[distanceMatrix[i, j]]++;
                }
            }

            double total = frequencies.Values.Sum();
            for (int i = 0; i < frequencies.Count; i++)
            {
                frequencies[i] = frequencies[i] / total;
            }
            return frequencies;
        }

        public double ClossnessCentrality(int[,] distanceMatrix, int i)
        {
            double sum = 0;
            int n = distanceMatrix.GetLength(0);
            for (int j = 0; j < n; j++)
            {
                sum += distanceMatrix[i, j];
            }
            return (double)distanceMatrix.GetLength(0) / sum;
        }

        public double ClusterCoeficient(int vertexID)
        {
            var edges = Values[vertexID];

            int n = edges.Count;
            int m = 0;
            foreach (var edge in edges)
            {
                if (LinkExists(edge.VertexA.ID, edge.VertexB.ID))
                    m++;
            }
            return (double)(2 * m) / (n * (n - 1));
        }

        /// <summary>
        /// Checks wether link between two vertices exists
        /// Use when working with undirected graph
        /// </summary>
        /// <param name="vertexID1"></param>
        /// <param name="vertexID2"></param>
        /// <returns></returns>
        public bool LinkExists( int vertexID1, int vertexID2 )
        {
            bool exists = false;
            if(!Values.ContainsKey(vertexID1) || !Values.ContainsKey(vertexID2))
            {
                return false;
            }
            var edges = Values[vertexID1];
            foreach ( var edge in edges )
            {
                if ( edge.VertexA.ID == vertexID2 || edge.VertexB.ID == vertexID2 )
                    exists = true;
            }
            return exists;
        }

    }
}
