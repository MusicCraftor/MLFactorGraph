using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLFactorGraph
{
    public class MLFGraph
    {
        public MLFGraph()
        {
            // Layer initialization
            NodeLayer = new List<Node>();
            EdgeLayer = new List<Edge>();
            GroupLayer = new List<Group>();

            // Id Allocator Initialization
            nodeIdAllocator = 0;
            edgeIdAllocator = 0;
            groupIdAllocator = 0;
        }

        // Only for test!!!!
        public int test;

        public List<Node> NodeLayer { get; set; }
        public List<Edge> EdgeLayer { get; set; }
        public List<Group> GroupLayer { get; set; }

        public Node AddNode()
        {
            Node v = new Node(this);
            NodeLayer.Add(v);
            return v;
        }

        public Edge AddEdge(uint fromId, uint toId, short label)
        {
            Node from = FindNode(fromId);
            Node to = FindNode(toId);
            if ((from == null) || (to == null))
            {
                return null;
            }

            Edge e = new Edge(this, label, from, to);
            EdgeLayer.Add(e);
            return e;
        }

        public Group AddGroup(short label, List<uint> nodeIds)
        {
            List<Node> nodeList = nodeIds.Select<uint, Node>(delegate (uint id)
            {
                Node v = FindNode(id);
                if (v == null)
                    return null;
                v.Group.DeleteMember(v);
                return v;
            }).ToList();
            if (nodeList.Exists(v => v == null))
            {
                return null;
            }

            Group g = new Group(this, label, nodeList);
            GroupLayer.Add(g);
            return g;
        }

        public Node FindNode(uint id)
        {
            return NodeLayer.Find(delegate (Node v)
            {
                return v.Id == id;
            });
        }

        public Edge FindEdge(uint id)
        {
            return EdgeLayer.Find(delegate (Edge e)
            {
                return e.Id == id;
            });
        }

        public Group FindGroup(uint id)
        {
            return GroupLayer.Find(delegate (Group g)
            {
                return g.Id == id;
            });
        }

        public void MoveMember(List<Node> nodes, Group from, Group to)
        {
            from.DeleteMember(nodes);
            to.AddMember(nodes);
        }

        public void MoveMember(List<uint> nodeIds, Group from, Group to)
        {
            List<Node> nodes = NodeLayer.FindAll(delegate (Node v)
            {
                return nodeIds.Exists(x => v.Id == x);
            });
            MoveMember(nodes, from, to);
        }

        public Dictionary<int, double> Lambda { get; set; }

        public double FactorFunction()
        {
            double result = 0.0;
            foreach (Node v in NodeLayer)
            {
                result += v.FactorFunction();
            }
            foreach (Edge e in EdgeLayer)
            {
                result += e.FactorFunction();
            }
            foreach (Group g in GroupLayer)
            {
                result += g.FactorFunction();
            }

            return result;
        }

        uint nodeIdAllocator;
        uint edgeIdAllocator;
        uint groupIdAllocator;

        internal uint AllocateNodeId()
        {
            return nodeIdAllocator++;
        }

        internal uint AllocateEdgeId()
        {
            return edgeIdAllocator++;
        }

        internal uint AllocateGroupId()
        {
            return groupIdAllocator++;
        }
    }
}
