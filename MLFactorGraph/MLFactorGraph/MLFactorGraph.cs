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
        public MLFGraph(List<short> labels, bool bidirectionEdge)
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

            this.BidirectionEdge = bidirectionEdge;
            this.Labels = labels;

            // Lambda Initialization
            Lambda = new Dictionary<int, Dictionary<byte, double>>();
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
        public enum Layer
        {
            AllLayer = 0,
            NodeLayer,
            EdgeLayer,
            GroupLayer
        }
        public bool BidirectionEdge { get; private set; }

        public Node AddNode()
        {
            Node v = new Node(this);
            NodeLayer.Add(v);
            return v;
        }
        public Edge AddEdge(uint fromId, uint toId, short label)
        {
            if (HasEdge(fromId, toId))
            {
                return null;
            }

            Node from = FindNode(fromId);
            Node to = FindNode(toId);
            if ((from == null) || (to == null))
            {
                return null;
            }

            Edge e = new Edge(this, label, from, to);
            EdgeLayer.Add(e);
            if (this.BidirectionEdge)
            {
                Edge dualE = new Edge(this, label, to, from);
                EdgeLayer.Add(dualE);

                e.DualEdge = dualE;
                dualE.DualEdge = e;
            }
            return e;
        }
        public Group AddGroup(short label, List<uint> nodeIds)
        {
            if (HasGroup(nodeIds))
            {
                return null;
            }

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
        public bool HasNode(uint id)
        {
            return NodeLayer.Exists(x => x.Id == id);
        }
        public bool HasEdge(uint fromId, uint toId)
        {
            return EdgeLayer.Exists(x => x.From.Id == fromId && x.To.Id == toId);
        }
        public bool HasGroup(List<uint> idList)
        {
            return GroupLayer.Exists(delegate (Group g)
            {
                bool memberNotFound = false;
                foreach (uint id in idList)
                {
                    if (!g.Member.Exists(x => x.Id == id))
                    {
                        memberNotFound = true;
                    }
                }
                if (!memberNotFound)
                {
                    return true;
                }
                return false;
            });
        }

        public void AddMemberTo(List<Node> nodes, Group to)
        {
            to.AddMember(nodes);
        }
        public void AddMemberTo(List<uint> nodeIds, Group to)
        {
            List<Node> nodes = NodeLayer.FindAll(delegate (Node v)
            {
                return nodeIds.Exists(x => v.Id == x);
            });
            AddMemberTo(nodes, to);
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

        public List<short> Labels { get; protected set; }

        public void InitializeLambda(int factorLabel, byte value)
        {
            if (!Lambda.ContainsKey(factorLabel))
            {
                Lambda[factorLabel] = new Dictionary<byte, double>();
            }
            Lambda[factorLabel][value] = LAMBDA_INITIALIZATION;
        }
        public void SetLambda(int factorLabel, byte value, double lambda)
        {
            InitializeLambda(factorLabel, value);
            Lambda[factorLabel][value] = lambda;
        }
        public double GetLambda(int factorLabel, byte value)
        {
            if ((!Lambda.ContainsKey(factorLabel)) || (!Lambda[factorLabel].ContainsKey(value)))
            {
                InitializeLambda(factorLabel, value);
            }
            return Lambda[factorLabel][value];
        }
        public Dictionary<int, Dictionary<byte, double>> Lambda { get; protected set; }

        public void AddUnitaryFactor(int factorId, Factorable.UnitaryFactor factorFunction, Layer layer, bool dynamic = true, bool enabled = true)
        {
            switch (layer)
            {
                case Layer.NodeLayer:
                    AddUnitaryFactorToLayer(factorId, factorFunction, this.NodeLayer, dynamic, enabled);
                    break;
                case Layer.EdgeLayer:
                    AddUnitaryFactorToLayer(factorId, factorFunction, this.EdgeLayer, dynamic, enabled);
                    break;
                case Layer.GroupLayer:
                    AddUnitaryFactorToLayer(factorId, factorFunction, this.GroupLayer, dynamic, enabled);
                    break;
                case Layer.AllLayer:
                    AddUnitaryFactorToLayer(factorId, factorFunction, this.NodeLayer, dynamic, enabled);
                    AddUnitaryFactorToLayer(factorId, factorFunction, this.EdgeLayer, dynamic, enabled);
                    AddUnitaryFactorToLayer(factorId, factorFunction, this.GroupLayer, dynamic, enabled);
                    break;
                default:
                    break;
            }
            if (!Lambda.ContainsKey(factorId))
            {
                Lambda.Add(factorId, new Dictionary<byte, double>());
            }
        }
        void AddUnitaryFactorToLayer<T>(int factorId, Factorable.UnitaryFactor factorFunction, List<T> layer, bool dynamic = true, bool enabled = true)
            where T : Factorable, ILayerNode
        {
            layer.ForEach(delegate (T f)
            {
                f.AddUnitaryFactor(factorId, factorFunction, dynamic, enabled);
            });

        }
        public void AddBinaryFactor(int factorId, Factorable.BinaryFactor factorFunction, Layer layer, bool enabled = true)
        {
            switch (layer)
            {
                case Layer.NodeLayer:
                    AddBinaryFactorToLayer(factorId, factorFunction, this.NodeLayer, enabled);
                    break;
                case Layer.EdgeLayer:
                    AddBinaryFactorToLayer(factorId, factorFunction, this.EdgeLayer, enabled);
                    break;
                case Layer.GroupLayer:
                    AddBinaryFactorToLayer(factorId, factorFunction, this.GroupLayer, enabled);
                    break;
                case Layer.AllLayer:
                    AddBinaryFactorToLayer(factorId, factorFunction, this.NodeLayer, enabled);
                    AddBinaryFactorToLayer(factorId, factorFunction, this.EdgeLayer, enabled);
                    AddBinaryFactorToLayer(factorId, factorFunction, this.GroupLayer, enabled);
                    break;
                default:
                    break;
            }
            if (!Lambda.ContainsKey(factorId))
            {
                Lambda.Add(factorId, new Dictionary<byte, double>());
            }
        }
        void AddBinaryFactorToLayer<T>(int factorId, Factorable.BinaryFactor factorFunction, List<T> layer, bool enabled = true)
            where T : Factorable, ILayerNode
        {
            layer.ForEach(delegate (T f)
            {
                f.AddBinaryFactor(factorId, factorFunction, enabled);
            });

        }

        public void FactorInitialization(Layer layer)
        {
            switch (layer)
            {
                case Layer.NodeLayer:
                    FactorInitializationToLayer(this.NodeLayer);
                    break;
                case Layer.EdgeLayer:
                    FactorInitializationToLayer(this.EdgeLayer);
                    break;
                case Layer.GroupLayer:
                    FactorInitializationToLayer(this.GroupLayer);
                    break;
                case Layer.AllLayer:
                    FactorInitializationToLayer(this.NodeLayer);
                    FactorInitializationToLayer(this.EdgeLayer);
                    FactorInitializationToLayer(this.GroupLayer);
                    break;
                default:
                    break;
            }
        }
        void FactorInitializationToLayer<T>(List<T> layer)
            where T : Factorable, ILayerNode
        {
            foreach (T t1 in layer)
            {
                t1.UnitaryFactorFunction();
            }
        }

        public BPGraph BeliefPropagation(int maxIteration, Layer layer)
        {
            BPGraph bpGraph = new BPGraph(this);
            bpGraph.BeliefPropagation(maxIteration, layer);
            return bpGraph;
        }

        public void OutputLambda()
        {
            foreach (KeyValuePair<int, Dictionary<byte, double>> lambdaRecord in this.Lambda)
            {
                Console.Write(lambdaRecord.Key);
                foreach (KeyValuePair<byte, double> lambda in lambdaRecord.Value)
                {
                    Console.Write(" ({0}, {1})", lambda.Key, lambda.Value);
                }
                Console.WriteLine();
            }
        }
        public void OutputEdgeFactor()
        {
            foreach (Edge e in this.EdgeLayer)
            {
                Console.Write(e.Id);
                foreach (KeyValuePair<int, Dictionary<byte, double>> lambdaRecord in this.Lambda)
                {
                    foreach (KeyValuePair<byte, double> lambda in lambdaRecord.Value)
                    {
                        Console.Write(" {0}", e.CheckUnitaryFactor(lambdaRecord.Key, lambda.Key));
                    }
                }
                Console.Write(" {0}", e.Label);
                Console.WriteLine();
            }
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

        private const double LAMBDA_INITIALIZATION = 0.0;
    }
}
