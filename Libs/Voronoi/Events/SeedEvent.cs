using C5;

namespace Voronoi.Events
{
    /// <summary>
    /// This event indicates that a new vector node will intersect with the sweep line
    /// </summary>
    public class SeedEvent : IEvent
    {
        public SeedEvent(Vertex vector) {
            Vertex = new VoronoiVertex(vector.X, vector.Y);
        }

        /// <summary>
        /// The vector node (point) intersecting with the sweep line
        /// </summary>
        public VoronoiVertex Vertex { get; }

        /// <summary>
        /// X-Coordinate of the vector node
        /// </summary>
        public double X => Vertex.X;

        /// <summary>
        /// Y-coordinate of the vector node
        /// </summary>
        public double Y => Vertex.Y;

        public double DistanceFromLine => Y;
        

        /// <summary>
        /// The handle in the priority queue
        /// </summary>
        public IPriorityQueueHandle<IEvent> Handle { get; set; }

        public override string ToString() {
            return $"Site Event ({X};{Y})";
        }
    }
}