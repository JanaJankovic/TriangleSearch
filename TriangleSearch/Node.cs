using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriangleSearch
{
    struct Node
    {
        public double X { get; set; }
        public double Y { get; set; }
        public int Alpha { get; set; }

        public static double getDistance(Node n1, Node n2)
        {
            double xDistance = (n1.X - n2.X) * (n1.X - n2.X);
            double yDistance = (n1.Y - n2.Y) * (n1.Y - n2.Y);
            return Math.Sqrt(Convert.ToDouble(xDistance + yDistance));
        }

        public static bool compareNodes(Node n1, Node n2)
        {
            return n1.Alpha == n2.Alpha;
        }
    }
}
