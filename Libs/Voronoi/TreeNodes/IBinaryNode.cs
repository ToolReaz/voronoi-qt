using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voronoi.TreeNodes
{
    public interface IBinaryNode
    {
        double X { get; }

        double Y { get; }

        IBinaryNode Left { get; set; }

        IBinaryNode Right { get; set; }

        IBinaryNode Parent { get; set; }

        void CalculateInnerBreakpoint(double sweepLine);
    }
}