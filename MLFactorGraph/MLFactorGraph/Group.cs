using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLFactorGraph
{
    public class Group: Factorable
    {
        public uint Id { get; protected set; }
        public short Label { get; protected set; }

        public List<Node> Member { get; protected set; }
        public List<Edge> MemberEdge { get; protected set; }

        public Dictionary<int, object> Attribute { get; set; }

        public MLFGraph Graph { get; protected set; }

        public Group(MLFGraph graph, short label, List<Node> memberList = null)
            : base(graph, graph.DataSource, 
                  delegate (Factorable f)
                  {
                      Group g = f as Group;
                      List<Group> groups = new List<Group>();
                      foreach (Edge e in g.MemberEdge)
                      {
                          if (!g.InGroup(e.From))
                          {
                              groups.Add(e.From.Group);
                          }
                          if (!g.InGroup(e.To))
                          {
                              groups.Add(e.To.Group);
                          }
                      }
                      groups = groups.Distinct().ToList();
                      groups.RemoveAll(null);
                      return groups.Cast<Factorable>().ToList();
                  })
        {
            this.Id = graph.AllocateGroupId();
            this.Label = label;

            this.Member = (memberList == null) ? new List<Node>() : memberList;
            this.MemberEdge = new List<Edge>();
            BuildMemberEdge();

            this.Graph = graph;

            this.Attribute = new Dictionary<int, object>();
        }

        public bool InGroup(Node v)
        {
            return Member.Exists(delegate (Node u)
            {
                return u == v;
            });
        }

        internal void AddMember(Node v)
        {
            List<Node> l = new List<Node>();
            l.Add(v);
            AddMember(l);
        }
        internal void AddMember(List<Node> nodes)
        {
            foreach (Node v in nodes)
            {
                v.Group = this;
                foreach (Edge e in v.InEdge)
                {
                    if (this.InGroup(e.From))
                    {
                        e.Group = this;
                        e.Label = this.Label;
                        this.MemberEdge.Add(e);
                    }
                }
                foreach (Edge e in v.OutEdge)
                {
                    if (this.InGroup(e.To))
                    {
                        e.Group = this;
                        e.Label = this.Label;
                        this.MemberEdge.Add(e);
                    }
                }

                this.Member.Add(v);
            }
        }

        internal void RemoveMember(Node v)
        {
            List<Node> l = new List<Node>();
            l.Add(v);
            RemoveMember(l);
        }
        internal void RemoveMember(List<Node> nodes)
        {
            foreach (Node v in nodes)
            {
                if (this.InGroup(v))
                {
                    v.Group = null;
                    foreach (Edge e in v.InEdge)
                    {
                        if (this.InGroup(e.From))
                        {
                            e.Group = null;
                            this.MemberEdge.Remove(e);
                        }
                    }
                    foreach (Edge e in v.OutEdge)
                    {
                        if (this.InGroup(e.To))
                        {
                            e.Group = null;
                            this.MemberEdge.Remove(e);
                        }
                    }
                }

                this.Member.Remove(v);
            }
        }

        internal void BuildMemberEdge()
        {
            MemberEdge = new List<Edge>();

            foreach (Node v in this.Member)
            {
                foreach (Edge e in v.OutEdge)
                {
                    if (this.InGroup(e.To))
                    {
                        e.Group = this;
                        e.Label = this.Label;
                        this.MemberEdge.Add(e);
                    }
                }
                /* No need to add InEdge, for those valid has already been added in the OutEdge phase */
            }
        }

        public void SetInvalid()
        {
            this.RemoveMember(this.Member);
            this.Graph = null;
        }
        public bool IsInvalid()
        {
            return this.Graph == null;
        }
    }
}
