using OxyPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace REH0063_MAD1
{
    internal class db
    {
        /// <summary>
        /// Clears videogame dataset from incomplete values and converts them into Point list
        /// </summary>
        private static List<ClusterGame> ClearInput(List<Videogame> data)
        {
            List<Videogame> data_cleaned = new List<Videogame>();
            List<ClusterGame> points = new List<ClusterGame>();

            for (int i = 0; i < data.Count; i++)
            {
                if (data[i]._euSales > 0)
                {
                    data_cleaned.Add(data[i]);
                }
            }

            for (int i = 0; i < data_cleaned.Count; i++)
            {
                points.Add(new ClusterGame(data_cleaned[i]._naSales, data_cleaned[i]._euSales, data_cleaned[i]._name));
            }

            return points;
        }
      
        /// <summary>
        /// Run DBSCAN and generate output
        /// </summary>
        public db(List<Videogame> data, double eps, int minPts)
        {
            Console.WriteLine("DBSCAN Clustering....");
            List<ClusterGame> points = ClearInput(data);
            List<List<ClusterGame>> clusters = StartClustering(points, eps, minPts);

            //Generate output text file
            using (StreamWriter writetext = new StreamWriter("output/DBclusters.txt"))
            {
                //Create blank graph
                Graph graf = new Graph();

                //List of scatter points for graph
                List<double> pointsX = new List<double>();
                List<double> pointsY = new List<double>();
                int sum = 0;

                //Clusters
                for (int i = 0; i < clusters.Count; i++)
                {
                    int count = clusters[i].Count;
                    sum += count;

                    //Write cluster into text file.
                    writetext.WriteLine("\n Cluster :" + (i + 1) + " / " + count + " games :\n");
                    foreach (ClusterGame p in clusters[i])
                    {
                        writetext.Write("\n" + p + "  " + p._name);
                        pointsX.Add(p._NAsales);
                        pointsY.Add(p._EUsales);
                    }
                    writetext.WriteLine();

                    //Generate color for cluster and add to graph
                    byte[] color = graf.GetRandomColor();
                    graf.AddToGraph(pointsX, pointsY, i, color[0], color[1], color[2]);
                    pointsX.Clear();
                    pointsY.Clear();
                }
                sum = points.Count - sum;

                //Noise points
                if (sum > 0)
                {
                    //Write noise points into text file
                    writetext.WriteLine("\n" + sum + " games are noise\n");

                    foreach (ClusterGame p in points)
                    {
                        if (p._cluster == ClusterGame._isnoise)
                        {
                            writetext.Write("\n" + p + "  " + p._name);
                            pointsX.Add(p._NAsales);
                            pointsY.Add(p._EUsales);
                        }
                    }
                    //Add noise into graph with different marker (See graph class)
                    graf.AddToGraph(pointsX, pointsY, 900, 0, 0, 0, MarkerType.Cross);
                    writetext.WriteLine();
                    writetext.WriteLine();
                }
                graf.GenerateGraph("db");
            }
        }

        private static double square(double n)
        {
            return n *= n;
        }

        private static void MarkNoise(ClusterGame p)
        {
            p._cluster = ClusterGame._isnoise;
        }

        /// <summary>
        /// Measures distance between two points
        /// </summary>
        public static double Dist(ClusterGame x, ClusterGame y)
        {
            double diffNA = y._NAsales - x._NAsales;
            double diffEU = y._EUsales - x._EUsales;
            return diffNA * diffNA + diffEU * diffEU;
        }
        
        /// <summary>
        /// Returns list of points in selected region around the specified point
        /// </summary>
        private static List<ClusterGame> GetNeighbours(List<ClusterGame> points, ClusterGame p, double region)
        {
            List<ClusterGame> reg = new List<ClusterGame>();

            for (int i = 0; i < points.Count; i++)
            {
                double dist = Dist(p, points[i]);
                if (dist <= region)
                    reg.Add(points[i]);
            }
            return reg;
        }
        
        /// <summary>
        /// DBSCAN clustering function on the list of points
        /// </summary>
        private static List<List<ClusterGame>> StartClustering(List<ClusterGame> points, double region, int min_points)
        {
            region = square(region);
            int ClstId = 1;

            if (points == null) return null;
            List<List<ClusterGame>> clusters = new List<List<ClusterGame>>();

            for (int counter = 0; counter < points.Count; counter++)
            {
                ClusterGame p = points[counter];
                if (p._cluster == ClusterGame._checked) if (CheckCluster(points, p, ClstId, region, min_points)) ClstId++;
            }

            // Assign point to clusters
            int max = points.OrderBy
                (p => p._cluster).Last()._cluster;

            if (max < 1)
                return clusters;

            for (int i = 0; i < max; i++)
                clusters.Add(new List<ClusterGame>());

            foreach (ClusterGame p in points)
                if (p._cluster > 0) clusters[p._cluster - 1].Add(p);

            return clusters;
        }
       
        /// <summary>
        /// Check if the cluster can be expanded
        /// </summary>
        private static bool CheckCluster(List<ClusterGame> points, ClusterGame p, int clusterId, double region, int min_points)
        {
            List<ClusterGame> neighboursList = GetNeighbours(points, p, region);

            // mark point as NOISE if the count of neighbours is bellow treshold
            if (neighboursList.Count < min_points) 
            {
                MarkNoise(p);
                return false;
            }
            //Expand cluster if the count of neighbours is above treshold
            else
            {
                //Remove points from the list
                for (int i = 0; i < neighboursList.Count; i++) neighboursList[i]._cluster = clusterId;
                neighboursList.Remove(p);
                //Repeat for every neighbour point
                while (neighboursList.Count > 0)
                {
                    ClusterGame current = neighboursList[0];

                    List<ClusterGame> result = GetNeighbours(points, current, region);
                    if (result.Count >= min_points)
                    {
                        for (int i = 0; i < result.Count; i++)
                        {
                            ClusterGame res = result[i];
                            if (res._cluster == ClusterGame._checked || 
                                res._cluster == ClusterGame._isnoise)
                            {
                                if (res._cluster == ClusterGame._checked) neighboursList.Add(res);
                                res._cluster = clusterId;
                            }
                        }
                    }
                    neighboursList.Remove(current);
                }
                return true;
            }
        }
    }
}