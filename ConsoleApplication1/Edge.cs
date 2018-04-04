using System.Collections.Generic;

namespace ConsoleApplication1
{
    public class Edge
    {
        public Node Node1 { get; set; }
        public Node Node2 { get; set; }
        public TriangleCell Cell1 { get; set; }
        public TriangleCell Cell2 { get; set; }
        public Vector Normal { get; set; }
        public double L { get; set; }
        public int CellsCount { get; set; }
        public List<Point> C { get; set; }
        public int Type { get; set; }
    }
}