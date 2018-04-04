using System;
using System.IO;
using System.Threading;

namespace ConsoleApplication1
{
    public class Heat
    {
        public Cell[,] Cells { get; set; }
        public double A { get; set; }
        public double H { get; set; }
        public int CellCount => (int)(1 / H);

        public Heat(double a, double h)
        {
            A = a;
            H = h;
            Initialize();
        }

        public void Run(double tMax, double tau)
        {
            for (var k = .0; k < tMax; k += tau)
            {
                for (var i = 1; i < CellCount - 1; i++)
                {
                    for (var j = 1; j < CellCount - 1; j++)
                    {
                        Cells[i, j].S = Cells[i, j].SOld + (Math.Pow(A, 2) * (Cells[i, j - 1].S + Cells[i, j + 1].S - 2 * Cells[i, j].S) / Math.Pow(H, 2)
                            + Math.Pow(A, 2) * (Cells[i - 1, j].S + Cells[i + 1, j].S - 2 * Cells[i, j].S) / Math.Pow(H, 2)) * tau;
                        Cells[i, j].SOld = Cells[i, j].S;
                    }
                }
                SaveToFile("test.vts");
            }
        }

        public void SaveToFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            using (var fs = new StreamWriter(File.Open(path, FileMode.OpenOrCreate)))
            {
                fs.WriteLine("<?xml version=\"1.0\"?>");
                fs.WriteLine("<VTKFile type=\"StructuredGrid\" version=\"0.1\" byte_order=\"LittleEndian\">");
                fs.WriteLine($"  <StructuredGrid WholeExtent=\"0 {CellCount} 0 {CellCount} 0 0\">");
                fs.WriteLine($"    <Piece Extent=\"0 {CellCount} 0 {CellCount} 0 0\">");
                fs.WriteLine("      <Points>");
                fs.WriteLine("        <DataArray type=\"Float32\" NumberOfComponents=\"3\" format=\"ascii\">");
                fs.WriteLine("          ");
                for (int j = 0; j <= CellCount; j++)
                {
                    for (int i = 0; i <= CellCount; i++)
                    {
                        fs.Write($"{Format(i * H)} {Format(j * H)} 0.0 ");
                    }
                }
                fs.WriteLine();
                fs.WriteLine("        </DataArray>");
                fs.WriteLine("      </Points>");
                fs.WriteLine("      <CellData Scalars=\"Temperature, Proc\">");
                fs.WriteLine("        <DataArray type=\"Float32\" Name=\"Temperature\" format=\"ascii\">");
                fs.WriteLine("          ");
                for (int j = 0; j < CellCount; j++)
                {
                    for (int i = 0; i < CellCount; i++)
                    {
                        fs.Write($"{ Format(Math.Round(Cells[i, j].S, 2)) } ");
                    }
                    fs.WriteLine();
                }
                fs.WriteLine("        </DataArray>");
                fs.WriteLine("      </CellData>");
                fs.WriteLine("    </Piece>");
                fs.WriteLine("  </StructuredGrid>");
                fs.WriteLine("</VTKFile>");
            }
        }

        public void ReadNodeFile(string path)
        {
            if (File.Exists(path))
            {
                using (var fp = new StreamReader(File.Open(path, FileMode.Open)))
                {
                    fp.ReadLine
                    nodes = new Point
                }
            }
        }
        public override string ToString()
        {
            var result = string.Empty;
            for (var i = 0; i < CellCount; i++)
            {
                for (var j = 0; j < CellCount; j++)
                {
                    result += $"\t{ Math.Round(Cells[i, j].S, 4) }";
                }
                result += "\n";
            }
            return result;
        }

        private void Output()
        {
            Console.Clear();
            Console.WriteLine(this);
            Thread.Sleep(30);
        }

        private string Format(double t) =>
            t.ToString().Replace(",", ".");

        private void Initialize()
        {
            Cells = new Cell[CellCount, CellCount];
            for (var i = 0; i < CellCount; i++)
            {
                for (var j = 0; j < CellCount; j++)
                {
                    Cells[i, j] = new Cell(j == 0 ? 1 : 0);
                }
            }
        }
    }
}
