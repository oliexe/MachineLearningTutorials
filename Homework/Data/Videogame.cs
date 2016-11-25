using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.Data
{
    class Videogame
    {
        public int rank;
        public string name;
        public string platform;
        public int year;
        public string genre;
        public string publisher;
        public double naSales;
        public double euSales;
        public double jpSales;
        public double otherSales;
        public double globalSales;

        public Videogame(int rank, string name, string platform, int year, string genre, string publisher,
            double naSales, double euSales, double jpSales, double otherSales, double globalSales)
        {
            this.rank = rank;
            this.name = name;
            this.platform = platform;
            this.year = year;
            this.genre = genre;
            this.publisher = publisher;
            this.naSales = naSales;
            this.euSales = euSales;
            this.jpSales = jpSales;
            this.otherSales = otherSales;
            this.globalSales = globalSales;
        }
    }
}
