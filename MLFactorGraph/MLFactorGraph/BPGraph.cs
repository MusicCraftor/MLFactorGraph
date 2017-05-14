using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLFactorGraph
{
    // should be internal!!!
    public class BPGraph
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
                    BPUnit<T> neigh = Base2BPMapping[neighBase] as BPUnit<T>;
                    unit.Neighbors.Add(neigh);
                    unit.Messages[neigh] = new Dictionary<short, double>();
                }
            }
        }

        public void BeliefPropagation(int maxIteration, MLFGraph.Layer layer)
        {
            switch (layer)
            {
                case MLFGraph.Layer.NodeLayer:
                    LayerBeliefPropagation(maxIteration, this.BPNodeLayer);
                    break;
                case MLFGraph.Layer.EdgeLayer:
                    LayerBeliefPropagation(maxIteration, this.BPEdgeLayer);
                    break;
                case MLFGraph.Layer.GroupLayer:
                    LayerBeliefPropagation(maxIteration, this.BPGroupLayer);
                    break;
                case MLFGraph.Layer.AllLayer:
                    LayerBeliefPropagation(maxIteration, this.BPNodeLayer);
                    LayerBeliefPropagation(maxIteration, this.BPEdgeLayer);
                    LayerBeliefPropagation(maxIteration, this.BPGroupLayer);
                    break;
                default:
                    break;
            }
        }
        public void LayerBeliefPropagation<T>(int maxIteration, List<BPUnit<T>> layer)
            where T : Factorable, ILayerNode
        {
            List<int> bfsOrder = BFSOrder(layer);
            for (int iter = 0; iter < maxIteration; iter++)
            {
                double maxDifference = 0;
                for (int i = layer.Count - 1; i >= 0; i--)
                {
                    double diff = layer[bfsOrder[i]].BeliefPropagation();
                    maxDifference = Math.Max(maxDifference, diff);
                }

                for (int i = 0; i <= layer.Count - 1; i++)
                {
                    double diff = layer[bfsOrder[i]].BeliefPropagation();
                    maxDifference = Math.Max(maxDifference, diff);
                }
                Console.WriteLine(maxDifference);

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

        public List<Dictionary<short, double>> GetGradient()
        {
            List<Dictionary<short, double>> gradientList = new List<Dictionary<short, double>>();
            foreach (BPUnit<Edge> eUnit in BPEdgeLayer)
            {
                gradientList.Add(eUnit.GetBelief());
            }
            return gradientList;
        }

        List<BPUnit<Node>> BPNodeLayer;
        List<BPUnit<Edge>> BPEdgeLayer;
        List<BPUnit<Group>> BPGroupLayer;
        Dictionary<object, object> Base2BPMapping;
        public MLFGraph BaseGraph { get; }
    }
}
