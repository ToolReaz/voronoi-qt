using System;
using System.Collections.Generic;

using Voronoi.Events;
using Voronoi.Extensions;
using Voronoi.TreeNodes;

using static Voronoi.COMPILATION_CONSTANTS;

namespace Voronoi
{
    public class BinaryTree
    {
        public double MaxY { get; set; }
        public List<PartialLine> PartialLines { get; } = new List<PartialLine>();

        public IBinaryNode Root { get; private set; }

        public void SetRoot(IBinaryNode root) {
            Root = root;
            Root.Parent = null;
        }

        public IBinaryNode GetNearest(Builder builder, IBinaryNode current, SeedEvent seedEvent, double edgeLimit) {
            while (true) {
                var x = seedEvent.X;
                var y = seedEvent.Y;

                if ((int) y == (int) MaxY) {
                    var le = new TreeItem(seedEvent.Vertex);
                    var evt = new Leaf(le);
                    var next = Root;

                    while (next.GetNextLeaf() != null) {
                        next = next.GetNextLeaf();
                    }

                    if (!(next is Leaf nextLeaf)) return null;

                    var h1 = new PartialLine(nextLeaf.ListItem.Point);
                    var h2 = new PartialLine(le.Point);

                    h1.Twin = h2;
                    h2.Twin = h1;
                    PartialLines.Add(h1);
                    PartialLines.Add(h2);

                    if (nextLeaf.ListItem.Point.PartialLine == null) {
                        nextLeaf.ListItem.Point.PartialLine = h1;
                    } else {
                        var n = nextLeaf.ListItem.Point.PartialLine;

                        while (n.Next != null) {
                            n = n.Next;
                        }

                        n.Next = h1;
                    }

                    nextLeaf.ListItem.PartialLine = h1;
                    le.PartialLine = h1;

                    le.Point.PartialLine = h2;

                    var b0 = new Breakpoint(nextLeaf.ListItem, evt.ListItem);
                    var bpNode0 = new BreakpointNode(b0);
                    nextLeaf.BreakpointNode = bpNode0;

                    if (next == Root) {
                        next.Parent = bpNode0;
                        Root = bpNode0;
                        bpNode0.Left = next;
                        bpNode0.Right = evt;
                        evt.Parent = bpNode0;
                    } else {
                        var parent = next.Parent;
                        parent.Right = bpNode0;
                        bpNode0.Parent = parent;
                        bpNode0.Left = next;
                        next.Parent = bpNode0;
                        bpNode0.Right = evt;
                        evt.Parent = bpNode0;
                    }

                    b0.InitializeBreakpoint(y);
                    b0.X = (x + next.X) / 2;
                    b0.Y = y + edgeLimit;

                    var ce = new CircleEvent(b0.X, b0.Y);

                    h1.CircleEvent = ce;
                    ce.PartialLine = h1;

                    return null;
                }

                current.CalculateInnerBreakpoint(y - 0.0001);

                if (Math.Abs(x - current.X) < EPSILON_DOUBLE) return current;

                var child = x > current.X ? current.Right : current.Left;
                if (child is null) return current;

                current = child;
            }
        }

        public List<CircleEvent> AddSeedEvent(Leaf current, TreeItem listItem) {
            var circleEvents = new List<CircleEvent>();

            var currentVector2 = current.ListItem.Point;
            var newVector2 = listItem.Point;
            var h1 = new PartialLine(currentVector2);
            var h2 = new PartialLine(newVector2);
            h1.Twin = h2;
            h2.Twin = h1;
            PartialLines.Add(h1);
            PartialLines.Add(h2);

            if (currentVector2.PartialLine == null) {
                currentVector2.PartialLine = h1;
            }

            newVector2.PartialLine = h2;

            var oldHe = current.ListItem.PartialLine;
            current.ListItem.PartialLine = h1;
            listItem.PartialLine = h1;

            var prev = current.GetPreviousLeaf();
            var next = current.GetNextLeaf();

            var newNode = new Leaf(listItem);
            var copy = current.ListItem.Copy();
            var copyNode = new Leaf(copy);

            var bp1 = new Breakpoint(current.ListItem, listItem);
            bp1.InitializeBreakpoint(listItem.Point.Y - 1);

            var bp2 = new Breakpoint(listItem, copy);
            bp2.InitializeBreakpoint(listItem.Point.Y - 1);

            var bp1Node = new BreakpointNode(bp1);
            var bp2Node = new BreakpointNode(bp2);

            current.BreakpointNode = bp1Node;
            newNode.BreakpointNode = bp2Node;

            if (current.Parent != null) {
                if (current == current.Parent.Left) {
                    current.Parent.Left = bp1Node;
                    bp1Node.Parent = current.Parent;
                } else {
                    current.Parent.Right = bp1Node;
                    bp1Node.Parent = current.Parent;
                }
            } else {
                SetRoot(bp1Node);
                Root.Parent = null;
            }

            bp1Node.Left = current;
            current.Parent = bp1Node;

            bp1Node.Right = bp2Node;
            bp2Node.Parent = bp1Node;

            bp2Node.Left = newNode;
            newNode.Parent = bp2Node;

            bp2Node.Right = copyNode;
            copyNode.Parent = bp2Node;

            if (oldHe != null) {
                copyNode.ListItem.PartialLine = oldHe;
                var pre = copyNode.ListItem.PartialLine;

                if (!pre.Face.Equals(currentVector2)) {
                    pre = pre.Twin;
                }

                var onext = pre.Next;
                pre.Next = h1;
                h1.Previous = pre;

                if (onext != null) {
                    onext.Previous = h1;
                    h1.Next = onext;
                }
            }

            if (prev != null && prev is Leaf prevLeaf) {
                var circleEvent = new CircleEvent(prevLeaf, current, newNode);
                var canCalculateCircle = circleEvent.CalculateCircle();

                prevLeaf.BreakpointNode.Breakpoint = new Breakpoint(prevLeaf.ListItem, current.ListItem);

                bp1Node.Breakpoint.InitializeBreakpoint(circleEvent.DistanceFromLine);
                prevLeaf.BreakpointNode.Breakpoint.InitializeBreakpoint(circleEvent.DistanceFromLine);

                if (canCalculateCircle && circleEvent.CheckConvergence()) {
                    current.DisappearEvent = circleEvent;
                    circleEvents.Add(circleEvent);
                }
            }

            if (next != null && next is Leaf nextLeaf) {
                var circleEvent = new CircleEvent(newNode, copyNode, nextLeaf);
                var canCalculateCircle = circleEvent.CalculateCircle();

                copyNode.BreakpointNode = (BreakpointNode) copyNode.GetNext();
                copyNode.BreakpointNode.Breakpoint = new Breakpoint(copy, nextLeaf.ListItem);

                bp2Node.Breakpoint.InitializeBreakpoint(circleEvent.DistanceFromLine);
                copyNode.BreakpointNode.Breakpoint.InitializeBreakpoint(circleEvent.DistanceFromLine);

                if (canCalculateCircle && circleEvent.CheckConvergence()) {
                    copyNode.DisappearEvent = circleEvent;
                    circleEvents.Add(circleEvent);
                }
            } else {
                copyNode.ListItem.PartialLine = h1;
            }

            return circleEvents;
        }

        public List<CircleEvent> RemoveNode(CircleEvent ce, Leaf prev, Leaf remove, Leaf next) {
            var circleEvents = new List<CircleEvent>();

            var currentVector2 = prev.ListItem.Point;
            var newVector2 = next.ListItem.Point;
            var h1 = new PartialLine(currentVector2);
            var h2 = new PartialLine(newVector2);
            h1.Twin = h2;
            h2.Twin = h1;
            PartialLines.Add(h1);
            PartialLines.Add(h2);

            h1.CircleEvent = ce;
            var bp1 = new Breakpoint(prev.ListItem, next.ListItem);

            if (prev.BreakpointNode == remove.Parent) {
                prev.BreakpointNode = remove.BreakpointNode;
            }

            prev.BreakpointNode.Breakpoint = bp1;

            {
                var oldEdgeP = prev.ListItem.PartialLine;

                if (!oldEdgeP.Face.Equals(prev.ListItem.Point)) {
                    oldEdgeP = oldEdgeP.Twin;
                }

                var n = oldEdgeP.Next;
                oldEdgeP.Next = h1;
                h1.Previous = oldEdgeP;

                if (n != null) {
                    h1.Next = n;
                    n.Previous = h1;
                }
            }

            {
                var oldEdgeN = next.ListItem.PartialLine;

                if (!oldEdgeN.Face.Equals(next.ListItem.Point)) {
                    oldEdgeN = oldEdgeN.Twin;
                }

                var n = oldEdgeN.Next;

                oldEdgeN.Next = h2;
                h2.Previous = oldEdgeN;

                if (n != null) {
                    h2.Next = n;
                    n.Previous = h2;
                }
            }

            prev.ListItem.PartialLine = h1;

            if (remove.Parent.Parent == null) {
                Root = remove.Parent.Left == remove
                    ? remove.Parent.Right
                    : remove.Parent.Left;
                Root.Parent = null;
            } else {
                var grandparent = remove.Parent.Parent;
                var parent = remove.Parent;

                if (remove == remove.Parent.Left) {
                    if (remove.Parent == grandparent.Left) {
                        grandparent.Left = parent.Right;
                        parent.Right.Parent = grandparent;
                    } else {
                        grandparent.Right = parent.Right;
                        parent.Right.Parent = grandparent;
                    }
                }

                else {
                    if (remove.Parent == grandparent.Left) {
                        grandparent.Left = parent.Left;
                        parent.Left.Parent = grandparent;
                    } else {
                        grandparent.Right = parent.Left;
                        parent.Left.Parent = grandparent;
                    }
                }

                remove.Parent = null;
            }

            var nextNextTree = next.GetNextLeaf();

            if (nextNextTree != null
                && nextNextTree is Leaf nextNext) {
                var circleEvent = new CircleEvent(prev, next, nextNext);
                var canCalculateCircle = circleEvent.CalculateCircle();
                
                next.BreakpointNode.Breakpoint = new Breakpoint(next.ListItem, nextNext.ListItem);

                bp1.InitializeBreakpoint(circleEvent.DistanceFromLine);

                next.BreakpointNode.Breakpoint.InitializeBreakpoint(circleEvent.DistanceFromLine);

                if (canCalculateCircle && circleEvent.CheckConvergence()) {
                    next.DisappearEvent = circleEvent;

                    circleEvents.Add(circleEvent);
                }
            }

            var prevPrevTree = prev.GetPreviousLeaf();

            if (prevPrevTree != null
                && prevPrevTree is Leaf prevPrev) {
                var circleEvent = new CircleEvent(prevPrev, prev, next);
                var canCalculateCircle = circleEvent.CalculateCircle();

                prevPrev.BreakpointNode.Breakpoint = new Breakpoint(prevPrev.ListItem, prev.ListItem);
                bp1.InitializeBreakpoint(circleEvent.DistanceFromLine);

                prevPrev.BreakpointNode.Breakpoint.InitializeBreakpoint(circleEvent.DistanceFromLine);

                if (canCalculateCircle && circleEvent.CheckConvergence()) {
                    prev.DisappearEvent = circleEvent;

                    circleEvents.Add(circleEvent);
                }
            }

            return circleEvents;
        }
    }
}