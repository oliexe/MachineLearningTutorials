using System;

namespace REH0063_MAD1
{
    internal class Point
    {
        public const int NOISE = -1;
        public const int UNCLASSIFIED = 0;
        public double X, Y;
        public int ClusterId;
        public string Name;

        public Point(double x, double y, string Name)
        {
            this.X = x;
            this.Y = y;
            this.Name = Name;
        }

        public override string ToString()
        {
            return String.Format("({0}, {1})", X, Y);
        }

        public static double DistanceSquared(Point p1, Point p2)
        {
            double diffX = p2.X - p1.X;
            double diffY = p2.Y - p1.Y;
            return diffX * diffX + diffY * diffY;
        }
    }
}