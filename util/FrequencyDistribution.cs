using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace trabahuso_api.util
{
    public class Distribution
    {
        public required string Range { get; set; }
        public int Count { get; set; }
    }
    public class FrequencyDistribution
    {

        private static int SturgesFormula(int n)
        {
            return (int)Math.Ceiling(1 + (3.322 * Math.Log10(n)));
        }

        private static double GetRange(double min, double max)
        {
            return max - min;
        }

        private static double GetClassSize(double range, int classIntervals)
        {
            return Math.Ceiling(range / classIntervals);
        }

        public List<Distribution> GetFrequencyDistribution(List<double> data)
        {
            data.Sort();

            double min = data[0];
            int n = data.Count;
            double max = data[n - 1];

            // Calculate intervals and class size
            int classIntervals = SturgesFormula(n);
            double range = GetRange(min, max);
            double classSize = GetClassSize(range, classIntervals);

            List<Distribution> frequencyDistributionList = [];


            for (int index = 0; index < classIntervals; index++)
            {
                double intervalUpperBoundary = min + (classSize - 1);
                string rangeStr = $"{min}-{intervalUpperBoundary}";

                frequencyDistributionList.Add(new Distribution
                {
                    Range = rangeStr,
                    Count = 0
                });

                min += classSize;

                if (index == classIntervals - 1 && intervalUpperBoundary < max)
                {
                    string lastRange = $"{min}-{min + (classSize - 1)}";
                    frequencyDistributionList.Add(new Distribution
                    {
                        Range = lastRange,
                        Count = 0
                    });
                }
            }

            return frequencyDistributionList;
        }
    }
}