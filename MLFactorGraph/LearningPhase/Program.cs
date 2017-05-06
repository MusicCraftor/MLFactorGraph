using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

using MLFactorGraph;
using MobileMLFactorGraph;
using ResearchUtils;

namespace LearningPhase
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseConnection mobileDatabase = new DatabaseConnection("server=localhost;user id=research;password=research;database=fmobile_500");
            MobileMLFGraph graph = new MobileMLFGraph(mobileDatabase);
            Console.WriteLine("Graph builded");
            Console.ReadKey();

            Console.WriteLine(graph.FactorFunction());
            Console.ReadKey();

            MobileMLFGraph graph2 = new MobileMLFGraph(mobileDatabase);
            Console.WriteLine("Graph builded");
            Console.WriteLine(graph.FactorFunction());
            Console.ReadKey();

            /*object test1 = (int)1;
            object test2 = (int)1;
            Console.WriteLine(test1 == test2);
            Console.ReadKey();*/

        }
    }
}
