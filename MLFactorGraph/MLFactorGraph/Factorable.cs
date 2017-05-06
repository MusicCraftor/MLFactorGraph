using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLFactorGraph
{
    public class Factorable
    {
        public Factorable(MLFGraph graph, Dataset dataSource)
        {
            this.Graph = graph;
            this.DataSource = dataSource;
            this.FactorDictionary = new Dictionary<int, Factor>();
            this.FactorEnabled = new Dictionary<int, bool>();
            this.FactorStorage = new List<double>();
        }

        public delegate double Factor(Factorable obj, Dataset data);

        public void AddStaticFactor(int factorLabel, double factorValue, bool enabled = false)
        {
            FactorStorage.Add(Double.NaN);
            int storageIndex = FactorStorage.FindIndex(x => x == Double.NaN);
            FactorStorage[storageIndex] = factorValue;

            FactorDictionary[factorLabel] = delegate (Factorable obj, Dataset data)
            {
                return this.FactorStorage[storageIndex];
            };
            FactorEnabled[factorLabel] = enabled;
        }
        public void AddDynamicFactor(int factorLabel, Factor factorDelegate, bool enabled = false)
        {
            FactorDictionary[factorLabel] = factorDelegate;
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

            return FactorDictionary[factorLabel](this, DataSource);
        }
        public double FactorFunction()
        {
            double result = 0.0;
            foreach (KeyValuePair<int, Factor> factor in FactorDictionary)
            {
                result += Graph.Lambda[factor.Key] * factor.Value(this, DataSource);
            }
            return result;
        }

        public Dataset DataSource { get; internal set; }

        Dictionary<int, Factor> FactorDictionary;
        Dictionary<int, bool> FactorEnabled;
        List<double> FactorStorage;
        MLFGraph Graph;
    }
}
