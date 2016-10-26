using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAD1_cv2
{
    class Iris
    {
        public double sepallen;
        public double sepalwid;
        public double petallen;
        public double petalwid;
        public string species;

        public Iris(double sepallen, double sepalwid, double petallen, double petalwid, string species)
        {
            this.sepallen = sepallen;
            this.sepalwid = sepalwid;
            this.petallen = petallen;
            this.petalwid = petalwid;
            this.species = species;
        }
    }
}
