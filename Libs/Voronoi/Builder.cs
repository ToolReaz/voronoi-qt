using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Voronoi.Events;
using Voronoi.Extensions;
using Voronoi.TreeNodes;

using static Voronoi.COMPILATION_CONSTANTS;

namespace Voronoi
{
    public class Builder
    {
        /// <summary>
        /// The priority queue for all events
        /// </summary>
        private C5.IntervalHeap<IEvent> PriorityQueue { get; } = new C5.IntervalHeap<IEvent>(new Comparator());

        /// <summary>
        /// Binary tree for the events
        /// </summary>
        private BinaryTree Events { get; } = new BinaryTree();

        /// <summary>
        /// The same array as <see cref="Seeds"/> except with the related lines forming the voronoi regions
        /// </summary>
        internal VoronoiVertex[] VoronoiVertices { get; }

        /// <summary>
        /// The array of seeds (nodes generating a seed event and placed before generating the diagram)
        /// </summary>
        internal Vertex[] Seeds { get; }

        internal double MaximumEdgeLimit { get; set; } = 9000;



        /// <summary>
        /// Main entry point for voronoi diagram generation
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static IEnumerable<PartialLine> GetVoronoi(Vertex[] points) {
            var generator = new Builder(points.ToHashSet().ToArray()); // to remove duplicate points
            generator.AddSeedEvents();
            generator.BuildDiagram();
            var lines = generator.VoronoiVertices.SelectMany(n => n.GetPartialLines());
            return lines.ToArray();
        }


        internal Builder(Vertex[] nodeList) {
            Seeds = nodeList;
            VoronoiVertices = new VoronoiVertex[Seeds.Length];
        }

        /// <summary>
        /// Add the seed events for each seed in the diagram (known prior to the diagram generation)
        /// </summary>
        private void AddSeedEvents() {
            for (var i = 0; i < Seeds.Length; i++) {
                var se = new SeedEvent(Seeds[i]);
                VoronoiVertices[i] = se.Vertex;
                C5.IPriorityQueueHandle<IEvent> h = null;
                PriorityQueue.Add(ref h, se);
                se.Handle = h;
            }
        }

        /// <summary>
        /// Generate the voronoi diagram
        /// </summary>
        internal void BuildDiagram() {
            while (PriorityQueue.Count != 0) {
                var nextEvent = PriorityQueue.DeleteMin();
                PerformStep(nextEvent);
            }

            FinalizeDiagram(Events.Root);
        }

        /// <summary>
        /// Perform a given step as if the sweep line reached an other event
        /// </summary>
        /// <param name="nextEvent"></param>
        internal void PerformStep(IEvent nextEvent) {
            if (Events.Root == null
                && nextEvent is SeedEvent siteEvent)
            {
                Events.SetRoot(new Leaf(new TreeItem(siteEvent.Vertex)));
                Events.MaxY = siteEvent.Y;
            }
            else
            {
                if (nextEvent is SeedEvent siteEventA)
                {
                    ProcessSeedEvent(siteEventA);
                }
                else if (nextEvent is CircleEvent circleEvent)
                {
                    ProcessCircleEvent(circleEvent);
                }
            }
        }

        /// <summary>
        /// Perform the necessary actions once the sweep line reach a seed event (meets a seed)
        /// </summary>
        /// <param name="seedEvent"></param>
        private void ProcessSeedEvent(SeedEvent seedEvent) {
            var node = Events.GetNearest(this, Events.Root, seedEvent, MaximumEdgeLimit);

            if (node is null || node is BreakpointNode)
            {
                return;
            }

            if (Math.Abs(node.X - seedEvent.X) < EPSILON_DOUBLE
                && Math.Abs(node.Y - seedEvent.Y) < EPSILON_DOUBLE) {
                return;
            }

            if (node is Leaf closestLeafNode) {
                if (closestLeafNode.DisappearEvent != null) {
                    PriorityQueue.Delete(closestLeafNode.DisappearEvent.Handle);
                    closestLeafNode.DisappearEvent = null;
                }

                foreach (var circleEvent in Events.AddSeedEvent(closestLeafNode, new TreeItem(seedEvent.Vertex))) {
                    C5.IPriorityQueueHandle<IEvent> h = null;
                    PriorityQueue.Add(ref h, circleEvent);
                    circleEvent.Handle = h;
                }
            }
        }

        private void ProcessCircleEvent(CircleEvent circleEvent) {
            var leftHalfEdge = circleEvent.Left.PartialLine;

            if (leftHalfEdge.Face.Equals(circleEvent.Left.Point)) {
                leftHalfEdge = leftHalfEdge.Twin;
            }

            var centerHalfEdge = circleEvent.Center.PartialLine;

            if (centerHalfEdge.Face.Equals(circleEvent.Right.Point)) {
                centerHalfEdge = centerHalfEdge.Twin;
            }

            var rc = centerHalfEdge.Twin;
            rc.CircleEvent = circleEvent;
            leftHalfEdge.CircleEvent = circleEvent;

            circleEvent.PartialLine = centerHalfEdge;

            var prev = circleEvent.CenterLeaf.GetPreviousLeaf();
            var next = circleEvent.CenterLeaf.GetNextLeaf();

            if (prev?.DisappearEvent != null) {
                PriorityQueue.Delete(prev.DisappearEvent.Handle);
                prev.DisappearEvent = null;
            }

            if (next?.DisappearEvent != null) {
                PriorityQueue.Delete(next.DisappearEvent.Handle);
                next.DisappearEvent = null;
            }

            var newCircles = Events.RemoveNode(circleEvent, prev, circleEvent.CenterLeaf, next);
            if (newCircles == null) return;

            foreach (var ce in newCircles) {
                C5.IPriorityQueueHandle<IEvent> h = null;
                PriorityQueue.Add(ref h, ce);
                ce.Handle = h;
            }
        }

        private void FinalizeDiagram(IBinaryNode node) {
            switch (node) {
                case Leaf leafNode when leafNode.BreakpointNode == null:
                    return;
                case Leaf leafNode: {
                    var breakpoint = leafNode.BreakpointNode.Breakpoint;

                    var face = breakpoint.LeftListEvent.PartialLine.Face;
                    var twinFace = breakpoint.LeftListEvent.PartialLine.Twin.Face;

                    var centerX = 0.5f
                                  * (breakpoint.LeftListEvent.PartialLine.Face.X
                                     + breakpoint.LeftListEvent.PartialLine.Twin.Face.X);
                    var centerY = 0.5f
                                  * (breakpoint.LeftListEvent.PartialLine.Face.Y
                                     + breakpoint.LeftListEvent.PartialLine.Twin.Face.Y);

                    if (Math.Abs(face.Y - twinFace.Y) < EPSILON_DOUBLE) {
                        var halfEdge = breakpoint.LeftListEvent.PartialLine;
                        var circleEvent = new CircleEvent(centerX, -MaximumEdgeLimit);

                        if (halfEdge.CircleEvent == null) {
                            halfEdge.CircleEvent = circleEvent;
                            circleEvent.PartialLine = halfEdge;
                        } else {
                            halfEdge.Twin.CircleEvent = circleEvent;
                            circleEvent.PartialLine = halfEdge.Twin;
                        }
                    } else {
                        var grad = -(twinFace.X - face.X) / (twinFace.Y - face.Y);

                        var constant = centerY - grad * centerX;

                        var bpx = breakpoint.X - centerX;
                        var bpy = breakpoint.Y - centerY;

                        var testX = centerX + 10000;
                        var testY = testX * grad + constant;

                        if (testX * bpx + testY * bpy <= 0) {
                            testX = centerX - 10000;
                            testY = (centerX - 10000) * grad + constant;
                        }

                        var circleEvent = new CircleEvent(testX, testY);

                        var halfEdge = breakpoint.LeftListEvent.PartialLine;

                        if (!halfEdge.Face.Equals(breakpoint.LeftListEvent.Point)) {
                            halfEdge = halfEdge.Twin;
                        }

                        if (halfEdge.CircleEvent == null) {
                            halfEdge.CircleEvent = circleEvent;
                        } else if (halfEdge.Twin.CircleEvent == null) {
                            halfEdge.Twin.CircleEvent = circleEvent;
                        }
                    }

                    break;
                }
                case BreakpointNode breakpointNode: {
                    breakpointNode.Breakpoint.InitializeBreakpoint(MaximumEdgeLimit);

                    if (node.Left != null) {
                        FinalizeDiagram(node.Left);
                    }

                    if (node.Right != null) {
                        FinalizeDiagram(node.Right);
                    }

                    break;
                }
            }
        }
    }
}