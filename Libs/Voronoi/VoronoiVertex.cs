using System;
using System.Collections.Generic;

using static Voronoi.COMPILATION_CONSTANTS;

namespace Voronoi
{
    public class VoronoiVertex : Vertex
    {
        public VoronoiVertex(double xVal, double yVal) : base(xVal, yVal) { }

        public PartialLine PartialLine { get; set; }

        public override bool Equals(object other) {
            return other is VoronoiVertex node && Equals(node);
        }

        public bool Equals(VoronoiVertex other) {
            return Math.Abs(X - other.X) < EPSILON_DOUBLE && Math.Abs(Y - other.Y) < EPSILON_DOUBLE;
        }

        public override int GetHashCode() {
            return (X, Y).GetHashCode();
        }

        public IEnumerable<PartialLine> GetPartialLines() {
            var pl = PartialLine;

            while (pl != null) {
                yield return pl;

                pl = pl.Next;
            }
        }
    }
}