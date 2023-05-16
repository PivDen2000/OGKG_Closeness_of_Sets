using KD_Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_OGKG
{
    public class KD
    {
        public List<double[]> points1 = new List<double[]>();
        public List<double[]> points2 = new List<double[]>();
        public double[] firstPoint;
        public double[] secondPoint;
        public double minDistance;

        private static Func<double[], double[], double> L2Norm_Squared_Double = (x, y) =>
        {
            double dist = 0f;
            for (int i = 0; i < x.Length; i++)
            {
                dist += (x[i] - y[i]) * (x[i] - y[i]);
            }

            return dist;
        };

        public void FindNearest()
        {
            if ((points1.Count == 0) || (points2.Count == 0)) return;
            List<int> array = new List<int>();
            for ( int i = 0; i < points1.Count; i++)
            {
                array.Add(i);
            }
            var tree = new KDTree<double, int>(2, points1.ToArray(), array.ToArray() , L2Norm_Squared_Double);
            var nearest = tree.NearestNeighbors(points2[0], 1);
            minDistance = L2Norm_Squared_Double(nearest.First().Item1, points2[0]);
            firstPoint = points2[0];
            secondPoint = nearest.First().Item1;
            for (int i = 1; i < points2.Count; i++)
            {
                nearest = tree.NearestNeighbors(points2[i], 1);
                double distance = L2Norm_Squared_Double(nearest.First().Item1, points2[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    firstPoint = points2[i];
                    secondPoint = nearest.First().Item1;
                }
            }
        }

        public void RandomGenerate(int A, int B, int max)
        {
            Random rnd = new Random();
            for (int i = 0; i < A; i++)
            {
                double x = rnd.NextDouble() * ((double)max);
                double y = rnd.NextDouble() * ((double)max);
                double[] point = new double[] { x, y };
                points1.Add(point);
            }

            for (int i = 0; i < B; i++)
            {
                double x = rnd.NextDouble() * ((double)max);
                double y = rnd.NextDouble() * ((double)max);
                double[] point = new double[] { x, y };
                points2.Add(point);
            }
        }
    }
}
