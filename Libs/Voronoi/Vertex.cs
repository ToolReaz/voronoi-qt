namespace Voronoi
{
    public class Vertex
    {
        public double X { get; }
        public double Y { get; }

        public Vertex(double x, double y) {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"X={X}; Y={Y}";
        }
    }
}
