using Voronoi.Events;

namespace Voronoi
{
    /// <summary>
    /// This represents a linked list of half lines in the voronoi diagram along with the circle event which was responsible for the creation of the <see cref="Face"/>
    /// </summary>
    public class PartialLine
    {
        public PartialLine(Vertex face) {
            Face = face;
        }

        public Vertex Face { get; set; }

        public PartialLine Twin { get; set; }

        public PartialLine Next { get; set; }

        public PartialLine Previous { get; set; }

        public CircleEvent CircleEvent { get; set; }
    }
}