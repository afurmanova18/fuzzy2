using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fuzzy
{

    public class Test
    {

        public static List<int> Cold = new List<int> { 0, 0, 10, 15 };
        public static List<int> Cool = new List<int> { 10, 15, 20, 25 };
        public static List<int> Normal = new List<int> { 20, 25, 27, 30 };
        public static List<int> Warm = new List<int> { 27, 30, 35, 40 };
        public static List<int> Hot = new List<int> { 35, 40, 50, 50 };

      
        public static List<int> High = new List<int> {0, 0, 20, 30 };// кондиционер выпускает горячий воздух
        public static List<int> Medium = new List<int> { 20, 30, 60, 70 };// кондиционер выпускает ?теплый?(средний) воздух
        public static List<int> Low = new List<int> { 60, 70, 100, 100 };// кондиционер выпускает холодный воздух
        

    }
    public class FuzzySetTrapezoid : FuzzySetInterface
    {
        private List<double> trapezoidIntervals;

        public FuzzySetTrapezoid(List<int> intervals)
        {
            trapezoidIntervals = intervals.Select(i => (double)i).ToList();
        }

        public double GetValue(double value)
        {
            if (value < trapezoidIntervals[0])
            {
                return 0;
            }
            else if (trapezoidIntervals[0] < value && trapezoidIntervals[1] > value)
            {
                return (value - trapezoidIntervals[0]) / (trapezoidIntervals[1] - trapezoidIntervals[0]);
            }
            else if (trapezoidIntervals[1] <= value && trapezoidIntervals[2] >= value)
            {
                return 1;
            }
            else if (trapezoidIntervals[2] < value && trapezoidIntervals[3] > value)
            {
                return (trapezoidIntervals[3] - value) / (trapezoidIntervals[3] - trapezoidIntervals[2]);
            }
            else
            {
                return 0;
            }
        }
    }


    public class Program
    {
       

        static void Main(string[] args)
        {
            List<Rule> rules = new List<Rule>();

            rules.Add(new Rule(new List<Condition>() { new Condition(new Variable(0),
                new FuzzySetTrapezoid(Test.Cold))}, 
                new List<Conclusion>() { new Conclusion(new Variable(0),
                new FuzzySetTrapezoid(Test.High), 1.0)}));



            //FuzzySetTrapezoid b = new FuzzySetTrapezoid(Test.Cool);

            //Console.WriteLine(b.GetValue(16));

            rules.Add(new Rule(new List<Condition>() { new Condition(new Variable(0),
                new FuzzySetTrapezoid(Test.Cool))},
                new List<Conclusion>() { new Conclusion(new Variable(0),
                new FuzzySetTrapezoid(Test.Medium), 1.0)}));

            rules.Add(new Rule(new List<Condition>() { new Condition(new Variable(0),
                new FuzzySetTrapezoid(Test.Normal))},
                new List<Conclusion>() { new Conclusion(new Variable(0),
                new FuzzySetTrapezoid(Test.Low),1.0)}));
             
            rules.Add(new Rule(new List<Condition>() { new Condition(new Variable(0),
                new FuzzySetTrapezoid(Test.Warm))},
                new List<Conclusion>() { new Conclusion(new Variable(0),
                new FuzzySetTrapezoid(Test.Medium), 1.0)}));

            rules.Add(new Rule(new List<Condition>() { new Condition(new Variable(0),
                new FuzzySetTrapezoid(Test.Hot))},
                new List<Conclusion>() { new Conclusion(new Variable(0),
                new FuzzySetTrapezoid(Test.Low), 1.0)}));


            double[] inputData = new double[] { 36 };
            Mamdani mamdani = new Mamdani(rules, inputData);
            List<double> result = mamdani.execute();
           
     
            if (result[0] >= 70)
            {
                Console.WriteLine( "Температура: " + inputData[0] + "\n" + result[0]+ "\n=>Система кондиционирования работает на режиме холодный воздух");
            }
            else if (result[0] >= 30)
            {
                Console.WriteLine("Температура: " + inputData[0] + "\n" + result[0] + "\n=>Система кондиционирования работает на среднем режиме.");
            }
            else
            {
                Console.WriteLine("Температура: " + inputData[0] + "\n" + +result[0] + "\n=>Система кондиционирования работает на режиме горячий воздух.");
            }

            inputData = new double[] { 41 };
            mamdani = new Mamdani(rules, inputData);
            result = mamdani.execute();

            if (result[0] >= 70)
            {
                Console.WriteLine("Температура: " + inputData[0] + "\n" + result[0] + "\n=>Система кондиционирования работает на режиме холодный воздух");
            }
            else if (result[0] >= 30)
            {
                Console.WriteLine("Температура: " + inputData[0] + "\n" + result[0] + "\n=>Система кондиционирования работает на среднем режиме.");
            }
            else
            {
                Console.WriteLine("Температура: " + inputData[0] + "\n" + +result[0] + "\n=>Система кондиционирования работает на режиме горячий воздух.");
            }

            Console.ReadLine();

        }
    }
}
