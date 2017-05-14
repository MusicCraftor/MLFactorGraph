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
        }

        public delegate byte UnitaryFactor(Factorable obj, Dataset data);
        public delegate byte BinaryFactor(Factorable obj, Factorable objAdj, Dataset data);

        public delegate List<Factorable> AdjacentFactorable(Factorable obj);
        public List<Factorable> GetAdjacent()
        {
            return AdjacentMethod(this);
        }

        public void AddUnitaryFactor(int factorLabel, UnitaryFactor factorDelegate, bool dynamic = true, bool enabled = true)
        {
            if (dynamic)
            {
                FactorDictionary[factorLabel] = factorDelegate;
            }
            else
            {
                byte result = factorDelegate(this, this.DataSource);
                UnitaryFactor factor = delegate (Factorable obj, Dataset data)
                {
                    return result;
                };
                FactorDictionary[factorLabel] = factor;
            }
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

        public byte GetUnitaryFactor(int factorLabel)
        {
            if (!FactorDictionary.ContainsKey(factorLabel))
            {
                return byte.MaxValue;
            }
            if (!FactorEnabled[factorLabel])
            {
                return byte.MaxValue;
            }

            switch (FactorTypes[factorLabel])
            {
                case FactorType.Unitary:
                    return ((UnitaryFactor)FactorDictionary[factorLabel])(this, DataSource);
                default:
                    return byte.MaxValue;
            }
        }
        public byte GetBinaryFactor(int factorLabel, Factorable adjFactorable)
        {
            if (!FactorDictionary.ContainsKey(factorLabel))
            {
                return byte.MaxValue;
            }
            if (!FactorEnabled[factorLabel])
            {
                return byte.MaxValue;
            }

            switch (FactorTypes[factorLabel])
            {
                case FactorType.Binary:
                    return ((BinaryFactor)FactorDictionary[factorLabel])(this, adjFactorable, DataSource);
                default:
                    return byte.MaxValue;
            }
        }
        public int CheckUnitaryFactor(int factorLabel, byte value)
        {
            return (value == GetUnitaryFactor(factorLabel)) ?
                1 :
                0;
        }
        public int CheckBinaryFactor(int factorLabel, Factorable adjFactorable, byte value)
        {
            return (value == GetBinaryFactor(factorLabel, adjFactorable)) ?
                1 :
                0;
        }
        public double UnitaryFactorFunction()
        {
            double sum = 0.0;
            foreach (KeyValuePair<int, object> pair in FactorDictionary)
            {
                byte factorValue = GetUnitaryFactor(pair.Key);
                if (factorValue != double.NaN)
                {
                    sum += Graph.GetLambda(pair.Key, factorValue) * CheckUnitaryFactor(pair.Key, factorValue);
                }
            }
            return Math.Exp(sum);
        }
        public double BinaryFactorFunction(Factorable adjFactorable)
        {
            double sum = 0.0;
            foreach (KeyValuePair<int, object> pair in FactorDictionary)
            {
                byte factorValue = GetBinaryFactor(pair.Key, adjFactorable);
                if (factorValue != double.NaN)
                {
                    sum += Graph.GetLambda(pair.Key, factorValue) * factorValue;
                }
            }
            return Math.Exp(sum);
        }

        public Dataset DataSource { get; internal set; }

        AdjacentFactorable AdjacentMethod;
        Dictionary<int, object> FactorDictionary;
        Dictionary<int, FactorType> FactorTypes;
        Dictionary<int, bool> FactorEnabled;
        MLFGraph Graph;

        public enum FactorType
        {
            Value = 0, // Preserved
            Unitary,
            Binary
        }
    }

}
