// Pouze pro testovani jineho clusterovani, Implementace podle: c-sharpcorner.com
using System;
using System.Collections.Generic;
using System.IO;

namespace REH0063_MAD1
{
    internal class qt
    {
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
        /// Returns cluster with the biggest amount of point aka best candidate
        /// </summary>
        private static List<ClusterGame> BestCluster(List<ClusterGame> points, double maxDiameter)
        {
            double diametersqrd = maxDiameter * maxDiameter;
            List<List<ClusterGame>> c = new List<List<ClusterGame>>();
            List<ClusterGame> cc = null;

            int[] candidateNumbers = new int[points.Count];
            int totalPointsAllocated = 0;
            int currentCandidateNumber = 0;

            for (int i = 0; i < points.Count; i++)
            {
                if (totalPointsAllocated == points.Count) break;
                if (candidateNumbers[i] > 0) continue;
                currentCandidateNumber++;
                cc = new List<ClusterGame>();
                cc.Add(points[i]);
                candidateNumbers[i] = currentCandidateNumber;
                totalPointsAllocated++;
                ClusterGame latestPoint = points[i];
                double[] diametersSquared = new double[points.Count];


                while (true)
                {
                    if (totalPointsAllocated == points.Count) break;
                    int closest = -1;
                    double minDiameterSquared = Int32.MaxValue;
                    for (int j = i + 1; j < points.Count; j++)
                    {
                        if (candidateNumbers[j] > 0) continue;

                        double distSquared = Dist(latestPoint, points[j]);
                        if (distSquared > diametersSquared[j]) diametersSquared[j] = distSquared;

                        if (diametersSquared[j] < minDiameterSquared)
                        {
                            minDiameterSquared = diametersSquared[j];
                            closest = j;
                        }
                    }


                    if ((double)minDiameterSquared <= diametersqrd)
                    {
                        cc.Add(points[closest]);
                        candidateNumbers[closest] = currentCandidateNumber;
                        totalPointsAllocated++;
                    }
                    else
                    {
                        break;
                    }
                }
                c.Add(cc);
            }
            int maxPoints = -1;
            int bestCandidateNumber = 0;
            for (int i = 0; i < c.Count; i++)
            {
                if (c[i].Count > maxPoints)
                {
                    maxPoints = c[i].Count;
                    bestCandidateNumber = i + 1;
                }
            }
            for (int i = candidateNumbers.Length - 1; i >= 0; i--)
            {
                if (candidateNumbers[i] == bestCandidateNumber) points.RemoveAt(i);
            }
            return c[bestCandidateNumber - 1];
        }

        /// <summary>
        /// Run QT and generate output
        /// </summary>
        public qt(List<Videogame> data, int maxDiameter)
        {
            Console.WriteLine("QR Clustering....");

            List<Videogame> data_cleaned = new List<Videogame>();
            List<ClusterGame> points = ClearInput(data);
            List<List<ClusterGame>> clusters = GetClusters(points, maxDiameter);

            //Generate graph and output files...
            using (StreamWriter writetext = new StreamWriter("output/QTclusters.txt"))
            {
                //Empty graph and list for points
                Graph graf = new Graph();
                List<double> pointsX = new List<double>();
                List<double> pointsY = new List<double>();

                for (int i = 0; i < clusters.Count; i++)
                {
                    int count = clusters[i].Count;
                    writetext.WriteLine("\n Cluster :" + (i + 1) + " / " + count + " games :\n");
                    foreach (ClusterGame p in clusters[i])
                    {
                        writetext.Write("\n" + p + "  " + p._name);
                        pointsX.Add(p._NAsales);
                        pointsY.Add(p._EUsales);
                    }

                    //Random color for a cluster and add to graph...
                    byte[] color = graf.GetRandomColor();
                    graf.AddToGraph(pointsX, pointsY, i, color[0], color[1], color[2]);
                    pointsX.Clear();
                    pointsY.Clear();

                    writetext.WriteLine();
                }
                graf.GenerateGraph("qt");
            }
        }

        /// <summary>
        /// QT clustering function on the list of points
        /// </summary>
        private static List<List<ClusterGame>> GetClusters(List<ClusterGame> points, double maxDiameter)
        {
            points = new List<ClusterGame>(points);
            List<List<ClusterGame>> clusters = new List<List<ClusterGame>>();

            while (points.Count > 0)
            {
                List<ClusterGame> bestCandidate = BestCluster(points, maxDiameter);
                clusters.Add(bestCandidate);
            }
            return clusters;
        }

    }
}