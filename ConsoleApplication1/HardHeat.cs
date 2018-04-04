using System;
using System.IO;
using System.Linq;

namespace ConsoleApplication1 {
    public class HardHeat {
        public Mesh Mesh { get; set; }
        public HardHeat()
        {
            Mesh = new Mesh();
            //Mesh.Cells.ForEach(x => x.Param = new Param { T = 0 }); //???
            //SaveToVTK($"test{0}.vtk");
        }

        public void Run(double tMax, double tau) {
            var t = 0.0;
            var step = 0;
            var path = "";
            var ccc = Mesh.Edges.Where(x => x.Cell1?.S == 0 || x.Cell2?.S == 0).ToList();
            while (t < tMax) {
                t += tau;
                step++;
                Console.WriteLine($"step={step}");
                for (int ie = 0; ie < Mesh.Edges.Count; ie++) {
                    var edge = Mesh.Edges[ie];
                    var T1 = edge.Cell1.Param.T;
                    var T2 = 0.0;
                    var hij = 0.0;
                    if (edge.Cell2 != null) {
                        T2 = edge.Cell2.Param.T;
                        var v2 = edge.Cell2.C;
                        v2 -= edge.Cell1.C;
                        hij = Math.Sqrt(v2.X * v2.X + v2.Y * v2.Y);
                    }
                    else {
                        switch (edge.Type) {
                            case 1:
                                T2 = 0.0;
                                break;
                            case 2:
                                T2 = 1.0;
                                break;
                        }
                        var v2 = edge.C.FirstOrDefault();
                        v2 -= edge.Cell1.C;
                        hij = 2.0 * Math.Sqrt(v2.X * v2.X + v2.Y * v2.Y);
                    }
                    var flux = (T2 - T1) * edge.L / hij;
                    edge.Cell1.Param.T += (flux * tau / edge.Cell1.S);
                    //intT[c1] += flux;
                    if (edge.Cell2 != null) {
                        edge.Cell2.Param.T -= flux;
                    }
                }

                //for (int i = 0; i < Mesh.Nodes.Count; i++) {
                //    Mesh.Cells[i].Param.T = 
                //0}

                if (step % 1 == 0) {
                    path = $"test{step}.vtk";
                    SaveToVTK(path);
                }
            }


        }

        public void SaveToVTK(string path) {
            if (File.Exists(path)) {
                File.Delete(path);
            }
            using (var fs = new StreamWriter(File.Open(path, FileMode.OpenOrCreate))) {
                fs.WriteLine("# vtk DataFile Version 2.0");
                fs.WriteLine("GASDIN data file");
                fs.WriteLine("ASCII");
                fs.WriteLine("DATASET UNSTRUCTURED_GRID");
                fs.WriteLine($"POINTS { Mesh.Nodes.Count } float");

                for (int i = 0; i < Mesh.Nodes.Count; i++) {
                    fs.Write($"{ Mesh.Nodes[i].X } { Mesh.Nodes[i].Y } 0.0 ");
                    if (i + 1 % 8 == 0) {
                        fs.WriteLine();
                    }
                }
                fs.WriteLine();
                fs.WriteLine($"CELLS { Mesh.Cells.Count } { 4 * Mesh.Cells.Count }");
                var last = "";
                for (int i = 0; i < Mesh.Cells.Count; i++) {
                    if (last != $"3 { Mesh.Nodes.IndexOf(Mesh.Cells[i].Node[0]) } { Mesh.Nodes.IndexOf(Mesh.Cells[i].Node[1]) } { Mesh.Nodes.IndexOf(Mesh.Cells[i].Node[2]) }") {
                        last = $"3 { Mesh.Nodes.IndexOf(Mesh.Cells[i].Node[0]) } { Mesh.Nodes.IndexOf(Mesh.Cells[i].Node[1]) } { Mesh.Nodes.IndexOf(Mesh.Cells[i].Node[2]) }";
                        fs.WriteLine(last);
                    }
                }
                fs.WriteLine();
                fs.WriteLine($"CELL_TYPES { Mesh.Cells.Count }");
                for (int i = 0; i < Mesh.Cells.Count; i++) {
                    fs.WriteLine("5");
                }
                fs.WriteLine();
                fs.WriteLine($"CELL_DATA { Mesh.Cells.Count }");
                /*fs.WriteLine($"SCALARS Density float 1");
                fs.WriteLine($"LOOKUP_TABLE default");
                for (int i = 0; i < Mesh.Cells.Length; i++) {
                    Param p;
                    convertToParam(i, p);
                    fprintf("%25.16f ", p.r);
                    if (i + 1 % 8 == 0 || i + 1 == Mesh.Cells.Length) fs.WriteLine();
                }
                fs.WriteLine("SCALARS Pressure float 1");
                fs.WriteLine("LOOKUP_TABLE default");
                for (int i = 0; i < Mesh.Cells.Length; i++) {
                    Param p;
                    convertToParam(i, p);
                    fprintf(fp, "%25.16f ", p.p);
                    if (i + 1 % 8 == 0 || i + 1 == Mesh.Cells.Length) fs.WriteLine();
                }*/
                fs.WriteLine("SCALARS Temperature float 1");
                fs.WriteLine("LOOKUP_TABLE default");
                for (int i = 0; i < Mesh.Cells.Count; i++) {
                    fs.Write($"{ Mesh.Cells[i].Param.T }");
                    if ((i + 1) % 8 == 0 || (i + 1) == Mesh.Cells.Count) {
                        fs.WriteLine();
                    }
                }
                /*fs.WriteLine("VECTORS Velosity float");
                for (int i = 0; i < Mesh.Cells.Length; i++) {
                  Param p;
                  convertToParam(i, p);
                  fs.WriteLine(fp, "%25.16f %25.16f %25.16f ", p.u, p.v, 0.0);
                  if (i + 1 % 8 == 0 || i + 1 == Mesh.Cells.Length) fs.WriteLine();
                }*/
            }
        }
    }
}
