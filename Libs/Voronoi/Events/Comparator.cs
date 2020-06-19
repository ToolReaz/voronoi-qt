using System;
using System.Collections.Generic;

using Voronoi.Events;

namespace Voronoi.Events
{
    /// <summary>
    /// Comparer for IEvents. 
    /// </summary>
    public class Comparator : IComparer<IEvent>
    {
        public int Compare(IEvent o1, IEvent o2) {
            if (o1 is null) throw new ArgumentNullException(nameof(o1));
            if (o2 is null) throw new ArgumentNullException(nameof(o2));

            if (o1.DistanceFromLine < o2.DistanceFromLine) return 1;
            if (o1.DistanceFromLine > o2.DistanceFromLine) return -1;
            if (o1.X < o2.X) return -1;
            if (o1.X > o2.X) return 1;

            return 0;
        }
    }
}