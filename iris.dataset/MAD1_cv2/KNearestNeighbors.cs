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
        private List<double[]> trainingSetValues = new List<double[]>();
        private List<string> trainingSetClasses = new List<string>();

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

            // create an array where we store the distance from our test data and the training data -> [0]
            // plus the index of the training data element -> [1]
            double[][] distances = new double[trainingSetValues.Count][];

            for (int i = 0; i < trainingSetValues.Count; i++)
                distances[i] = new double[2];

            // start computing
            for (var test = 0; test < this.testSetValues.Count; test++)
            {
                Parallel.For(0, trainingSetValues.Count, index =>
                    {
                        var dist = ElucDist.Get(this.testSetValues[test], this.trainingSetValues[index]);
                        distances[index][0] = dist;
                        distances[index][1] = index;
                    }
                );

                Console.WriteLine("--VALUE {0} closest {1} neighbors--", test, this.K);

                // sort and select first K of them
                var sortedDistances = distances.AsParallel().OrderBy(t => t[0]).Take(this.K);

                string realClass = testSetClasses[test];

                // print and check the result
                foreach (var d in sortedDistances)
                {
                    string predictedClass = trainingSetClasses[(int)d[1]];
                    Console.WriteLine("REAL: {0}    PREDICTED: {1}", realClass, predictedClass);
                }
                Console.WriteLine();
            }

            Console.WriteLine();
        }
    }
}