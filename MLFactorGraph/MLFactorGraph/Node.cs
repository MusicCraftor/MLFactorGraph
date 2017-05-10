using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLFactorGraph
{
    public class Node : Factorable, ILayerNode
    {
        public uint Id { get; protected set; }

        public short Label { get; set; }

        public Group Group { get; internal set; }

        public MLFGraph Graph { get; protected set; }

        public List<Edge> InEdge { get; protected set; }
        public List<Edge> OutEdge { get; protected set; }

        public Dictionary<int, object> Attribute { get; set; }

        public Node(MLFGraph graph, Group group = null) 
            : base(graph, graph.DataSource,
                  delegate (Factorable f)
                  {
                      Node v = f as Node;
                      List<Node> nodes = new List<Node>();
                      foreach (Edge e in v.InEdge)
                      {
                          nodes.Add(e.From);
                      }
                      foreach (Edge e in v.OutEdge)
                      {
                          nodes.Add(e.To);
                      }
                      nodes = nodes.Distinct().ToList();
                      nodes.Remove(v);
                      return nodes.Cast<Factorable>().ToList();
                  })
        {
            this.Id = graph.AllocateNodeId();

            this.Group = group;

            this.Graph = graph;

            this.InEdge = new List<Edge>();
            this.OutEdge = new List<Edge>();

            this.Attribute = new Dictionary<int, object>();
        }

        public void SetInvalid()
        {
            foreach (Edge e in this.InEdge)
            {
                e.SetInvalid();
            }
            foreach (Edge e in this.OutEdge)
            {
                e.SetInvalid();
            }

            if (this.Group != null)
            {
                this.Group.RemoveMember(this);
            }

            this.Graph = null;
        }
        public bool IsInvalid()
        {
            return this.Graph == null;
        }
    }
}
