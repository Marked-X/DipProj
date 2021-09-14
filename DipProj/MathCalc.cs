using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DipProj
{
    static class MathCalc
    {
        static public Func<double, double> Triangle(double a, double b, double c)
        {
            return (x) => { if (x >= a && x <= b) { return 1 - ((b - x) / (b - a)); } else if (x > b && x <= c) { return 1 - ((x - b) / (c - b));  } else return 0; };
        }

        static public Func<double, double> Z_func(double a, double b)
        {
            return (x) => { if (x < a) return 1; else if (x >= a && x <= ((a + b) / 2)) return (1 - 2 * Math.Pow(((x - a) / (b - a)), 2)); 
                else if (x >= ((a + b) / 2) && x <= b) return (2 * Math.Pow(((x - b) / (b - a)), 2)); else return 0; };
        }

        static public Func<double, double> S_func(double a, double b)
        {
            return (x) => { if (x < a) return 0; else if (x >= a && x <= ((a + b) / 2)) return (2 * Math.Pow(((x - a) / (b - a)), 2)); 
                else if (x >= ((a + b) / 2) && x <= b) return (1 - 2 * Math.Pow(((x - b) / (b - a)), 2)); else return 1; };
        }

        static public double[] Fuzzification(List<Rule> rules, double[] input)
        {
            int i = 0;
            double[] fuzzy = new double[rules.Count * 2];

            foreach(Rule rule in rules)
            {
                foreach(FuzzySet condition in rule.Conditions)
                {
                    fuzzy[i] = condition.GetValue(input[condition.Variable.Id]);
                    i++;
                }
            }

            return fuzzy;
        }

        static public double[] Aggregation(double[] fuzzy, List<Rule> rules)
        {
            int i = 0;
            int j = 0;
            double[] aggregated = new double[rules.Count];

            foreach(Rule rule in rules)
            {
                double truthOfConditions = 1.0;
                foreach (FuzzySet condition in rule.Conditions)
                {
                    truthOfConditions = Math.Min(truthOfConditions, fuzzy[i]);
                    i++;
                }
                aggregated[j] = truthOfConditions;
                j++;
            }

            return aggregated;
        }

        static public List<ActivatedFuzzySet> Activation(double[] aggregated, List<Rule> rules) 
        {
            int i = 0;
            List<ActivatedFuzzySet> activatedFuzzySets = new List<ActivatedFuzzySet>();
            foreach (Rule rule in rules)
            {
                ActivatedFuzzySet activatedFuzzySet = new ActivatedFuzzySet(rule.Conclusion.Variable, rule.Conclusion.Name, rule.Conclusion.MembershipFunction);
                activatedFuzzySet.truthDegree = aggregated[i] * rule.Weight;
                activatedFuzzySets.Add(activatedFuzzySet);
                i++;
            }
            return activatedFuzzySets;
        }

        static public Union Accumulation(List<ActivatedFuzzySet> activatedFuzzySets, List<Rule> rules)
        {
            int i = 0;
            Union union = new Union();
            foreach (Rule rule in rules)
            {
                union.fuzzySets.Add(activatedFuzzySets[i]);
                i++;
            }
            return union;
        }

        static double Defuzzification(Union union, Chart chart)
        {
            double num = 0.0;
            double denum = 0.0;
            double result;
            chart.Series["Result"].Points.Clear();
            chart.Series["Centroid"].Points.Clear();
            for (double x = 0; x <= 100; x++)
            {
                num += x * union.GetMaxValue(x);
                denum += union.GetMaxValue(x);
            }
            for (int i = 0; i < 100; i++)
            {
                chart.Series["Result"].Points.AddXY(i, union.GetMaxValue(i));
            }
            result = num / denum;
            chart.Series["Centroid"].Points.AddXY(result, 0);
            chart.Series["Centroid"].Points.AddXY(result, 1);
            return result;
        }

        static public double Solve(List<Rule> rules, double[] input, Chart chart)
        {
            double[] fuzzy = Fuzzification(rules, input);
            double[] aggregated = Aggregation(fuzzy, rules);
            List<ActivatedFuzzySet> activated = Activation(aggregated, rules);
            Union accumulated = Accumulation(activated, rules);
            return Defuzzification(accumulated, chart);
        }
    }
}
