using System;
using System.Diagnostics;

using C5;

using Voronoi.TreeNodes;

using static Voronoi.COMPILATION_CONSTANTS;

namespace Voronoi.Events
{
    public class CircleEvent : IEvent
    {
        public PartialLine PartialLine;

        /// <summary>
        /// The X-coordinate of the center of the circle
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// The Y-coordinate of the center of the circle
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// The radius of the circle
        /// </summary>
        public double Radius { get; set; }
        public TreeItem Left { get; }

        public TreeItem Center { get; }

        public TreeItem Right { get; }

        public Leaf LeftLeaf { get; }

        public Leaf CenterLeaf { get; }

        public Leaf RightLeaf { get; }

        public override string ToString() {
            return $"CircleEvent ({X};{Y}), Radius {Radius}";
        }

        public CircleEvent(double x, double y) {
            X = x;
            Y = y;
        }

        public IPriorityQueueHandle<IEvent> Handle { get; set; }

        public CircleEvent(Leaf left, Leaf center, Leaf right) {
            LeftLeaf = left;
            RightLeaf = right;
            CenterLeaf = center;

            Left = LeftLeaf.ListItem.Copy();
            Right = RightLeaf.ListItem.Copy();
            Center = CenterLeaf.ListItem.Copy();

            CalculateCircle();
        }

        /// <summary>
        /// The distance between the sweep line and the circle center
        /// </summary>
        public double DistanceFromLine => Y - Radius;

        public bool CalculateCircle() {
            var point1 = Left.Point;
            var point2 = Center.Point;
            var point3 = Right.Point;

            if (Math.Abs(point2.X - point3.X) < EPSILON_DOUBLE
                && Math.Abs(point2.X - point1.X) < EPSILON_DOUBLE) {
                return false;
            }

            {
                var grad12 = (point2.Y - point1.Y) / (point2.X - point1.X);
                var grad23 = (point3.Y - point2.Y) / (point3.X - point2.X);

                if (Math.Abs(grad12 - grad23) < EPSILON_DOUBLE) {
                    return false;
                }
            }

            if (Math.Abs(point2.Y - point3.Y) < EPSILON_DOUBLE) {
                point1 = Right.Point;
                point3 = Left.Point;
            }

            if (Math.Abs(point2.X - point1.X) < EPSILON_DOUBLE
                || Math.Abs(point2.Y - point1.Y) < EPSILON_DOUBLE
                || Math.Abs(point2.X - point3.X) < EPSILON_DOUBLE
                || Math.Abs(point2.Y - point3.Y) < EPSILON_DOUBLE
                || Math.Abs(point1.X - point3.X) < EPSILON_DOUBLE
                || Math.Abs(point1.Y - point3.Y) < EPSILON_DOUBLE) {
                RotateCircle(11.3);
            } else {
                (X, Y, Radius) = GetCircle(point1, point2, point3);
            }

            return true;
        }

        public void RotateCircle(double degrees) {
            var cos = Math.Cos(degrees * Math.PI / 180);
            var sin = Math.Sin(degrees * Math.PI / 180);

            var point1 = Left.Point;
            var point2 = Center.Point;
            var point3 = Right.Point;

            var p1X = (cos * point1.X - sin * point1.Y);
            var p1Y = (sin * point1.X + cos * point1.Y);

            point1 = new VoronoiVertex(p1X, p1Y);

            var p2X = (cos * point2.X - sin * point2.Y);
            var p2Y = (sin * point2.X + cos * point2.Y);

            point2 = new VoronoiVertex(p2X, p2Y);

            var p3X = (cos * point3.X - sin * point3.Y);
            var p3Y = (sin * point3.X + cos * point3.Y);

            point3 = new VoronoiVertex(p3X, p3Y);

            var (centerX, centerY, radius) = GetCircle(point1, point2, point3);

            var centerX2 = (cos * centerX + sin * centerY);
            var centerY2 = (-sin * centerX + cos * centerY);
            Radius = radius;
            X = centerX2;
            Y = centerY2;
        }

        /// <summary>
        /// Get the coordinates of the circle and its radius based on 3 points
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <returns></returns>
        private static (double, double, double) GetCircle(Vertex point1, Vertex point2, Vertex point3)
        {
            // Get the center of the circle based on the intersection point between the bisector of point (1, 2) and (2, 3)
            var normal12 = -(point2.X - point1.X) / (point2.Y - point1.Y);

            var normal23 = -(point3.X - point2.X) / (point3.Y - point2.Y);

            var const12 = 0.5 * (point1.Y + point2.Y) - 0.5 * normal12 * (point1.X + point2.X);
            var const23 = 0.5 * (point2.Y + point3.Y) - 0.5 * normal23 * (point2.X + point3.X);

            var centerX = (const23 - const12) / (normal12 - normal23);
            var centerY = normal23 * centerX + const23;

            var radius = Math.Sqrt((point1.X - centerX) * (point1.X - centerX)
                                    + (point1.Y - centerY) * (point1.Y - centerY));
            return (centerX, centerY, radius);
        }

        public bool CheckConvergence() => (Left.Point.Y -  Center.Point.Y) 
                                          * (Right.Point.X - Center.Point.X) 
                                          <= (Left.Point.X -  Center.Point.X) 
                                          * (Right.Point.Y - Center.Point.Y);
    }
}