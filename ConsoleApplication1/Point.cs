namespace ConsoleApplication1 {
    public class Point
    {
        public int ID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public Point(int id, double x, double y) {
            ID = id;
            X = x;
            Y = y;
        }
        public Point(int id)
        {
            ID = id;
        }
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
        public Point() { }

        public static Point operator +(Point p1, Point p2) => new Point(p1.X + p2.X, p1.Y + p2.Y);
        public static Point operator +(Point p1, double q) => new Point(p1.X + q, p1.Y + q);
        public static Point operator -(Point p) => new Point(-p.X, -p.Y);
        public static Point operator -(Point p1, Point p2) => p1 + (-p2);
        public static Point operator -(Point p1, double q) => p1 + (-q);
        public static Point operator *(Point p1, Point p2) => new Point(p1.X * p2.X, p1.Y * p2.Y);
        public static Point operator *(Point p1, double q) => new Point(p1.X * q, p1.Y * q);
        public static Point operator *(double q, Point p1) => p1 * q;
        public static Point operator /(Point p1, double q) => new Point(p1.X / q, p1.Y / q);
    }

    public class Vector : Point { }
    public class Node : Point { }
}
