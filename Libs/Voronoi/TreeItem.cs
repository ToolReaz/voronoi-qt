namespace Voronoi
{
    public class TreeItem
    {
        public TreeItem(VoronoiVertex node) {
            Point = node;
        }

        public VoronoiVertex Point { get; }

        public TreeItem Copy() => new TreeItem(Point) {PartialLine = PartialLine};

        public PartialLine PartialLine { get; set; }
    }
}