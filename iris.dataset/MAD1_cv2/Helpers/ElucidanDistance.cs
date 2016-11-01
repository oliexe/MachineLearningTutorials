using MAD.Data;
using System;

namespace MAD.Helpers
{
    ///<summary>
    ///Elucidan Distance between two specific points in the dataset.
    ///</summary>
    public class ElucidanDistance
    {
        public double Get(MeansPoint dataPoint, MeansPoint mean)
        {
            double _diffs = 0.0;
            _diffs = Math.Pow(dataPoint.Width - mean.Width, 2);
            _diffs += Math.Pow(dataPoint.Length - mean.Length, 2);
            return Math.Sqrt(_diffs);
        }

        public double Get(double[] pointOne, double[] pointTwo)
        {
            double d = 0.0;

            for (int i = 0; i < pointOne.Length; i++)
            {
                double temp = pointOne[i] - pointTwo[i];
                d += temp * temp;
            }
            return Math.Sqrt(d);
        }

        public double Get(Iris a, Iris b)
        {
            double dist = Math.Sqrt(((a.sepallen - b.sepallen) * (a.sepallen - b.sepallen) +
                (a.sepalwid - b.sepalwid) * (a.sepalwid - b.sepalwid) +
                (a.petallen - b.petallen) * (a.petallen - b.petallen) +
                (a.petalwid - b.petalwid) * (a.petalwid - b.petalwid)
                ));
            return dist;
        }
    }
}