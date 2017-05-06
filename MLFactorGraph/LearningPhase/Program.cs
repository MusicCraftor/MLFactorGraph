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


            /*MobileMLFGraph graph = new MobileMLFGraph(mobileDatabase);
            Console.WriteLine("Graph builded");
            Console.ReadKey();

            Console.WriteLine(graph.FactorFunction());
            Console.ReadKey();*/


            /*MobileMLFGraph graph2 = new MobileMLFGraph(mobileDatabase);
            Console.WriteLine("Graph builded");
            Console.WriteLine(graph.FactorFunction());
            Console.ReadKey();*/


            /*object test1 = (int)1;
            object test2 = (int)1;
            Console.WriteLine(test1 == test2);
            Console.ReadKey();*/


            /*MobileMLFGraph dualGraph = new MobileMLFGraph(mobileDatabase, true);
            Console.WriteLine("Dual Graph builded");
            Console.ReadKey();

            Node from = dualGraph.NodeLayer[0];
            Node to = from.OutEdge[0].To;
            Edge e = from.OutEdge[0];
            Edge dualE = to.OutEdge[0];
            if (e.DualEdge == dualE)
            {
                Console.WriteLine("Dual Edge found");
                e.Label = MobileLabel.UNKNOWN;
                Console.WriteLine("Dual Edge Label changed to: " + dualE.Label);
                dualE.Label = MobileLabel.NORELATION;
                Console.WriteLine("Edge Label changed to: " + e.Label);
            }
            Console.ReadKey();

            Group g = dualGraph.GroupLayer[0];
            Console.WriteLine(dualGraph.AddGroup(MobileLabel.UNKNOWN, g.Member.Select(x => x.Id).ToList()) == null);
            Console.ReadKey();*/
        }
    }
}
