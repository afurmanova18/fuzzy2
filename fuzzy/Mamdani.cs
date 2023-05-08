﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fuzzy
{
    public class Mamdani { 
     private List<Rule> rules;
    private double[] inputData;

    public Mamdani(List<Rule> rules, double[] inputData)
    {
        this.rules = rules;
        this.inputData = inputData;
    }

        public List<double> execute()
        {
            List<double> fuzzificated = Fuzzification(inputData);
            List<double> aggregated = Aggregation(fuzzificated);
            List<ActivatedFuzzySet> activated = Activation(aggregated);
            List<UnionOfFuzzySets> accumulated = Accumulation(activated);
            return Defuzzification(accumulated);
        }


        private List<double> Fuzzification(double[] inputData)
        {
            List<double> b = new List<double>();
            foreach (Rule rule in rules)
            {
                foreach (Condition condition in rule.GetConditions())
                {
                    int j = condition.GetVariable().GetId();
                    FuzzySetInterface term = condition.GetTerm();
                    b.Add(term.GetValue(inputData[j]));
                }
            }
            return b;
        }

        private List<double> Aggregation(List<double> b)
        {
            int i = 0;
            int j = 0;
            List<double> c = new List<double>();
            foreach (Rule rule in rules)
            {
                double truthOfConditions = 1.0;
                foreach (Condition condition in rule.GetConditions())
                {
                    truthOfConditions = Math.Min(truthOfConditions, b[i]);
                    i++;
                }
                c.Add(truthOfConditions);
            }
            return c;
        }

        private List<ActivatedFuzzySet> Activation(List<double> c)
        {
            int i = 0;
            List<ActivatedFuzzySet> activatedFuzzySets = new List<ActivatedFuzzySet>();
            foreach (Rule rule in rules)
            {
                foreach (Conclusion conclusion in rule.GetConclusions())
                {
                    activatedFuzzySets.Add(new ActivatedFuzzySet(
                            conclusion.GetTerm(), c[i] * conclusion.GetWeight()
                    ));
                    i++;
                }
            }
            return activatedFuzzySets;
        }


        private List<UnionOfFuzzySets> Accumulation(List<ActivatedFuzzySet> activatedFuzzySets)
        {
            Dictionary<int, UnionOfFuzzySets> unionsOfFuzzySets = new Dictionary<int, UnionOfFuzzySets>();
            int i = 0;
            foreach (Rule rule in rules) { 
                foreach (Conclusion conclusion in rule.GetConclusions()) { 
                    int index = conclusion.GetVariable().GetId(); 
                    if (!unionsOfFuzzySets.ContainsKey(index)) { 
                        unionsOfFuzzySets.Add(index, new UnionOfFuzzySets()); 
                    } 
                    unionsOfFuzzySets[index].AddFuzzySet(activatedFuzzySets[i]); 
                    i++; }
            }
            return new List<UnionOfFuzzySets>(unionsOfFuzzySets.Values);
        }

        private List<double> Defuzzification(List<UnionOfFuzzySets> unionsOfFuzzySets)
        {
            List<double> y = new List<double>();
            foreach (UnionOfFuzzySets unionOfFuzzySets in unionsOfFuzzySets)
            {
                double i1 = Integral(unionOfFuzzySets, true);
                double i2 = Integral(unionOfFuzzySets, false);
                y.Add(i1 / i2);
            }
            return y;
        }

        private double Integral(FuzzySetInterface fuzzySet, bool b)
        {
            //Func<double, double> function = b ? (x => x * fuzzySet.GetValue(x)) : fuzzySet.GetValue;

            Func<double, double> function;
            if (b)
            {
                function = delegate (double x) { return x * fuzzySet.GetValue(x); };
            }
            else
            {
                function = delegate (double x) { return fuzzySet.GetValue(x); };
            }
            return Integrate(0, 100, function);
        }


        public static double Integrate(double a, double b, Func<double, double> function)
        {
            int N = 10000;
            double h = (b - a) / (N - 1);

            double sum = (1.0 / 3.0) * (function(a) + function(b));

            for (int i = 1; i < N - 1; i += 2)
            {
                double x = a + h * i;
                sum += (4.0 / 3.0) * function(x);
            }

            for (int i = 2; i < N - 1; i += 2)
            {
                double x = a + h * i;
                sum += (2.0 / 3.0) * function(x);
            }

            return sum * h;
        }

    }
}