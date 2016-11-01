using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAD.Helpers
{
    public class ElucidanDistance
    {
        public double Get(MeansPoint dataPoint, MeansPoint mean)
        {
            double _diffs = 0.0;
            _diffs = Math.Pow(dataPoint.Width - mean.Width, 2);
            _diffs += Math.Pow(dataPoint.Length - mean.Length, 2);
            return Math.Sqrt(_diffs);
        }
    }
}
