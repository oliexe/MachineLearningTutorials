using System;

namespace REH0063_MAD1
{
    internal class Point
    {
        public const int _noise = -1; //Point classified as noise (DBSCAN)
        public const int _unclass = 0;

        public double _X, _Y; // X,Y Coordinates
        public int _cluster; // ID of cluster this point belongs in
        public string _name; // Name of the game

        /// <summary>
        /// Point for clustering
        /// </summary>
        public Point(double x, double y, string name)
        {
            this._X = x;
            this._Y = y;
            this._name = name;
        }

        /// <summary>
        /// Export point in clustering into string format
        /// </summary>
        public override string ToString()
        {
            return String.Format("({0}, {1})", _X, _Y);
        }

        /// <summary>
        /// Squared distance between two points
        /// </summary>
        public static double Dist(Point p1, Point p2)
        {
            double diffX = p2._X - p1._X;
            double diffY = p2._Y - p1._Y;
            return diffX * diffX + diffY * diffY;
        }
    }
}