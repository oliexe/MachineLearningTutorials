using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Data;
using System.IO;

namespace MAD1
{
    class MAD1_du2
    {
        //Text na bool
        public static bool isCSVBool(string isbool)
        {
            if (isbool == "yes")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Nacist CSV
        public static DataTable loadCSV()
        {
            DataTable result = new DataTable("samples");

            DataColumn column = result.Columns.Add("Outlook");
            column.DataType = typeof(string);

            column = result.Columns.Add("Temperature");
            column.DataType = typeof(string);

            column = result.Columns.Add("Humidity");
            column.DataType = typeof(string);

            column = result.Columns.Add("Windy");
            column.DataType = typeof(string);

            column = result.Columns.Add("play");
            column.DataType = typeof(bool);

            var reader = new StreamReader(File.OpenRead(@"C:\Users\olire\Desktop\weather_nominal.csv"));
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');
                result.Rows.Add(new object[] { values[0], values[1], values[2], values[3].ToLower(), isCSVBool(values[4])});
            }

            return result;

        }

        //Tisk stromu
        public static void printNode(Node root, string tabs)
        {
            Console.WriteLine(tabs + '|' + root.attribute + '|');

            if (root.attribute.values != null)
            {
                for (int i = 0; i < root.attribute.values.Length; i++)
                {
                    Console.WriteLine(tabs + "\t" + "<" + root.attribute.values[i] + ">");
                    Node childNode = root.getChildByBranchName(root.attribute.values[i]);
                    printNode(childNode, "\t" + tabs);
                }
            }
        }

        static void Main(string[] args)
        {
            //Outlook
            Attribute outlook = new Attribute("Outlook", 
                new string[] { "sunny", "overcast", "rainy" });
            //Temperature
            Attribute temp = new Attribute("Temperature", 
                new string[] { "hot", "mild", "cool" });
            //Humidity
            Attribute humidity = new Attribute("Humidity", 
                new string[] { "high", "normal" });
            //Windy
            Attribute windy = new Attribute("Windy", 
                new string[] { "true", "false" });

            //načíst data z CSV
            Attribute[] attributes = new Attribute[] { outlook, temp, humidity, windy };
            DataTable samples = loadCSV();
       
            //ID3 na data
            ID3 id3 = new ID3();
            Node root = id3.makeTree(samples, "play", attributes);
            Console.ReadKey();

            printNode(root, "--");
            Console.ReadKey();
        }
    }
}