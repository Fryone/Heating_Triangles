using System;

namespace ConsoleApplication1 {
    class Program {
        static void Main(string[] args) {
            HardHeat();
        }

        private static void SimpleHeat() {
            var a = double.Parse(Console.ReadLine());
            var h = double.Parse(Console.ReadLine());
            var heat = new SimpleHeat(a, h);
            Console.WriteLine("begin...");
            heat.Run(1, 1.5 * Math.Pow(h, 2));
            Console.WriteLine("end");
            Console.ReadKey();
        }

        private static void HardHeat() {
            //var a = double.Parse(Console.ReadLine());
            Console.WriteLine("start...");
            var tau = double.Parse(Console.ReadLine());
            Console.WriteLine("1...");
            var mesh = new HardHeat();
            Console.WriteLine("begin...");
            mesh.Run(1, tau);
            Console.WriteLine("end");
            Console.ReadKey();
        }
    }
}
