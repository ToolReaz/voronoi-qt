using C5;

namespace Voronoi.Events
{
    public interface IEvent
    {
        /// <summary>
        /// The X-Coordinate of the center of the event
        /// </summary>
        double X { get; }

        /// <summary>
        /// The Y-Coordinate of the center of the event
        /// </summary>
        double Y { get; }

        /// <summary>
        /// The distance between the event occurence and the starting point of the line (it serves in the priority queue as the priority of the event)
        /// </summary>
        /// <returns></returns>
        double DistanceFromLine { get; }

        /// <summary>
        /// The handle to the element inside of the priority queue
        /// </summary>
        IPriorityQueueHandle<IEvent> Handle { get; set; }
    }
}