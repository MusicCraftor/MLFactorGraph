using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLFactorGraph
{
    public class Edge : Factorable
    {
        public uint Id { get; protected set; }
        public short Label { get; set; }

        public Node From { get; protected set; }
        public Node To { get; protected set; }

        public Group Group { get; internal set; }

        public MLFGraph Graph { get; protected set; }

        public Edge(MLFGraph graph, short label, Node from, Node to, Group group = null) : base(graph)
        {
            this.Id = graph.AllocateEdgeId();
            this.Label = label;

            this.From = from;
            this.To = to;

            this.Group = group;

            this.Graph = graph;

            from.OutEdge.Add(this);
            to.InEdge.Add(this);
        }
    }
} 
