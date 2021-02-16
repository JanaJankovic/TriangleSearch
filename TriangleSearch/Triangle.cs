using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriangleSearch
{
    struct Triangle
    {
        public Node N1 { get; set; }
        public Node N2 { get; set; }
        public Node N3 { get; set; }
        public double Diameter { get; set; }

        public static double calculateDiameter(Triangle triangle)
        {
            double a = Node.getDistance(triangle.N1, triangle.N2);
            double b = Node.getDistance(triangle.N1, triangle.N3);
            double c = Node.getDistance(triangle.N2, triangle.N3);
            return a + b + c;
        }

        public static bool canFormTriangle(Node n1, Node n2, Node n3)
        {
            return n1.Alpha != n2.Alpha && n1.Alpha != n3.Alpha && n2.Alpha != n3.Alpha;
        }
    }
}
