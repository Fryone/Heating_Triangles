using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApplication1 {
    public class Mesh {
        public List<Node> Nodes;
        public List<TriangleCell> Cells;
        public List<Edge> Edges;
        public Mesh() {
            InitFromFile("carman.1");
        }

        public void InitFromFile(string fileName) {
            Nodes = ReadNodeFromFile(fileName + ".node");
            Cells = ReadEleFromFile(fileName + ".ele", Nodes);
            Cells.ForEach(x => x.Param = new Param { T = 0 });
            Cells = InitNeighFromFile(fileName + ".neigh", Cells);
            Cells = InitEdges(fileName + ".edge", Cells, Nodes, out Edges);
            Console.WriteLine("1...");
        }

        private List<Node> ReadNodeFromFile(string path) {
            var result = new List<Node>();
            using (var fs = new StreamReader(File.Open(path, FileMode.Open))) {
                result = fs.ReadToEnd()
                    .Split('\n')
                    .Skip(1)
                    .Where(x => !(string.IsNullOrWhiteSpace(x) || x[0] == '#'))
                    .Select(x => new Node {
                        X = double.Parse(x.Replace('.', ',').Split(' ')[1]),
                        Y = double.Parse(x.Replace('.', ',').Split(' ')[2])
                    })
                    .ToList();
            }
            return result;
        }

        private List<TriangleCell> ReadEleFromFile(string path, List<Node> nodes) {
            if (!File.Exists(path)) {
                throw new FileNotFoundException("Error_ele_path");
            }
            var result = new List<TriangleCell>();
            Func<Point, Point> abs = (Point p) => new Point(Math.Abs(p.X), Math.Abs(p.Y));
            using (var fs = new StreamReader(File.Open(path, FileMode.Open))) {
                result = fs.ReadToEnd()
                    .Split('\n')
                    .Skip(1)
                    .Where(x => !(string.IsNullOrWhiteSpace(x) || x[0] == '#'))
                    .Select(x => {
                        var nums = x
                            .Split(' ')
                            .Where(y => !string.IsNullOrWhiteSpace(y))
                            .Select(s => int.Parse(s))
                            .ToArray();
                        var cell = new TriangleCell {
                            NCount = 3,
                            ECount = 3,
                            Node = new List<Node>
                            {
                            nodes[nums[1] - 1],
                            nodes[nums[2] - 1],
                            nodes[nums[3] - 1]
                            },
                            Type = nums[4]
                        };
                        cell.C = (cell.Node[0] + cell.Node[1] + cell.Node[2]) / 3;
                        cell.H = new Vector {
                            X = new[] {
                            Math.Abs(cell.Node[0].X - cell.Node[1].X),
                            Math.Abs(cell.Node[1].X - cell.Node[2].X),
                            Math.Abs(cell.Node[0].X - cell.Node[2].X)
                            }.Max(),
                            Y = new[] {
                            Math.Abs(cell.Node[0].Y - cell.Node[1].Y),
                            Math.Abs(cell.Node[1].Y - cell.Node[2].Y),
                            Math.Abs(cell.Node[0].Y - cell.Node[2].Y)
                            }.Max()
                        };
                        return cell;
                    })
                    .ToList();
            }
            return result;
        }

        private List<TriangleCell> InitNeighFromFile(string path, List<TriangleCell> cells) {
            var result = new List<TriangleCell>();
            using (var fs = new StreamReader(File.Open(path, FileMode.Open))) {
                result = fs.ReadToEnd()
                    .Split('\n')
                    .Skip(1)
                    .Where(x => !(string.IsNullOrWhiteSpace(x) || x[0] == '#'))
                    .Select(x => {
                        var nums = x
                            .Split(' ')
                            .Where(y => !string.IsNullOrWhiteSpace(y))
                            .Select(s => int.Parse(s))
                            .ToArray();
                        cells[nums[0] - 1].Neighbours = new List<TriangleCell> {
                            nums[1] > 0 ? cells[nums[1] - 1] : null,
                            nums[2] > 0 ? cells[nums[2] - 1] : null,
                            nums[3] > 0 ? cells[nums[3] - 1] : null
                        };
                        return cells[nums[0] - 1];
                    })
                    .ToList();
            }
            return result;
        }

        private List<TriangleCell> InitEdges(string path, List<TriangleCell> cells, List<Node> nodes, out List<Edge> initializedEdges)
        {
            var result = new List<TriangleCell>();
            var edges = new List<Edge>();
            using (var fs = new StreamReader(File.Open(path, FileMode.Open))) {
                result = fs.ReadToEnd()
                    .Split('\n')
                    .Skip(1)
                    .Where(x => !(string.IsNullOrWhiteSpace(x) || x[0] == '#'))
                    .SelectMany(x => {
                        var nums = x
                            .Split(' ')
                            .Where(y => !string.IsNullOrWhiteSpace(y))
                            .Select(s => int.Parse(s))
                            .ToArray();
                        var edge = new Edge() {
                            Node1 = nodes[nums[1] - 1],
                            Node2 = nodes[nums[2] - 1],
                            Type = nums[3]
                        };
                        var newCells = cells
                            .Where(r => r.Node.Contains(edge.Node1) && r.Node.Contains(edge.Node2))
                            .ToList();
                        edge.Cell1 = newCells[0];
                        edge.Cell2 = newCells.Count > 1 ? newCells[1] : null;
                        var sqrt = 1 / Math.Sqrt(3);
                        edge.C = new List<Point>
                        {
                        (edge.Node1 + edge.Node2) * 0.5,
                        (edge.Node1 + edge.Node2) * 0.5 + 0.5 * sqrt * (edge.Node2 - edge.Node1),
                        (edge.Node1 + edge.Node2) * 0.5 + 0.5 * sqrt * (edge.Node2 - edge.Node1)
                        };
                        edge.Normal = new Vector { X = edge.Node2.Y - edge.Node1.Y, Y = edge.Node1.X - edge.Node2.X };
                        edge.L = Math.Sqrt(Math.Pow(edge.Normal.X, 2) + Math.Pow(edge.Normal.Y, 2));
                        edge.Normal = (edge.Normal / edge.L) as Vector;
                        edges.Add(edge);
                        newCells.ForEach(r => {
                            if (r.Edge == null) {
                                r.Edge = new List<Edge>();
                            }
                            r.Edge.Add(edge);
                        });
                        return newCells;
                    })
                    .ToList();
            }
            result.ForEach(x => {
                var a = x.Edge[0].L;
                var b = x.Edge[1].L;
                var c = x.Edge[2].L;
                var p = (a + b + c) / 2.0;
                x.S = Math.Sqrt(p * (p - a) * (p - b) * (p - c));
            });
            initializedEdges = edges;
            return result;
        }
    }
}
