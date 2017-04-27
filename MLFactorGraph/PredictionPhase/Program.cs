using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MLFactorGraph;

namespace PredictionPhase
{
    class Program
    {
        static void Main(string[] args)
        {
            // Test MLFGraph
            MLFGraph graph = new MLFGraph();
            Node node1 = new Node(graph);
            graph.test = 1111;
            Console.WriteLine(graph.test);
            Console.WriteLine(node1.Graph.test);

            Console.ReadKey();
        }
    }
}
