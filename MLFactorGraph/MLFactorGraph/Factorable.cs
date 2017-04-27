using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLFactorGraph
{
    public class Factorable
    {
        public Factorable(MLFGraph graph)
        {
            this.Graph = graph;
            this.FactorDictionary = new Dictionary<int, Factor>();
        }

        public delegate double Factor(Dataset data);

        public void AddFactor(int factorLabel, Factor factorDelegate)
        {
            FactorDictionary[factorLabel] = factorDelegate;
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

        public double FactorFunction()
        {
            double result = 0.0;
            foreach (KeyValuePair<int, Factor> factor in FactorDictionary)
            {
                result += Graph.Lambda[factor.Key] * factor.Value(DataSource);
            }
            return result;
        }

        public Dataset DataSource { get; set; }

        Dictionary<int, Factor> FactorDictionary;
        MLFGraph Graph;
    }
}
