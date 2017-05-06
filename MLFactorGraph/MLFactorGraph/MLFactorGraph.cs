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

            // Data Source Initialization
            this.DataSource = null;

            // Lambda Initialization
            Lambda = new Dictionary<int, double>();
        }

        public Dataset DataSource { get; private set; }

        public void SetDataSource(Dataset dataset)
        {
            this.DataSource = dataset;

            foreach (Factorable f in NodeLayer)
            {
                f.DataSource = this.DataSource;
            }
            foreach (Factorable f in EdgeLayer)
            {
                f.DataSource = this.DataSource;
            }
            foreach (Factorable f in GroupLayer)
            {
                f.DataSource = this.DataSource;
            }
        }

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
                if (v.Group != null)
                    v.Group.RemoveMember(v);
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
        public void RemoveNode(Node v)
        {
            if (v.Graph == this)
            {
                NodeLayer[(int)v.Id] = null;
                v.SetInvalid();
            }
        }
        public void RemoveEdge(Edge e)
        {
            if (e.Graph == this)
            {
                EdgeLayer[(int)e.Id] = null;
                e.SetInvalid();
            }
        }
        public void RemoveGroup(Group g)
        {
            if (g.Graph == this)
            {
                GroupLayer[(int)g.Id] = null;
                g.SetInvalid();
            }
        }
        public Node FindNode(uint id)
        {
            try
            {
                return NodeLayer[(int)id];
            }
            catch (ArgumentOutOfRangeException)
            {
            }
            return null;
        }
        public Edge FindEdge(uint id)
        {
            try
            {
                return EdgeLayer[(int)id];
            }
            catch (ArgumentOutOfRangeException)
            {
            }
            return null;
        }
        public Group FindGroup(uint id)
        {
            try
            {
                return GroupLayer[(int)id];
            }
            catch (ArgumentOutOfRangeException)
            {
            }
            return null;
        }

        public void MoveMember(List<Node> nodes, Group from, Group to)
        {
            from.RemoveMember(nodes);
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

        public Dictionary<int, double> Lambda { get; protected set; }

        public enum Layer
        {
            AllLayer = 0,
            NodeLayer,
            EdgeLayer,
            GroupLayer
        }

        public void AddFactor(int factorId, Factorable.Factor factorFunction, Layer layer)
        {
            switch (layer)
            {
                case Layer.NodeLayer:
                    AddFactorToLayer(factorId, factorFunction, this.NodeLayer);
                    break;
                case Layer.EdgeLayer:
                    AddFactorToLayer(factorId, factorFunction, this.EdgeLayer);
                    break;
                case Layer.GroupLayer:
                    AddFactorToLayer(factorId, factorFunction, this.GroupLayer);
                    break;
                case Layer.AllLayer:
                    AddFactorToLayer(factorId, factorFunction, this.NodeLayer);
                    AddFactorToLayer(factorId, factorFunction, this.EdgeLayer);
                    AddFactorToLayer(factorId, factorFunction, this.GroupLayer);
                    break;
                default:
                    break;
            }
            if (!Lambda.ContainsKey(factorId))
            {
                Lambda.Add(factorId, 1.0);
            }
        }
        void AddFactorToLayer<T>(int factorId, Factorable.Factor factorFunction, List<T> layer)
            where T : Factorable
        {
            layer.ForEach(delegate (T f)
            {
                f.AddDynamicFactor(factorId, factorFunction);
            });

        }

        public double FactorFunction()
        {
            if (!DataSource.Available)
            {
                return Double.NaN;
            }

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
