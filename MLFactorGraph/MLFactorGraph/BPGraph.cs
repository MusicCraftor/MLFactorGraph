using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLFactorGraph
{
    internal class BPGraph
    {
        public BPGraph(MLFGraph baseGraph)
        {
            this.BPNodeLayer = new List<BPUnit<Node>>();
            this.BPEdgeLayer = new List<BPUnit<Edge>>();
            this.BPGroupLayer = new List<BPUnit<Group>>();
            this.Base2BPMapping = new Dictionary<object, object>();
            this.BaseGraph = baseGraph;

            BuildBPGraph();
        }

        void BuildBPGraph()
        {
            BuildBPLayerNode(BaseGraph.NodeLayer, this.BPNodeLayer);
            BuildBPLayerNode(BaseGraph.EdgeLayer, this.BPEdgeLayer);
            BuildBPLayerNode(BaseGraph.GroupLayer, this.BPGroupLayer);

            BuildBPLayerNeighbor(this.BPNodeLayer);
            BuildBPLayerNeighbor(this.BPEdgeLayer);
            BuildBPLayerNeighbor(this.BPGroupLayer);
        }
        void BuildBPLayerNode<T>(List<T> sourceLayer, List<BPUnit<T>> targetLayer)
            where T : Factorable, ILayerNode
        {
            foreach (T baseUnit in sourceLayer)
            {
                BPUnit<T> unit = new BPUnit<T>(this, baseUnit);
                targetLayer.Add(unit);
                Base2BPMapping.Add(baseUnit, unit);
            }
        }
        void BuildBPLayerNeighbor<T>(List<BPUnit<T>> layer)
            where T : Factorable, ILayerNode
        {
            foreach (BPUnit<T> unit in layer)
            {
                foreach (T neighBase in unit.BaseUnit.GetAdjacent())
                {
                    unit.Neighbors.Add(Base2BPMapping[neighBase] as BPUnit<T>);
                }
            }
        }

        public void LayerBeliefPropagation<T>(int maxIteration, List<BPUnit<T>> layer)
            where T : Factorable, ILayerNode
        {
            List<int> bfsOrder = BFSOrder(layer);
            for (int iter = 0; iter < maxIteration; iter++)
            {
                int start, end, interval;
                if (iter % 2 == 0)
                {
                    start = layer.Count - 1; end = 0; interval = -1;
                }
                else
                {
                    start = 0; end = layer.Count - 1; interval = +1;
                }

                double maxDifference = 0;
                for (int i = start; i <= end; i += interval)
                {
                    double diff = layer[bfsOrder[i]].BeliefPropagation();
                    maxDifference = Math.Max(maxDifference, diff);
                }

                if (maxDifference < 1e-6)
                {
                    break;
                }
            }
        }
        List<int> BFSOrder<T>(List<BPUnit<T>> layer)
            where T : Factorable, ILayerNode
        {
            return Enumerable.Range(0, layer.Count).ToList();
        }

        List<BPUnit<Node>> BPNodeLayer;
        List<BPUnit<Edge>> BPEdgeLayer;
        List<BPUnit<Group>> BPGroupLayer;
        Dictionary<object, object> Base2BPMapping;
        public MLFGraph BaseGraph { get; }
    }
}
