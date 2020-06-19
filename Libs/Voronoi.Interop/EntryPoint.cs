using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Voronoi.Interop
{
    public class EntryPoint
    {
        public static int GetVoronoi(string mmapfile)
        {
            try {
                //Console.WriteLine($"INITIALIZED: {mmapfile}");
                var mmap = MemoryMappedFile.CreateOrOpen(mmapfile, 1024 * 1024 * 4 * sizeof(double));
                var accessor = mmap.CreateViewAccessor();
                var dataLength = accessor.ReadInt32(0);
                var dArray = new Point[dataLength];
                accessor.ReadArray(sizeof(int), dArray, 0, dataLength);

                var lines = GetVoronoi(dArray);
                accessor.Write(0, lines.Length);
                accessor.WriteArray(sizeof(int), lines, 0, lines.Length);
                
            } catch(Exception ex) {
                Console.WriteLine(ex.ToString());
            }
            return 0;
        }

        public static Line[] GetVoronoi(Point[] points) {

            var pts = points.Select(p => new Vertex(p.X, p.Y)).ToArray();

            var he = Builder.GetVoronoi(pts);
            return he.Select(half => new Line(half.CircleEvent, half.Twin.CircleEvent)).ToArray();
        }
    }
}
