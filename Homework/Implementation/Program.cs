using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Homework.Data;
using Homework.Helpers;

namespace Homework
{
    class Program
    {
        static void Main(string[] args)
        {
            Loader load = new Loader();
            List<Videogame> data = load.csv(@"C:\vgsales.csv");
        }
    }
}
