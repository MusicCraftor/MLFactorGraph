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
            this.GetAdjacent = AdjacentMethod;
            this.FactorDictionary = new Dictionary<int, object>();
            this.FactorEnabled = new Dictionary<int, bool>();
            this.FactorStorage = new List<double>();
        }

        public delegate double UnitaryFactor(Factorable obj, Dataset data);
        public delegate double BinaryFactor(Factorable obj, Factorable objAdj, Dataset data);

        public delegate List<Factorable> AdjacentFactorable(Factorable obj);

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

        public double GetFactor(int factorLabel)
        {
            if (!FactorDictionary.ContainsKey(factorLabel))
            {
                return Double.NaN;
            }

            switch(FactorTypes[factorLabel])
            {
                case FactorType.Value:
                    return Double.NaN;
                case FactorType.Unitary:
                    return ((UnitaryFactor)FactorDictionary[factorLabel])(this, DataSource);
                case FactorType.Binary:
                    return Enumerable.Sum(GetAdjacent(this).Select(delegate (Factorable f)
                    {
                        return ((BinaryFactor)FactorDictionary[factorLabel])(this, f, DataSource);
                    }));
                default:
                    return Double.NaN;
            }
        }

        public Dataset DataSource { get; internal set; }

        AdjacentFactorable GetAdjacent;
        Dictionary<int, object> FactorDictionary;
        Dictionary<int, FactorType> FactorTypes;
        Dictionary<int, bool> FactorEnabled;
        List<double> FactorStorage;
        MLFGraph Graph;
    }

    public enum FactorType
    {
        Value = 0,
        Unitary,
        Binary
    }
}
