using OxyPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace REH0063_MAD1
{
    internal class dbscan
    {
        public dbscan(List<Videogame> data)
        {
            Console.WriteLine("DBSCAN Clustering....");
            List<Videogame> data_cleaned = new List<Videogame>();
            List<Point> points = new List<Point>();

            // Throw off uncomplete data
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].euSales > 0)
                {
                    data_cleaned.Add(data[i]);
                }
            }

            for (int i = 0; i < data_cleaned.Count; i++)
            {
                points.Add(new Point(data_cleaned[i].naSales, data_cleaned[i].euSales, data_cleaned[i].name));
            }

            //eps = 1.0 a minPts=2
            double eps = 1.0;
            int minPts = 2;

            List<List<Point>> clusters = GetClusters(points, eps, minPts);
            Console.Clear();
            Graph graf = new Graph();
            Console.WriteLine();

            List<double> pointsX = new List<double>();
            List<double> pointsY = new List<double>();
            // print clusters to console

            using (StreamWriter writetext = new StreamWriter("output/DBclusters.txt"))
            {
            int total = 0;
            for (int i = 0; i < clusters.Count; i++)
            {
                int count = clusters[i].Count;
                total += count;
                writetext.WriteLine("\nCluster {0} consists of {1} games :\n", i + 1, count);
                foreach (Point p in clusters[i])
                {
                        writetext.Write("\n {0} " + p.Name, p);
                    pointsX.Add(p.X);
                    pointsY.Add(p.Y);
                }
                    writetext.WriteLine();

                byte[] color = graf.GetRandomColor();
                graf.AddToGraph(pointsX, pointsY, i, color[0], color[1], color[2]);
                pointsX.Clear();
                pointsY.Clear();
            }

            // print any points which are NOISE
            total = points.Count - total;
            if (total > 0)
            {
                    writetext.WriteLine("+++++++++++++++++++++++++++++++++++++++++");
                    writetext.WriteLine("\n {0} games without cluster (noise) :\n", total);
                foreach (Point p in points)
                {
                    if (p.ClusterId == Point.NOISE)
                    {
                        writetext.WriteLine("{0} " + p.Name, p);
                        pointsX.Add(p.X);
                        pointsY.Add(p.Y);
                    }
                }
                    graf.AddToGraph(pointsX, pointsY, 900, 0,0,0, MarkerType.Cross);
                    writetext.WriteLine();
                    writetext.WriteLine();
                }
            }
            graf.GenerateGraph("db");
        }

        private static List<List<Point>> GetClusters(List<Point> points, double eps, int minPts)
        {
            if (points == null) return null;
            List<List<Point>> clusters = new List<List<Point>>();
            eps *= eps; // square eps
            int clusterId = 1;
            for (int i = 0; i < points.Count; i++)
            {
                Point p = points[i];
                if (p.ClusterId == Point.UNCLASSIFIED)
                {
                    if (ExpandCluster(points, p, clusterId, eps, minPts)) clusterId++;
                }
            }
            // sort out points into their clusters, if any
            int maxClusterId = points.OrderBy(p => p.ClusterId).Last().ClusterId;
            if (maxClusterId < 1) return clusters; // no clusters, so list is empty
            for (int i = 0; i < maxClusterId; i++) clusters.Add(new List<Point>());
            List<double> pointsX = new List<double>();
            List<double> pointsY = new List<double>();

            foreach (Point p in points)
            {
                if (p.ClusterId > 0) clusters[p.ClusterId - 1].Add(p);
            }

            return clusters;
        }

        private static List<Point> GetRegion(List<Point> points, Point p, double eps)
        {
            List<Point> region = new List<Point>();
            for (int i = 0; i < points.Count; i++)
            {
                double distSquared = Point.DistanceSquared(p, points[i]);
                if (distSquared <= eps) region.Add(points[i]);
            }
            return region;
        }

        private static bool ExpandCluster(List<Point> points, Point p, int clusterId, double eps, int minPts)
        {
            List<Point> seeds = GetRegion(points, p, eps);
            if (seeds.Count < minPts) // no core point
            {
                p.ClusterId = Point.NOISE;
                return false;
            }
            else // all points in seeds are density reachable from point 'p'
            {
                for (int i = 0; i < seeds.Count; i++) seeds[i].ClusterId = clusterId;
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
                            if (resultP.ClusterId == Point.UNCLASSIFIED || resultP.ClusterId == Point.NOISE)
                            {
                                if (resultP.ClusterId == Point.UNCLASSIFIED) seeds.Add(resultP);
                                resultP.ClusterId = clusterId;
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