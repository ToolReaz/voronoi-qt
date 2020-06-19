using System;
using System.Diagnostics;

namespace Voronoi
{
    public class Breakpoint
    {
        public Breakpoint(TreeItem listEvent1, TreeItem listEvent2) {
            LeftListEvent = listEvent1;
            RightListEvent = listEvent2;
        }

        public TreeItem LeftListEvent { get; }

        public TreeItem RightListEvent { get; }

        public double X { get; set; }

        public double Y { get; set; }

        public void InitializeBreakpoint(double sweepLine) {
            var p = LeftListEvent.Point;
            var q = RightListEvent.Point;

            var sweepLineSquared = sweepLine * sweepLine;

            var alpha = 0.5 / (p.Y - sweepLine);
            var beta = 0.5 / (q.Y - sweepLine);

            var a = alpha - beta;
            var b = -2 * (alpha * p.X - beta * q.X);
            var c =
                alpha * (p.Y * p.Y + p.X * p.X - sweepLineSquared) - beta * (q.Y * q.Y + q.X * q.X - sweepLineSquared);

            if (Math.Abs(p.Y - q.Y) < COMPILATION_CONSTANTS.EPSILON_DOUBLE) {
                // If it is a vertical line
                X = -c / b;
            } else {
                (X, _) = SolveQuadratic(a, b, c);
            }

            Y = (0.5 / (p.Y - sweepLine)) * ((X - p.X) * (X - p.X) + p.Y * p.Y - sweepLineSquared);
        }

        private (double, double) SolveQuadratic(double a, double b, double c) {
            var disc = b * b - 4 * a * c;

            if (disc < 0)
            {
                // Theoretically impossible.
                // Should not happen unless floating point precision is messed up (a, b or c are too big)

                // throw new ArithmeticException("Discriminant is below 0. Cannot solve quadratic equation in R.");
                Debugger.Log(9, "Floating point precision error", $"A: {a}, B: {b}, C: {c}{Environment.NewLine}");
                return (double.NaN, double.NaN);
            }

            return ((-b + Math.Sqrt(disc)) / (2 * a), (-b - Math.Sqrt(disc)) / (2 * a));
        }
    }
}