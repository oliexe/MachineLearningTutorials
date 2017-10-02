using System;
using System.IO;

namespace MAD.Helpers
{
    internal class CSV
    {
        /// <summary>
        /// CSV generator "helper" funtion.
        /// </summary>
        public void generate(double[,] source, string filename)
        {
            using (StreamWriter writer =
            new StreamWriter(filename))
            {
                double[,] x = source;

                for (int i = 0; i < 150; i++)
                {
                    for (int z = 0; z < 150; z++)
                    {
                        writer.Write(x[i, z] + ",");
                    }
                    writer.WriteLine();
                }
            }
            Console.WriteLine(filename + " generated!");
        }
    }
}