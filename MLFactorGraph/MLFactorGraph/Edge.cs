using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLFactorGraph
{
    public class Edge : Factorable, ILayerNode
    {
        public uint Id { get; protected set; }

        private short label;
        public short Label
        {
            get
            {
                return label;
            }
            set
            {
                this.label = value;
                if (DualEdge != null)
                {
                    DualEdge.label = value;
                }
            }
        }

        public Node From { get; protected set; }
        public Node To { get; protected set; }

        public Dictionary<int, object> Attribute { get; set; }

        public Group Group { get; internal set; }

        public MLFGraph Graph { get; protected set; }

        public Edge DualEdge { get; internal set; }

        public Edge(MLFGraph graph, short label, Node from, Node to, Group group = null, Edge dualEdge = null)
            : base(graph, graph.DataSource,
                  delegate (Factorable f)
                  {
                      Edge e = f as Edge;
                      if (graph.BidirectionEdge)
                      {
                          return e.From.InEdge.Concat(e.To.OutEdge).Cast<Factorable>().ToList();
                      }
                      else
                      {
                          List<Edge> edges = e.From.InEdge.Concat(e.From.OutEdge).Concat(e.To.InEdge).Concat(e.To.OutEdge).ToList();
                          edges.RemoveAll(x => x == e);
                          return edges.Cast<Factorable>().ToList();
                      }
                  })
        {
            this.Id = graph.AllocateEdgeId();
            this.Label = label;

            this.From = from;
            this.To = to;

            this.Group = group;

            this.Graph = graph;

            this.DualEdge = dualEdge;

            from.OutEdge.Add(this);
            to.InEdge.Add(this);

            this.Attribute = new Dictionary<int, object>();
        }

        public void SetInvalid()
        {
            From.OutEdge.Remove(this);
            To.InEdge.Remove(this);

            if (this.Group != null)
            {
                this.Group.MemberEdge.Remove(this);
            }

            this.Graph = null;
        }
        public bool IsInvalid()
        {
            return this.Graph == null;
        }
    }
} 
