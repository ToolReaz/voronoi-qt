using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Voronoi.Events;

namespace Voronoi.Interop
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct Line
    {
        public Point Point1;
        public Point Point2;

        public Line(CircleEvent c1, CircleEvent c2) {
            Point1 = new Point(c1);
            Point2 = new Point(c2);
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct Point
    {
        public double X;
        public double Y;

        public Point(CircleEvent evt) {
            X = evt.X;
            Y = evt.Y;
        }
    }
}
