using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace REH0063_MAD1
{
    internal class Loader
    {
        public List<Videogame> csv(string filename)
        {
            var reader = new StreamReader(File.OpenRead(filename));
            List<Videogame> output = new List<Videogame>();
            reader.ReadLine(); //Get rid of the first line of CSV.

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                line = line.Replace("N/A", "0"); //Converting missing "N/A" values to 0 for further integer conversion.
                if (line.Contains('"')) //Fix for the commas and quotes in some videogames.
                {
                    line = line.Split('"', '"')[0] + line.Split('"', '"')[1].Replace(",", "") + line.Split('"', '"')[2];
                }
                var values = line.Split(','); //CSV Line splitting into separate values.

                Videogame result_line =
                    new Videogame(Convert.ToInt32(values[0]), values[1], values[2],
                    Convert.ToInt32(values[3]), values[4], values[5], Convert.ToDouble(values[6]),
                    Convert.ToDouble(values[7]), Convert.ToDouble(values[8]), Convert.ToDouble(values[9]),
                    Convert.ToDouble(values[10]));

                output.Add(result_line);
            }
            return output;
        }
    }
}