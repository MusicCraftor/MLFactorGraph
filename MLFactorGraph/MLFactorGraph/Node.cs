using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLFactorGraph
{
    public class Node : Factorable
    {
        public uint Id { get; protected set; }

        public Group Group { get; internal set; }

        public MLFGraph Graph { get; protected set; }

        public List<Edge> InEdge { get; protected set; }
        public List<Edge> OutEdge { get; protected set; }

        public Node(MLFGraph graph, Group group = null) : base(graph)
        {
            this.Id = graph.AllocateNodeId();

            this.Group = group;

            this.Graph = graph;

            this.InEdge = new List<Edge>();
            this.OutEdge = new List<Edge>();
        }

        public Node(Node v) : base(v.Graph)
        {
            this.Id = v.Id;
            this.Group = v.Group;
            this.Graph = v.Graph;
        }
    }
}
