using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voronoi.TreeNodes
{
    public class BreakpointNode : IBinaryNode
    {
        public BreakpointNode(Breakpoint breakpoint) {
            Breakpoint = breakpoint;
        }

        public Breakpoint Breakpoint { get; set; }

        public double X => Breakpoint.X;

        public double Y => Breakpoint.Y;

        public IBinaryNode Left { get; set; }

        public IBinaryNode Right { get; set; }

        public IBinaryNode Parent { get; set; }

        public void CalculateInnerBreakpoint(double sweepLine) {
            Breakpoint.InitializeBreakpoint(sweepLine);
        }

        public override string ToString() {
            return $"X={X}; Y={Y}";
        }
    }
}