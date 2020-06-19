using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Voronoi.TreeNodes;

namespace Voronoi.Extensions
{
    public static class TreeNodeExtensions
    {
        private static IBinaryNode LeftChildAccessor(IBinaryNode node) => node.Left;

        private static IBinaryNode RightChildAccessor(IBinaryNode node) => node.Right;

        private delegate IBinaryNode TreeNodeAccessor(IBinaryNode node);

        public static IBinaryNode GetNext(this IBinaryNode current) => current.GetNeighborNode(false);

        private static IBinaryNode GetNeighborNode(this IBinaryNode current, bool before) {
            var accessor1 = before ? (TreeNodeAccessor) LeftChildAccessor : RightChildAccessor;
            var accessor2 = before ? (TreeNodeAccessor) RightChildAccessor : LeftChildAccessor;

            if (accessor1(current) != null) {
                var neighborNode = accessor1(current);

                while (accessor2(neighborNode) != null) {
                    neighborNode = accessor2(neighborNode);
                }

                return neighborNode;
            }

            if (current.Parent != null
                && accessor2(current.Parent) == current) {
                return current.Parent;
            }

            if (current.Parent != null) {
                var neighborNode = current.Parent;

                while (neighborNode.Parent != null) {
                    if (accessor2(neighborNode.Parent) == neighborNode)
                        return neighborNode.Parent;

                    neighborNode = neighborNode.Parent;
                }

                return null;
            }

            return null;
        }

        public static Leaf GetNextLeaf(this IBinaryNode current) {
            var next = current.GetNext();

            while (next != null) {
                if (next is Leaf leaf) {
                    return leaf;
                }

                next = next.GetNext();
            }

            return null;
        }

        public static IBinaryNode GetPrevious(this IBinaryNode current) => current.GetNeighborNode(true);

        public static Leaf GetPreviousLeaf(this IBinaryNode current) {
            var prev = current.GetPrevious();

            while (prev != null) {
                if (prev is Leaf leaf) {
                    return leaf;
                }

                prev = prev.GetPrevious();
            }

            return null;
        }
    }
}