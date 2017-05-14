using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLFactorGraph
{
    // should be internal!!!
    public class BPUnit<T>
        where T : Factorable, ILayerNode
    {
        public BPUnit(BPGraph bpGraph, T baseUnit)
        {
            this.BaseUnit = baseUnit;
            this.BpGraph = bpGraph;
            this.Neighbors = new List<BPUnit<T>>();

            this.Messages = new Dictionary<BPUnit<T>, Dictionary<short, double>>();
            this.Belief = null;
        }

        public double BeliefPropagation()
        {
            Belief = null;
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
                            try
                            {
                                product *= Messages[otherNgh][nghLabel];
                            }
                            catch (KeyNotFoundException)
                            {
                                // Ignore this message
                            }
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
            if (lastMsg.Count != message.Count)
            {
                return 1.0;
            }
            return Enumerable.Zip(lastMsg, message, (first, second) => Math.Abs(first.Value - second.Value))
                .Max();
        }

        public Dictionary<short, double> GetBelief()
        {
            if (Belief == null)
            {
                Belief = new Dictionary<short, double>();
                foreach (short label in BaseUnit.Graph.Labels)
                {
                    this.BaseUnit.Label = label;
                    double product = this.BaseUnit.UnitaryFactorFunction();
                    foreach (BPUnit<T> neighbor in Neighbors)
                    {
                        foreach (short nghLabel in BaseUnit.Graph.Labels)
                        {
                            try
                            {
                                product *= Messages[neighbor][nghLabel];
                            }
                            catch (KeyNotFoundException)
                            {
                                // Ignore this message
                            }
                        }
                    }
                    Belief[label] = product;
                }
                NormalizeBelief();
            }
            return Belief;
        }
        void NormalizeBelief()
        {
            double beliefSum = Belief.Sum(x => x.Value);
            Belief = Belief.ToDictionary(x => x.Key, x => x.Value / beliefSum);
        }

        public T BaseUnit { get; protected set; }
        public BPGraph BpGraph { get; protected set; }
        public List<BPUnit<T>> Neighbors { get; internal set; }
        
        internal Dictionary<BPUnit<T>, Dictionary<short, double>> Messages { get; set; }
        Dictionary<short, double> Belief;
    }
}
