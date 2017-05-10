using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLFactorGraph
{
    class BPUnit<T>
        where T : Factorable, ILayerNode
    {
        public BPUnit(BPGraph bpGraph, T baseUnit)
        {
            this.BaseUnit = baseUnit;
            this.BpGraph = bpGraph;
            this.Neighbors = new List<BPUnit<T>>();
        }

        public double BeliefPropagation()
        {
            double maxDiff = 0.0;

            foreach (BPUnit<T> neighbor in Neighbors)
            {
                Dictionary<short, double> msg = new Dictionary<short, double>();
                foreach (short nghLabel in BpGraph.BaseGraph.Labels)
                {
                    neighbor.BaseUnit.Label = nghLabel;

                    double product = 0.0;
                    foreach (short label in BpGraph.BaseGraph.Labels)
                    {
                        this.BaseUnit.Label = label;
                        product += this.BaseUnit.UnitaryFactorFunction()
                            + this.BaseUnit.BinaryFactorFunction(neighbor.BaseUnit);
                    }

                    foreach (BPUnit<T> otherNgh in Neighbors)
                    {
                        if (otherNgh != neighbor)
                        {
                            product *= Messages[otherNgh][nghLabel];
                        }
                    }

                    msg[nghLabel] = product;
                }
                maxDiff = Math.Max(maxDiff, 
                    SendMessageTo(neighbor, NormalizeMessage(msg)));
            }

            return maxDiff;
        }

        Dictionary<short, double> NormalizeMessage(Dictionary<short, double> rawMessage)
        {
            double msgSum = rawMessage.Sum(x => x.Value);
            return rawMessage.ToDictionary(x => x.Key, x => x.Value / msgSum);
        }
        double SendMessageTo(BPUnit<T> unit, Dictionary<short, double> message)
        {
            Dictionary<short, double> lastMsg;
            if (unit.Messages.ContainsKey(this))
            {
                lastMsg = unit.Messages[this];
            }
            else
            {
                lastMsg = new Dictionary<short, double>();
            }

            unit.Messages[this] = message;
            return Enumerable.Zip(lastMsg, message, (first, second) => Math.Abs(first.Value - second.Value))
                .Max();
        }

        public T BaseUnit { get; protected set; }
        public BPGraph BpGraph { get; protected set; }
        public List<BPUnit<T>> Neighbors { get; internal set; }

        public Dictionary<int, double> Belief { get; protected set; }
        internal Dictionary<BPUnit<T>, Dictionary<short, double>> Messages { get; set; }
    }
}
