using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLFactorGraph
{
    public class Factorable
    {
        public Factorable(MLFGraph graph, Dataset dataSource, AdjacentFactorable AdjacentMethod)
        {
            this.Graph = graph;
            this.DataSource = dataSource;
            this.AdjacentMethod = AdjacentMethod;
            this.FactorDictionary = new Dictionary<int, object>();
            this.FactorEnabled = new Dictionary<int, bool>();
            this.FactorTypes = new Dictionary<int, FactorType>();
            this.FactorStorage = new List<double>();
        }

        public delegate double UnitaryFactor(Factorable obj, Dataset data);
        public delegate double BinaryFactor(Factorable obj, Factorable objAdj, Dataset data);

        public delegate List<Factorable> AdjacentFactorable(Factorable obj);
        public List<Factorable> GetAdjacent()
        {
            return AdjacentMethod(this);
        }

        public void AddUnitaryFactor(int factorLabel, UnitaryFactor factorDelegate, bool enabled = true)
        {
            FactorDictionary[factorLabel] = factorDelegate;
            FactorTypes[factorLabel] = FactorType.Unitary;
            FactorEnabled[factorLabel] = enabled;
        }
        public void AddBinaryFactor(int factorLabel, BinaryFactor factorDelegate, bool enabled = true)
        {
            FactorDictionary[factorLabel] = factorDelegate;
            FactorTypes[factorLabel] = FactorType.Binary;
            FactorEnabled[factorLabel] = enabled;
        }

        public void EnableFactor(int factorLabel)
        {
            if (FactorDictionary.ContainsKey(factorLabel))
            {
                FactorEnabled[factorLabel] = true;
            }
        }
        public void DisableFactor(int factorLabel)
        {
            if (FactorDictionary.ContainsKey(factorLabel))
            {
                FactorEnabled[factorLabel] = false;
            }
        }
        public void RemoveFactor(int factorLabel)
        {
            if (FactorDictionary.ContainsKey(factorLabel))
            {
                FactorDictionary.Remove(factorLabel);
            }
        }
        public void ClearFactor()
        {
            FactorDictionary.Clear();
        }

        double GetUnitaryFactor(int factorLabel)
        {
            if (!FactorDictionary.ContainsKey(factorLabel))
            {
                return Double.NaN;
            }
            if (!FactorEnabled[factorLabel])
            {
                return Double.NaN;
            }

            switch (FactorTypes[factorLabel])
            {
                case FactorType.Unitary:
                    return ((UnitaryFactor)FactorDictionary[factorLabel])(this, DataSource);
                default:
                    return Double.NaN;
            }
        }
        double GetBinaryFactor(int factorLabel, Factorable adjFactorable)
        {
            if (!FactorDictionary.ContainsKey(factorLabel))
            {
                return Double.NaN;
            }
            if (!FactorEnabled[factorLabel])
            {
                return Double.NaN;
            }

            switch (FactorTypes[factorLabel])
            {
                case FactorType.Binary:
                    return ((BinaryFactor)FactorDictionary[factorLabel])(this, adjFactorable, DataSource);
                default:
                    return Double.NaN;
            }
        }
        public double UnitaryFactorFunction()
        {
            double sum = 0.0;
            foreach (KeyValuePair<int, object> pair in FactorDictionary)
            {
                double factorValue = GetUnitaryFactor(pair.Key);
                if (factorValue != double.NaN)
                {
                    sum += Graph.Lambda[pair.Key] * factorValue;
                }
            }
            return Math.Exp(sum);
        }
        public double BinaryFactorFunction(Factorable adjFactorable)
        {
            double sum = 0.0;
            foreach (KeyValuePair<int, object> pair in FactorDictionary)
            {
                double factorValue = GetBinaryFactor(pair.Key, adjFactorable);
                if (factorValue != double.NaN)
                {
                    sum += Graph.Lambda[pair.Key] * factorValue;
                }
            }
            return Math.Exp(sum);
        }

        public Dataset DataSource { get; internal set; }

        AdjacentFactorable AdjacentMethod;
        Dictionary<int, object> FactorDictionary;
        Dictionary<int, FactorType> FactorTypes;
        Dictionary<int, bool> FactorEnabled;
        List<double> FactorStorage;
        MLFGraph Graph;

        public enum FactorType
        {
            Value = 0, // Preserved
            Unitary,
            Binary
        }
    }

}
