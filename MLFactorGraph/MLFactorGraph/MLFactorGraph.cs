﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLFactorGraph
{
    public class MLFGraph
    {
        public MLFGraph(bool bidirectionEdge)
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

        public Dictionary<int, double> Lambda { get; protected set; }

        public void AddUnitaryFactor(int factorId, Factorable.UnitaryFactor factorFunction, Layer layer)
        {
            switch (layer)
            {
                case Layer.NodeLayer:
                    AddUnitaryFactorToLayer(factorId, factorFunction, this.NodeLayer);
                    break;
                case Layer.EdgeLayer:
                    AddUnitaryFactorToLayer(factorId, factorFunction, this.EdgeLayer);
                    break;
                case Layer.GroupLayer:
                    AddUnitaryFactorToLayer(factorId, factorFunction, this.GroupLayer);
                    break;
                case Layer.AllLayer:
                    AddUnitaryFactorToLayer(factorId, factorFunction, this.NodeLayer);
                    AddUnitaryFactorToLayer(factorId, factorFunction, this.EdgeLayer);
                    AddUnitaryFactorToLayer(factorId, factorFunction, this.GroupLayer);
                    break;
                default:
                    break;
            }
            if (!Lambda.ContainsKey(factorId))
            {
                Lambda.Add(factorId, 1.0);
            }
        }
        void AddUnitaryFactorToLayer<T>(int factorId, Factorable.UnitaryFactor factorFunction, List<T> layer)
            where T : Factorable
        {
            layer.ForEach(delegate (T f)
            {
                f.AddUnitaryFactor(factorId, factorFunction);
            });

        }
        public void AddBinaryFactor(int factorId, Factorable.BinaryFactor factorFunction, Layer layer)
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
        void AddFactorToLayer<T>(int factorId, Factorable.BinaryFactor factorFunction, List<T> layer)
            where T : Factorable
        {
            layer.ForEach(delegate (T f)
            {
                f.AddBinaryFactor(factorId, factorFunction);
            });

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
