using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace karate.dataset
{
    class Node
    {
        public int number;
        public List<int> sousedi = new List<int>();

        public Node(int number)
        {
            this.number = number;
        }

        public void AddNeighbour(int neigh)
        {
            sousedi.Add(neigh);
        }

        public void Print()
        {
            Console.Write(number + " ==> ");

            foreach (int item in sousedi)
            {
                Console.Write(item + " ");
            }

            Console.Write( " (Stupen " + GetDegree() + ") ");
            Console.WriteLine();
        }

        public int GetDegree()
        {
            return sousedi.Count();
        }
    }
}
