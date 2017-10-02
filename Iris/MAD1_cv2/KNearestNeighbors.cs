using MAD.Data;
using MAD.Enum;
using MAD.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAD
{
    public class KNearestNeighbors
    {
        //Trainingové data
        private List<double[]> trainingSetValues = new List<double[]>();
        private List<string> trainingSetClasses = new List<string>();

        //Data pro kalsifikací
        private List<double[]> testSetValues = new List<double[]>();
        private List<string> testSetClasses = new List<string>();

        private int K;

        public void InitData(List<Iris> input, DataType dataType)
        {
            foreach (Iris line in input)
            {
                double[] lineDoubles = new double[] { line.sepallen, line.sepalwid, line.petallen, line.petalwid };

                if (dataType == DataType.TRAINING)
                {
                    this.trainingSetValues.Add(lineDoubles);
                    this.trainingSetClasses.Add(line.species);
                }
                else if (dataType == DataType.TEST)
                {
                    this.testSetValues.Add(lineDoubles);
                    this.testSetClasses.Add(line.species);
                }
            }
        }

        public void Classify(int neighborsNumber)
        {
            Console.WriteLine();
            ElucidanDistance ElucDist = new ElucidanDistance();

            this.K = neighborsNumber;
            double correct = 0, testN = 0;

            //Vytvořím si 2D pole pro uložení vzdáleností
            double[][] distances = new double[trainingSetValues.Count][];
            for (int i = 0; i < trainingSetValues.Count; i++)
                distances[i] = new double[2];

            // pro všechny prvky v tetsovací sadě
            for (var test = 0; test < this.testSetValues.Count; test++)
            {
                //paralelně počitám euclid z testovacího prvku na všechny prvky v training sadě
                Parallel.For(0, trainingSetValues.Count, index =>
                    {
                        var dist = ElucDist.Get(this.testSetValues[test], this.trainingSetValues[index]);
                        distances[index][0] = dist;
                        distances[index][1] = index;
                    }
                );

                Console.WriteLine("--VALUE {0} closest {1} neighbors--", test, this.K);

                // seřadit vzdalenosti ke všem sousedum v training sadě a vzít první K 
                var sortedDistances = distances.AsParallel().OrderBy(t => t[0]).Take(this.K);

                string realClass = testSetClasses[test];
                foreach (var d in sortedDistances)
                {
                    string predictedClass = trainingSetClasses[(int)d[1]];
                    Console.Write("PREDICTED: {0}", predictedClass);

                    if (string.Equals(realClass, predictedClass) == true){
                    Console.Write(" - TRUE");
                    correct++;
                    }

                    testN++;

                    if (string.Equals(realClass, predictedClass) != true)
                    {
                    Console.Write(" - WRONG!");
                    }

                    Console.WriteLine();
                }
                Console.WriteLine();
            }

            Console.WriteLine("TOTAL: {0}", testN);
            Console.WriteLine("CORRECT: {0}", correct);
            Console.WriteLine("{0}%", (correct / testN) * 100);

            Console.ReadLine();
            Console.Clear();

            Console.WriteLine();
        }
    }
}