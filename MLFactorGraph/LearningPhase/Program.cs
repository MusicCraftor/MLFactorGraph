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

            //Console.WriteLine(graph.EdgeLayer[0].UnitaryFactorFunction());

            /*graph.OutputLambda();
            Console.ReadKey();*/

            /*graph.OutputEdgeFactor();
            Console.ReadKey()*/

            BPGraph bpGraph = graph.BeliefPropagation(1000, MLFGraph.Layer.EdgeLayer);

            var gradient = bpGraph.GetGradient();
            OutputListDict(gradient);
        }

        static void OutputListDict(List<Dictionary<short, double>> listDict)
        {
            int i = 0;
            foreach (Dictionary<short, double> dict in listDict)
            {
                Console.Write("Dict {0}", i++);
                foreach (KeyValuePair<short, double> pair in dict)
                {
                    Console.Write(" ({0}, {1})", pair.Key, pair.Value);
                }
                Console.WriteLine();
            }
        }
    }
}
