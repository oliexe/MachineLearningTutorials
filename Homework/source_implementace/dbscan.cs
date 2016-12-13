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
        /// Run DBSCAN and generate output
        /// </summary>
        public db(List<Videogame> data, double eps, int minPts)
        {
            Console.WriteLine("DBSCAN Clustering....");
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
            List<List<Point>> clusters = GetClusters(points, eps, minPts);

            //Generate graph and output files...
            using (StreamWriter writetext = new StreamWriter("output/DBclusters.txt"))
            {
                Graph graf = new Graph();

                List<double> pointsX = new List<double>();
                List<double> pointsY = new List<double>();

                int total = 0;
                for (int i = 0; i < clusters.Count; i++)
                {
                    int count = clusters[i].Count;
                    total += count;
                    writetext.WriteLine("\nCluster {0} - {1} games :\n", i + 1, count);
                    foreach (Point p in clusters[i])
                    {
                        writetext.Write("\n {0} " + p._name, p);
                        pointsX.Add(p._X);
                        pointsY.Add(p._Y);
                    }
                    writetext.WriteLine();

                    byte[] color = graf.GetRandomColor();
                    graf.AddToGraph(pointsX, pointsY, i, color[0], color[1], color[2]);
                    pointsX.Clear();
                    pointsY.Clear();
                }
                total = points.Count - total;
                if (total > 0)
                {
                    writetext.WriteLine("\n {0} games as noise :\n", total);
                    foreach (Point p in points)
                    {
                        if (p._cluster == Point._noise)
                        {
                            writetext.WriteLine("{0} " + p._name, p);
                            pointsX.Add(p._X);
                            pointsY.Add(p._Y);
                        }
                    }
                    graf.AddToGraph(pointsX, pointsY, 900, 0, 0, 0, MarkerType.Cross);
                    writetext.WriteLine();
                    writetext.WriteLine();
                }
                graf.GenerateGraph("db");
            }
        }

        /// <summary>
        /// DBSCAN clustering function on the list of points
        /// </summary>
        private static List<List<Point>> GetClusters(List<Point> points, double eps, int minPts)
        {
            if (points == null) return null;
            List<List<Point>> clusters = new List<List<Point>>();
            eps *= eps; // square eps
            int clusterId = 1;
            for (int i = 0; i < points.Count; i++)
            {
                Point p = points[i];
                if (p._cluster == Point._unclass)
                {
                    if (ExpandCluster(points, p, clusterId, eps, minPts)) clusterId++;
                }
            }
            // sort out points into their clusters, if any
            int maxClusterId = points.OrderBy(p => p._cluster).Last()._cluster;
            if (maxClusterId < 1) return clusters; // no clusters, so list is empty
            for (int i = 0; i < maxClusterId; i++) clusters.Add(new List<Point>());

            foreach (Point p in points)
            {
                if (p._cluster > 0) clusters[p._cluster - 1].Add(p);
            }

            return clusters;
        }

        private static List<Point> GetRegion(List<Point> points, Point p, double eps)
        {
            List<Point> region = new List<Point>();
            for (int i = 0; i < points.Count; i++)
            {
                double dist = Point.Dist(p, points[i]);
                if (dist <= eps) region.Add(points[i]);
            }
            return region;
        }

        private static bool ExpandCluster(List<Point> points, Point p, int clusterId, double eps, int minPts)
        {
            List<Point> seeds = GetRegion(points, p, eps);
            if (seeds.Count < minPts) // no core point
            {
                p._cluster = Point._noise;
                return false;
            }
            else // all points in seeds are density reachable from point 'p'
            {
                for (int i = 0; i < seeds.Count; i++) seeds[i]._cluster = clusterId;
                seeds.Remove(p);
                while (seeds.Count > 0)
                {
                    Point currentP = seeds[0];
                    List<Point> result = GetRegion(points, currentP, eps);
                    if (result.Count >= minPts)
                    {
                        for (int i = 0; i < result.Count; i++)
                        {
                            Point resultP = result[i];
                            if (resultP._cluster == Point._unclass || resultP._cluster == Point._noise)
                            {
                                if (resultP._cluster == Point._unclass) seeds.Add(resultP);
                                resultP._cluster = clusterId;
                            }
                        }
                    }
                    seeds.Remove(currentP);
                }
                return true;
            }
        }
    }
}