using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Voronoi.Events;

namespace Voronoi.TreeNodes
{
    public class Leaf : IBinaryNode
    {
        public Leaf(TreeItem item) {
            ListItem = item;
        }

        public TreeItem ListItem { get; }

        public BreakpointNode BreakpointNode { get; set; }

        public CircleEvent DisappearEvent { get; set; }

        public double X => ListItem.Point.X;

        public double Y => ListItem.Point.Y;

        /// <summary>
        /// A leaf should not have any children
        /// </summary>
        [Obsolete("This property will always return null and any assigned value will be discarded")]
        public IBinaryNode Left {
            get => null;
            set { }
        }

        /// <summary>
        /// A leaf should not have any children
        /// </summary>
        [Obsolete("This property will always return null and any assigned value will be discarded")]
        public IBinaryNode Right {
            get => null;
            set { }
        }

        public IBinaryNode Parent { get; set; }

        [Obsolete("This method does nothing and is only here for the interface implementation")]
        public void CalculateInnerBreakpoint(double sweepLine) {
            // do nothing
        }

        public override string ToString() {
            return $"X={X}; Y={Y}";
        }
    }
}