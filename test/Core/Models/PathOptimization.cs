using System;
using System.Collections.Generic;
using Avalonia;

namespace Autodraw.Core.Models
{
    public static class PathOptimization
    {
        /// <summary>
        /// Simplifies a path using an ITERATIVE Ramer-Douglas-Peucker algorithm.
        /// Prevents StackOverflow on complex images.
        /// </summary>
        public static List<Point> SimplifyPath(List<Point> points, double tolerance)
        {
            if (points == null || points.Count < 3)
                return points ?? new List<Point>();

            int n = points.Count;
            bool[] usePoint = new bool[n];
            for (int i = 0; i < n; i++) usePoint[i] = false;

            // Use an explicit stack instead of recursion
            Stack<(int start, int end)> stack = new Stack<(int, int)>();
            stack.Push((0, n - 1));
            usePoint[0] = true;
            usePoint[n - 1] = true;

            while (stack.Count > 0)
            {
                var range = stack.Pop();
                int first = range.start;
                int last = range.end;

                double maxDistance = 0;
                int indexFarthest = 0;

                for (int i = first + 1; i < last; i++)
                {
                    double distance = PerpendicularDistance(points[first], points[last], points[i]);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        indexFarthest = i;
                    }
                }

                if (maxDistance > tolerance && indexFarthest != 0)
                {
                    usePoint[indexFarthest] = true;
                    stack.Push((first, indexFarthest));
                    stack.Push((indexFarthest, last));
                }
            }

            List<Point> returnPoints = new List<Point>();
            for (int i = 0; i < n; i++)
            {
                if (usePoint[i]) returnPoints.Add(points[i]);
            }
            return returnPoints;
        }

        private static double PerpendicularDistance(Point p1, Point p2, Point p)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            double mag = Math.Sqrt(dx * dx + dy * dy);
            if (mag == 0) return Math.Sqrt(Math.Pow(p.X - p1.X, 2) + Math.Pow(p.Y - p1.Y, 2));
            return Math.Abs(dy * p.X - dx * p.Y + (p2.X * p1.Y - p2.Y * p1.X)) / mag;
        }

        public static List<Point> GenerateBezierPath(List<Point> points, int resolution = 10)
        {
            if (points == null || points.Count < 3) return points ?? new List<Point>();
            var smoothPath = new List<Point>();
            int n = points.Count - 1;

            GetCurveControlPoints(points.ToArray(), out Point[] p1, out Point[] p2);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    double t = (double)j / resolution;
                    smoothPath.Add(CalculateBezierPoint(t, points[i], p1[i], p2[i], points[i + 1]));
                }
            }
            smoothPath.Add(points[n]);
            return smoothPath;
        }

        private static Point CalculateBezierPoint(double t, Point p0, Point p1, Point p2, Point p3)
        {
            double u = 1 - t;
            double tt = t * t, uu = u * u;
            double uuu = uu * u, ttt = tt * t;
            return new Point(
                uuu * p0.X + 3 * uu * t * p1.X + 3 * u * tt * p2.X + ttt * p3.X,
                uuu * p0.Y + 3 * uu * t * p1.Y + 3 * u * tt * p2.Y + ttt * p3.Y
            );
        }

        private static void GetCurveControlPoints(Point[] knots, out Point[] firstControlPoints, out Point[] secondControlPoints)
        {
            int n = knots.Length - 1;
            if (n == 1)
            {
                firstControlPoints = new[] { new Point((2 * knots[0].X + knots[1].X) / 3, (2 * knots[0].Y + knots[1].Y) / 3) };
                secondControlPoints = new[] { new Point((knots[0].X + 2 * knots[1].X) / 3, (knots[0].Y + 2 * knots[1].Y) / 3) };
                return;
            }

            double[] rhsX = new double[n], rhsY = new double[n];
            for (int i = 1; i < n - 1; ++i)
            {
                rhsX[i] = 4 * knots[i].X + 2 * knots[i + 1].X;
                rhsY[i] = 4 * knots[i].Y + 2 * knots[i + 1].Y;
            }
            rhsX[0] = knots[0].X + 2 * knots[1].X;
            rhsY[0] = knots[0].Y + 2 * knots[1].Y;
            rhsX[n - 1] = (8 * knots[n - 1].X + knots[n].X) / 2.0;
            rhsY[n - 1] = (8 * knots[n - 1].Y + knots[n].Y) / 2.0;

            double[] x = GetFirstControlPoints(rhsX);
            double[] y = GetFirstControlPoints(rhsY);

            firstControlPoints = new Point[n];
            secondControlPoints = new Point[n];
            for (int i = 0; i < n; ++i)
            {
                firstControlPoints[i] = new Point(x[i], y[i]);
                if (i < n - 1)
                    secondControlPoints[i] = new Point(2 * knots[i + 1].X - x[i + 1], 2 * knots[i + 1].Y - y[i + 1]);
                else
                    secondControlPoints[i] = new Point((knots[n].X + x[n - 1]) / 2, (knots[n].Y + y[n - 1]) / 2);
            }
        }

        private static double[] GetFirstControlPoints(double[] rhs)
        {
            int n = rhs.Length;
            double[] x = new double[n], tmp = new double[n];
            double b = 2.0;
            x[0] = rhs[0] / b;
            for (int i = 1; i < n; i++)
            {
                tmp[i] = 1 / b;
                b = (i < n - 1 ? 4.0 : 3.5) - tmp[i];
                x[i] = (rhs[i] - x[i - 1]) / b;
            }
            for (int i = n - 2; i >= 0; i--)
            {
                x[i] -= tmp[i + 1] * x[i + 1];
            }
            return x;
        }
    }
}