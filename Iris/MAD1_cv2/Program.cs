using MAD.Data;
using System.Collections.Generic;

namespace MAD
{
    internal class Program
    {
        private static ConsolePrint output = new ConsolePrint();

        private static void Main(string[] args)
        {
            List<Iris> dataset = output.InitData(@"C:\iris.csv");
            output.InitStruct(dataset);

            output.CV4();
            output.CV5();
            output.CV6();
            output.CV7();
        }
    }
}