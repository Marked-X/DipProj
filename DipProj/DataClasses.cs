using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DipProj
{
    public class Variable
    {
        public int Id;
        public string Name;
        public Dictionary<string, FuzzySet> FuzzySets { get; } = new Dictionary<string, FuzzySet>();
    }
    public class FuzzySet
    {
        public Variable Variable;
        public string Name;
        public Func<double, double> MembershipFunction;
        public double GetValue(double x)
        {
            return MembershipFunction(x);
        }
    }
    public class ActivatedFuzzySet : FuzzySet
    {
        public double truthDegree;
        public ActivatedFuzzySet(Variable var, String name, Func<double, double> MembFunc)
        {
            this.Variable = var;
            this.Name = name;
            this.MembershipFunction = MembFunc;
        }      
        public double GetActivatedValue(double x)
        {
            return Math.Min(GetValue(x), truthDegree);
        }
    }
    public class Rule
    {
        public List<FuzzySet> Conditions = new List<FuzzySet>();
        public FuzzySet Conclusion;
        public double Weight = 1;
    }
    public class Union : List<FuzzySet>
    {
        public List<FuzzySet> fuzzySets = new List<FuzzySet>();
        public double GetMaxValue(double x)
        {
            double result = 0.0; 
            foreach(ActivatedFuzzySet fuzzySet in fuzzySets)
            {
                result = Math.Max(result, fuzzySet.GetActivatedValue(x));
            }
            return result;
        }
    }
}