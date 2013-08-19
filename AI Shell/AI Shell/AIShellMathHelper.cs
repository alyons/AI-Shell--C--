/// Copyright Alexander Lyons 2013
///
/// This file is part of AI Shell.
///
/// AI Shell is free software: you can redistribute it and/or modify
/// it under the terms of the GNU General Public License as published by
/// the Free Software Foundation, either version 3 of the License, or
/// (at your option) any later version.
/// 
/// AI Shell is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License
/// along with AI Shell.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AI_Shell
{
    public static class MathHelper
    {
        public static double Mean<T>(List<T> data)
        {
            double mean = 0;
            double count = 0;

            count = data.Count;

            if (count == 0) throw new ArgumentNullException("There must be at least one value in the data set to calculate an average.");

            if (typeof(T) == typeof(int))
            {
                foreach (T n in data) mean += Convert.ToInt32(n);
            }
            else if (typeof(T) == typeof(short))
            {
                foreach (T n in data) mean += Convert.ToInt32(n);
            }
            else if (typeof(T) == typeof(long))
            {
                foreach (T n in data) mean += Convert.ToInt32(n);
            }
            else if (typeof(T) == typeof(double))
            {
                foreach (T n in data) mean += Convert.ToDouble(n);
            }
            else if (typeof(T) == typeof(float))
            {
                foreach (T n in data)
                {
                    float f = 0.0f;
                    float.TryParse(Convert.ToString(n), out f);
                    mean += f;
                }
            }
            else
            {
                throw new ArgumentException("Input classes must be of a numeric type (short, int, long, float, double).");
            }

            return mean / count;
        }
        public static double Mean<T>(List<T> data, int index, int count)
        {
            List<T> subData = data.GetRange(index, count);
            return Mean<T>(subData);
        }
        public static double Median<T>(List<T> data)
        {
            List<T> sortedList = new List<T>();
            sortedList.AddRange(data);
            sortedList.Sort();

            int count = sortedList.Count;
            int mid = count / 2;

            if (typeof(T) != typeof(short) && typeof(T) != typeof(int) && typeof(T) != typeof(long) && typeof(T) != typeof(float) && typeof(T) != typeof(double))
                throw new ArgumentException("Input classes must be of a numeric type (short, int, long, float, double).");

            return (count % 2 != 0) ? Convert.ToDouble(sortedList[mid]) : ((Convert.ToDouble(sortedList[mid]) + Convert.ToDouble(sortedList[mid + 1])) / 2.0);
        }
        public static IComparable NonNumericMedian(List<IComparable> data)
        {
            List<IComparable> sortedList = new List<IComparable>();
            sortedList.AddRange(data);
            sortedList.Sort();

            int count = sortedList.Count();
            int mid = count / 2;

            return sortedList[mid];
        }
        public static List<T> Mode<T>(List<T> data)
        {
            Dictionary<T, int> occurance = new Dictionary<T, int>();
            List<T> modes = new List<T>();

            foreach (T t in data)
            {
                if (occurance.ContainsKey(t))
                {
                    occurance[t] = occurance[t] + 1;
                }
                else
                {
                    occurance.Add(t, 1);
                }
            }

            int highestOccurance = 0;

            foreach (int v in occurance.Values) if (highestOccurance < v) highestOccurance = v;

            foreach (KeyValuePair<T, int> pair in occurance)
            {
                if (pair.Value == highestOccurance)
                {
                    modes.Add(pair.Key);
                }
            }

            if (modes.Count == 0) throw new Exception("Failed to find any modes (program error).");

            return modes;
        }
        public static Dictionary<T, int> ItemCount<T>(List<T> data)
        {
            Dictionary<T, int> occurance = new Dictionary<T, int>();

            foreach (T t in data)
            {
                if (occurance.ContainsKey(t))
                {
                    occurance[t] = occurance[t] + 1;
                }
                else
                {
                    occurance.Add(t, 1);
                }
            }

            return occurance;
        }
        public static List<IComparable> NonNumericMode(List<IComparable> data)
        {
            Dictionary<IComparable, int> occurance = new Dictionary<IComparable, int>();
            List<IComparable> modes = new List<IComparable>();

            foreach (IComparable t in data)
            {
                if (occurance.ContainsKey(t))
                {
                    occurance[t] = occurance[t] + 1;
                }
                else
                {
                    occurance.Add(t, 1);
                }
            }

            int highestOccurance = 0;

            foreach (int v in occurance.Values) if (highestOccurance < v) highestOccurance = v;

            foreach (KeyValuePair<IComparable, int> pair in occurance)
            {
                if (pair.Value == highestOccurance)
                {
                    modes.Add(pair.Key);
                }
            }

            return modes;
        }
        public static double Range<T>(List<T> data)
        {
            if (typeof(T) != typeof(short) && typeof(T) != typeof(int) && typeof(T) != typeof(long) && typeof(T) != typeof(float) && typeof(T) != typeof(double))
                throw new ArgumentException("Input classes must be of a numeric type (short, int, long, float, double).");

            List<T> sortedList = new List<T>();
            sortedList.AddRange(data);
            sortedList.Sort();

            double first = Convert.ToDouble(sortedList.First());
            double last = Convert.ToDouble(sortedList.Last());

            return Math.Abs(last - first);
        }
        public static double Variance<T>(List<T> data)
        {
            double mean = Mean<T>(data);
            double sumOfDeviationSquares = 0;
            double count = data.Count;

            if (typeof(T) == typeof(int))
            {
                foreach (T n in data) sumOfDeviationSquares += Math.Pow(Convert.ToInt32(n) - mean, 2);
            }
            else if (typeof(T) == typeof(short))
            {
                foreach (T n in data) sumOfDeviationSquares += Math.Pow(Convert.ToInt16(n) - mean, 2);
            }
            else if (typeof(T) == typeof(long))
            {
                foreach (T n in data) sumOfDeviationSquares += Math.Pow(Convert.ToInt64(n) - mean, 2);
            }
            else if (typeof(T) == typeof(double))
            {
                foreach (T n in data) sumOfDeviationSquares += Math.Pow(Convert.ToDouble(n) - mean, 2);
            }
            else if (typeof(T) == typeof(float))
            {
                foreach (T n in data)
                {
                    float f = 0.0f;
                    float.TryParse(Convert.ToString(n), out f);
                    mean += Math.Pow(f - mean, 2);
                }
            }
            else
            {
                throw new ArgumentException("Input classes must be of a numeric type (short, int, long, float, double).");
            }

            return (count >= 30) ? Math.Sqrt(sumOfDeviationSquares / count) : Math.Sqrt(sumOfDeviationSquares / (count - 1));
        }
        public static T Minimum<T>(List<T> data)
        {
            if (!(typeof(T).GetInterfaces().Contains(typeof(IComparable))))
                throw new ArgumentException("The input classes must be comparable.");

            List<T> sortedList = new List<T>();
            sortedList.AddRange(data);
            sortedList.Sort();

            return sortedList.First();
        }
        public static T Maximum<T>(List<T> data)
        {
            if (!(typeof(T).GetInterfaces().Contains(typeof(IComparable))))
                throw new ArgumentException("The input classes must be comparable.");

            List<T> sortedList = new List<T>();
            sortedList.AddRange(data);
            sortedList.Sort();

            return sortedList.Last();
        }

        public static double SimpleLinearRegression<D, T>(Dictionary<D, T> data, out double intercept)
        {
            if (typeof(D) != typeof(short) && typeof(D) != typeof(int) && typeof(D) != typeof(long) && typeof(D) != typeof(float) && typeof(D) != typeof(double) && typeof(D) != typeof(DateTime))
                throw new ArgumentException("First input class must be of a numeric type (short, int, long, float, double) or DateTime.");

            if (typeof(T) != typeof(short) && typeof(T) != typeof(int) && typeof(T) != typeof(long) && typeof(T) != typeof(float) && typeof(T) != typeof(double))
                throw new ArgumentException("Second input class must be of a numeric type (short, int, long, float, double).");

            if (data == null) throw new ArgumentNullException("There must be at least to KeyValuePairs in the dictionary to perform a linear regression. (Dictionary is null)");
            if (data.Count < 2) throw new ArgumentNullException("There must be at least to KeyValuePairs in the dictionary to perform a linear regression. (Dictionary has less than two (2) KeyValuePairs)");

            double slope = 0.0;
            double sumX = 0.0;
            double sumY = 0.0;
            double sumXX = 0.0;
            double sumXY = 0.0;
            double count = data.Count;

            foreach (KeyValuePair<D, T> pair in data)
            {
                if (typeof(D) == typeof(DateTime))
                {
                    sumX += DateTimeToEpoch(Convert.ToDateTime(pair.Key));
                    sumXX += Math.Pow(DateTimeToEpoch(Convert.ToDateTime(pair.Key)), 2);
                    sumXY += DateTimeToEpoch(Convert.ToDateTime(pair.Key)) * Convert.ToDouble(pair.Value);
                }
                else
                {
                    sumX += Convert.ToDouble(pair.Key);
                    sumXX += Math.Pow(Convert.ToDouble(pair.Key), 2);
                    sumXY += Convert.ToDouble(pair.Key) * Convert.ToDouble(pair.Value);
                }

                sumY += Convert.ToDouble(pair.Value);
            }

            slope = ((count * sumXY) - (sumX * sumY)) / ((count * sumXX) - Math.Pow(sumX, 2));
            intercept = (sumY - (slope * sumX)) / count;

            return slope;
        }
        public static double SimpleLinearRegression<T>(List<T> data)
        {
            if (typeof(T) != typeof(short) && typeof(T) != typeof(int) && typeof(T) != typeof(long) && typeof(T) != typeof(float) && typeof(T) != typeof(double))
                throw new ArgumentException("Input classes must be of a numeric type (short, int, long, float, double).");

            double intercept = 0.0;

            return SimpleLinearRegression<T>(data, out intercept);
        }
        public static double SimpleLinearRegression<T>(List<T> data, out double intercept)
        {
            if (typeof(T) != typeof(short) && typeof(T) != typeof(int) && typeof(T) != typeof(long) && typeof(T) != typeof(float) && typeof(T) != typeof(double))
                throw new ArgumentException("Input classes must be of a numeric type (short, int, long, float, double).");

            Dictionary<int, T> dictData = new Dictionary<int, T>();

            for (int i = 0; i < data.Count; i++)
            {
                dictData.Add(i, data[i]);
            }

            return SimpleLinearRegression<int, T>(dictData, out intercept);
        }

        public static double TheilSenEstimation<D, T>(Dictionary<D, T> data, out double intercept)
        {
            if (typeof(D) != typeof(short) && typeof(D) != typeof(int) && typeof(D) != typeof(long) && typeof(D) != typeof(float) && typeof(D) != typeof(double) && typeof(D) != typeof(DateTime))
                throw new ArgumentException("First input class must be of a numeric type (short, int, long, float, double) or DateTime.");

            if (typeof(T) != typeof(short) && typeof(T) != typeof(int) && typeof(T) != typeof(long) && typeof(T) != typeof(float) && typeof(T) != typeof(double))
                throw new ArgumentException("Second input class must be of a numeric type (short, int, long, float, double).");

            double slope = 0.0;

            data.OrderBy(d => d.Key);

            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();
            List<double> slopes = new List<double>();
            List<double> intercepts = new List<double>();

            foreach (KeyValuePair<D, T> pair in data)
            {
                xValues.Add((typeof(D) != typeof(DateTime)) ? Convert.ToDouble(pair.Key) : DateTimeToEpoch(Convert.ToDateTime(pair.Key)));
                yValues.Add(Convert.ToDouble(pair.Value));
            }

            if (xValues.Count != yValues.Count)
                throw new Exception("Failed to extract all of the data points from the dictionary. \nX & Y array sizes not the same.");

            for (int i = 0; i < xValues.Count - 1; i++)
            {
                for (int j = i; j < xValues.Count; j++)
                {
                    slopes.Add((yValues[j] - yValues[i]) / (xValues[j] - xValues[i]));
                }
            }

            slope = Median<double>(slopes);

            for (int k = 0; k < yValues.Count; k++) intercepts.Add(yValues[k] - (slope * xValues[k]));

            intercept = Median<double>(intercepts);

            return slope;
        }

        public static double StandardDeviationsFromMean<T>(List<T> data, T point)
        {
            if (typeof(T) != typeof(short) && typeof(T) != typeof(int) && typeof(T) != typeof(long) && typeof(T) != typeof(float) && typeof(T) != typeof(double))
                throw new ArgumentException("Input classes must be of a numeric type (short, int, long, float, double).");

            double m = Mean<T>(data);
            double v = Variance<T>(data);

            double d = Convert.ToDouble(point) - m;

            return d / v;
        }
        public static bool WithinStandardDeviationsOfMean<T>(List<T> data, T point, double standardDeviations)
        {
            return Math.Abs(StandardDeviationsFromMean<T>(data, point)) <= standardDeviations;
        }
        public static bool WithinStandardDeviationsOfMean<T>(List<T> data, T point, double sdLow, double sdHigh)
        {
            return (sdLow <= StandardDeviationsFromMean<T>(data, point) && StandardDeviationsFromMean<T>(data, point) <= sdHigh);
        }
        public static bool BetweenStandardDeviationsOfMean<T>(List<T> data, T point, double standardDeviations)
        {
            return Math.Abs(StandardDeviationsFromMean<T>(data, point)) < standardDeviations;
        }
        public static bool BetweenStandardDeviationsOfMean<T>(List<T> data, T point, double sdLow, double sdHigh)
        {
            return (sdLow < StandardDeviationsFromMean<T>(data, point) && StandardDeviationsFromMean<T>(data, point) < sdHigh);
        }
        public static double ValueAtStandardDeviations<T>(List<T> data, double sd)
        {
            double mean = Mean<T>(data);
            double variance = Variance<T>(data);

            return (mean + (variance * sd));
        }

        public static double DateTimeToEpoch(DateTime time)
        {
            DateTime epochBase = new DateTime(1970, 1, 1, 0, 0, 0);
            TimeSpan epochCalc = time - epochBase;

            return epochCalc.TotalSeconds;
        }
        public static DateTime EpochToDateTime(double time)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(time);
        }

        public static bool IsNumeric(object value)
        {
            return value is short
                || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }
    }
}
