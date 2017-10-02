using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAD1
{
    public class ID3
    {
        private DataTable DataSet_temp;

        private int TrueNumber = 0;
    
        private int TotalLines = 0;

        private string SearchFor = "play";

        private double EntropyOfSet = 0.0;

        //Pocet všech pozitivních řádků (zbytek je negativní)
        private int Count(DataTable DataSet)
        {
            int result = 0;

            foreach (DataRow aRow in DataSet.Rows)
            {
                if ((bool)aRow[SearchFor] == true)
                    result++;
            }

            return result;
        }

        // Počet negativních/pozitivních v datasetu na určitem atributu
        private void getVal(DataTable DataSet, Attribute attribute, string value, out int pos, out int neg)
        {
            pos = 0;
            neg = 0;

            foreach (DataRow aRow in DataSet.Rows)
            {
                if (((string)aRow[attribute.AttributeName] == value))
                    if ((bool)aRow[SearchFor] == true)
                        pos++;
                    else
                        neg++;
            }
        }

        //vypočet entropie
        // -p+log2p+ - p-log2p-
        private double Entropy(int positives, int negatives)
        {

            int total = positives + negatives;

            if (total == 0)
            {
                return 0;
            }

            // -p+log2p+ - p-log2p-
            double WithTrue = (double)positives / total;
            double WithFalse = (double)negatives / total;
            if (WithTrue != 0)
                WithTrue = -(WithTrue) * System.Math.Log(WithTrue, 2);
            if (WithFalse != 0)
                WithFalse = -(WithFalse) * System.Math.Log(WithFalse, 2);


            double result = WithTrue + WithFalse;
            Console.WriteLine("E: " + result);
            return result;
        }
    
        // vypocet gainu
        // entropie setu - entropie hodnoty
        private double Gain(DataTable DataSet, Attribute attribute)
        {
            string[] val = attribute.values;
            double sum = 0.0;

            // entropy pro každý argument
            for (int i = 0; i < val.Length; i++)
            {
                int positives, negatives;
                positives = negatives = 0;
                getVal(DataSet, attribute, val[i], out positives, out negatives);
                double entropy = Entropy(positives, negatives);
                sum += -(double)(positives + negatives) / TotalLines * entropy;
            }

            //Vratit Gain Entropy setu - entropie hodnoty argumentu
            Console.Write("GAIN: ");
            Console.Write(EntropyOfSet + sum);
            Console.WriteLine();
            Console.WriteLine("---------------------------");

            return EntropyOfSet + sum;
        }

        //projet všechny atributy DataSetu (tabulky) a vratit ten s největším gainem
        private Attribute GetBestGain(DataTable DataSet, Attribute[] attributes)
        {
            double maxGain = 0.0;
            Attribute result = null;

            foreach (Attribute attribute in attributes)
            {
                double aux = Gain(DataSet, attribute);
                if (aux > maxGain)
                {
                    maxGain = aux;
                    result = attribute;
                }
            }
            return result;
        }

        //je set pouze pozitivní ?
        private bool IsPositive(DataTable DataSet, string targetAttribute)
        {
            foreach (DataRow row in DataSet.Rows)
            {
                if ((bool)row[targetAttribute] == false)
                    return false;
            }

            return true;
        }
        //je set pouze negativní ?
        private bool IsNegative(DataTable DataSet, string targetAttribute)
        {
            foreach (DataRow row in DataSet.Rows)
            {
                if ((bool)row[targetAttribute] == true)
                    return false;
            }

            return true;
        }


        private ArrayList getDistinctValues(DataTable DataSet, string targetAttribute)
        {
            ArrayList distinctValues = new ArrayList(DataSet.Rows.Count);

            foreach (DataRow row in DataSet.Rows)
            {
                if (distinctValues.IndexOf(row[targetAttribute]) == -1)
                    distinctValues.Add(row[targetAttribute]);
            }

            return distinctValues;
        }
        private object getMostCommonValue(DataTable DataSet, string targetAttribute)
        {
            ArrayList distinctValues = getDistinctValues(DataSet, targetAttribute);
            int[] count = new int[distinctValues.Count];

            foreach (DataRow row in DataSet.Rows)
            {
                int index = distinctValues.IndexOf(row[targetAttribute]);
                count[index]++;
            }

            int MaxIndex = 0;
            int MaxCount = 0;

            for (int i = 0; i < count.Length; i++)
            {
                if (count[i] > MaxCount)
                {
                    MaxCount = count[i];
                    MaxIndex = i;
                }
            }

            return distinctValues[MaxIndex];
        }



        private Node ID3Tree(DataTable DataSet, string Target, Attribute[] attributes)
        {
            //Entropie tabulky je 0.00 = JE LEAF 
            if (IsPositive(DataSet, Target) == true)
            {
                Node tmpNode = new Node(new Attribute(true));
                tmpNode.totalData = DataSet.Rows.Count;
                return tmpNode;
            }
            if (IsNegative(DataSet, Target) == true)
            {
                Node tmpNode = new Node(new Attribute(false));
                tmpNode.totalData = DataSet.Rows.Count;
                return tmpNode;
            }
   
            //OR
     
            TotalLines = 14;
            SearchFor = Target;
            TrueNumber = Count(DataSet);

            //vypočteme entropii setu
            Console.WriteLine("++++++++++++SET++++++++++++++");
            EntropyOfSet = Entropy(TrueNumber, TotalLines - TrueNumber);
            Console.WriteLine("+++++++++++++++++++++++++++++");
       
            //Najit "nejlepší atribut" - který má největší information gain v tabulce
            Attribute bestAttribute = GetBestGain(DataSet, attributes);
            Console.WriteLine("BEST GAIN IN SET: " + bestAttribute.AttributeName);
            Console.WriteLine();
 
            //Udělat z nejlepšího atributu kořen (node)
            Node root = new Node(bestAttribute);
            root.totalData = TotalLines;
            DataTable aSample = DataSet.Clone();


                //Cyklus pro každou možnou hodnotu kterou muže nabývat "Best" atribut
                foreach (string value in bestAttribute.values)
                {
                    //Všechny řádky atributu nabývající danou hodnotu
                    aSample.Rows.Clear();
                    DataRow[] rows = DataSet.Select(bestAttribute.AttributeName + " = " + "'" + value + "'");
                    foreach (DataRow row in rows)
                    {
                        aSample.Rows.Add(row.ItemArray);
                    }

                   // nový list atributu bez "best gain" attributu
                ArrayList aAttributes = new ArrayList(attributes.Length - 1);
                    for (int i = 0; i < attributes.Length; i++)
                    {
                        if (attributes[i].AttributeName != bestAttribute.AttributeName)
                            aAttributes.Add(attributes[i]);
                    }
                 
                  //Rekurze ID3 algoritmu na novou pod-tabulku (omezený DataSet)  
                  //Vkládat node do nodu vytvořeného z best atributu
                  ID3 nextTree = new ID3();
                  Node ChildNode = nextTree.makeTree(aSample, Target, (Attribute[])aAttributes.ToArray(typeof(Attribute)));
                  root.AddTreeNode(ChildNode, value);                 
                }
            
            return root;
        }

        public Node makeTree
            (DataTable DataSet, string targetAttribute, Attribute[] attributes)
        {
            DataSet_temp = DataSet;
            return ID3Tree(DataSet_temp, targetAttribute, attributes);
        }
    }
}
