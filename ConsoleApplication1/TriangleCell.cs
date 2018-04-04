using System.Collections.Generic;

namespace ConsoleApplication1 {
    public class TriangleCell {
        public double S { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }
        public int NCount { get; set; }
        public int ECount { get; set; }
        public List<TriangleCell> Neighbours { get; set; } = new List<TriangleCell>(3);
        public Point C { get; set; }
        public Vector H { get; set; }
        public List<Node> Node { get; set; }
        public List<Edge> Edge { get; set; }
        public uint Flag { get; set; }
        public TriangleCell() { }
        public Param Param { get; set; }
        public TriangleCell(double s) {
            S = s;
        }
    }
}
