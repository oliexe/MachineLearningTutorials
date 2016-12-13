using System;
using System.Collections.Generic;
using System.IO;

namespace REH0063_MAD1
{
    internal class qt
    {
        /// <summary>
        /// Run QT and generate output
        /// </summary>
        public qt(List<Videogame> data, int maxDiameter)
        {
            Console.WriteLine("QR Clustering....");
            List<Videogame> data_cleaned = new List<Videogame>();
            List<Point> points = new List<Point>();

            // Throw off uncomplete data with 0 values - not to mess up clustering
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i]._euSales > 0)
                {
                    data_cleaned.Add(data[i]);
                }
            }

            // Convert cleaned data into "Point" structure used for clustering
            for (int i = 0; i < data_cleaned.Count; i++)
            {
                points.Add(new Point(data_cleaned[i]._naSales, data_cleaned[i]._euSales, data_cleaned[i]._name));
            }

            //BEGIN CLUSTERING!
            List<List<Point>> clusters = GetClusters(points, maxDiameter);

            //Generate graph and output files...
            using (StreamWriter writetext = new StreamWriter("output/QTclusters.txt"))
            {
                Graph graf = new Graph();
                List<double> pointsX = new List<double>();
                List<double> pointsY = new List<double>();

                for (int i = 0; i < clusters.Count; i++)
                {
                    int count = clusters[i].Count;
                    writetext.WriteLine("\nCluster {0} consists of {1} games :\n", i + 1, count);
                    foreach (Point p in clusters[i])
                    {
                        writetext.WriteLine(" {0} " + p._name, p);
                        pointsX.Add(p._X);
                        pointsY.Add(p._Y);
                    }

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
        private static List<List<Point>> GetClusters(List<Point> points, double maxDiameter)
        {
            points = new List<Point>(points);
            List<List<Point>> clusters = new List<List<Point>>();

            while (points.Count > 0)
            {
                List<Point> bestCandidate = GetBestCandidate(points, maxDiameter);
                clusters.Add(bestCandidate);
            }
            return clusters;
        }

        /*
            GetBestCandidate() returns first candidate cluster encountered if there is more than one
            with the maximum number of points.
        */
        private static List<Point> GetBestCandidate(List<Point> points, double maxDiameter)
        {
            double maxDiameterSquared = maxDiameter * maxDiameter; // square maximum diameter
            List<List<Point>> candidates = new List<List<Point>>(); // stores all candidate clusters
            List<Point> currentCandidate = null; // stores current candidate cluster
            int[] candidateNumbers = new int[points.Count]; // keeps track of candidate numbers to which points have been allocated
            int totalPointsAllocated = 0; // total number of points already allocated to candidates
            int currentCandidateNumber = 0; // current candidate number
            for (int i = 0; i < points.Count; i++)
            {
                if (totalPointsAllocated == points.Count) break; // no need to continue further
                if (candidateNumbers[i] > 0) continue; // point already allocated to a candidate
                currentCandidateNumber++;
                currentCandidate = new List<Point>(); // create a new candidate cluster
                currentCandidate.Add(points[i]); // add the current point to it
                candidateNumbers[i] = currentCandidateNumber;
                totalPointsAllocated++;
                Point latestPoint = points[i]; // latest point added to current cluster
                double[] diametersSquared = new double[points.Count]; // diameters squared of each point when added to current cluster
                                                                      // iterate through any remaining points
                                                                      // successively selecting the point closest to the group until the threshold is exceeded
                while (true)
                {
                    if (totalPointsAllocated == points.Count) break; // no need to continue further
                    int closest = -1; // index of closest point to current candidate cluster
                    double minDiameterSquared = Int32.MaxValue; // minimum diameter squared, initialized to impossible value
                    for (int j = i + 1; j < points.Count; j++)
                    {
                        if (candidateNumbers[j] > 0) continue; // point already allocated to a candidate
                                                               // update diameters squared to allow for latest point added to current cluster
                        double distSquared = Point.Dist(latestPoint, points[j]);
                        if (distSquared > diametersSquared[j]) diametersSquared[j] = distSquared;
                        // check if closer than previous closest point
                        if (diametersSquared[j] < minDiameterSquared)
                        {
                            minDiameterSquared = diametersSquared[j];
                            closest = j;
                        }
                    }

                    // if closest point is within maxDiameter, add it to the current candidate and mark it accordingly
                    if ((double)minDiameterSquared <= maxDiameterSquared)
                    {
                        currentCandidate.Add(points[closest]);
                        candidateNumbers[closest] = currentCandidateNumber;
                        totalPointsAllocated++;
                    }
                    else // otherwise finished with current candidate
                    {
                        break;
                    }
                }
                // add current candidate to candidates list
                candidates.Add(currentCandidate);
            }
            // now find the candidate cluster with the largest number of points
            int maxPoints = -1; // impossibly small value
            int bestCandidateNumber = 0; // ditto
            for (int i = 0; i < candidates.Count; i++)
            {
                if (candidates[i].Count > maxPoints)
                {
                    maxPoints = candidates[i].Count;
                    bestCandidateNumber = i + 1; // counting from 1 rather than 0
                }
            }
            // iterating backwards to avoid indexing problems, remove points in best candidate from points list
            // this will automatically be persisted to caller as List<Point> is a reference type
            for (int i = candidateNumbers.Length - 1; i >= 0; i--)
            {
                if (candidateNumbers[i] == bestCandidateNumber) points.RemoveAt(i);
            }

            // return best candidate to caller
            return candidates[bestCandidateNumber - 1];
        }
    }
}