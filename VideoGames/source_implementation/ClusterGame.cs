using System;

namespace REH0063_MAD1
{
    internal class ClusterGame
    {
        public const int _isnoise = -1; //Point classified as noise (DBSCAN)
        public const int _checked = 0;
        public double _NAsales, _EUsales;
        public int _cluster; // ID of cluster this game belongs in
        public string _name; // Name of the game

        public ClusterGame(double x, double y, string name)
        {
            this._NAsales = x;
            this._EUsales = y;
            this._name = name;
        }

        /// <summary>
        /// Export point in clustering into string format
        /// </summary>
        public override string ToString()
        {
            return String.Format("({0}, {1})", _NAsales, _EUsales);
        }
    }
}