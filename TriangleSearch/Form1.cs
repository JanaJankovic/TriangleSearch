using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TriangleSearch
{
    public partial class TriangleSearchForm : Form
    {
        private const int RADIUS = 300; // radius of circle
        private const int RADIUS_NODE = 5; // radius of nodes on circle
        private bool generate = false;  // boolean to trigger reshreshing of panel
        private List<Node> nodes = new List<Node>();
        private SortedList<double, Triangle> triangles = new SortedList<double, Triangle>();
        private Triangle selectedTriangle; // triangle that is rendered
        private bool selected = false; // change from default to triangle from list
        private int n; // number of nodes
        float x, y; // coordinates of cicrle center

        public TriangleSearchForm()
        {
            InitializeComponent();

            //position circle to the middle of panel
            x = this.panelGeometry.Width / 2;
            y = this.panelGeometry.Height / 2;
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            nodes.Clear();
            triangles.Clear();

            n = Convert.ToInt32(Math.Round(this.numberOfDots.Value, 0));
            
            // minimum number of nodes is 3
            if (n >= 3)
            {
                generate = true;
                selected = false;
                this.Refresh(); // refresh canvas
            } else
            {
                MessageBox.Show("Minimum is 3");
            }
             
        }

        private void panelGeometry_Paint(object sender, PaintEventArgs e)
        {
            if (generate)
            {
                Graphics graphics = e.Graphics;

                renderCircle(graphics);
                
                //for first initialization
                if (!selected)
                {
                    generateNodes(x, y); 
                    generateTriangles();
                    selectedTriangle = triangles.Values[triangles.Count - 1]; // get the biggest triangle from sorted list

                }

                renderNodes(graphics);
                renderTriangle(graphics);

            }
        }

        private void renderCircle(Graphics graphics)
        {
            Pen penCircle = new Pen(Color.Black);
            graphics.DrawEllipse(penCircle, x - RADIUS / 2, y - RADIUS / 2, RADIUS, RADIUS);
        }

        private void renderNodes(Graphics graphics)
        {
            
            Pen pen = new Pen(Color.Red);
            SolidBrush solidBrush = new SolidBrush(Color.Red);

            foreach (Node n in nodes)
            {
                graphics.DrawEllipse(pen, (float)(n.X - RADIUS_NODE / 2),
                    (float)(n.Y - RADIUS_NODE / 2), RADIUS_NODE, RADIUS_NODE);

                graphics.FillEllipse(solidBrush, (float)(n.X - RADIUS_NODE / 2),
                    (float)(n.Y - RADIUS_NODE / 2), RADIUS_NODE, RADIUS_NODE);
            }
        }

        private void renderTriangle(Graphics graphics)
        {

            Pen pen = new Pen(Color.Red);

            if ((float)selectedTriangle.Diameter < (float)triangles.Keys[triangles.Count - 1])
                pen = new Pen(Color.Blue);

            graphics.DrawLine(pen, (float)selectedTriangle.N1.X, (float)selectedTriangle.N1.Y,
                (float)selectedTriangle.N2.X, (float)selectedTriangle.N2.Y);
            graphics.DrawLine(pen, (float)selectedTriangle.N1.X, (float)selectedTriangle.N1.Y,
                (float)selectedTriangle.N3.X, (float)selectedTriangle.N3.Y);
            graphics.DrawLine(pen, (float)selectedTriangle.N2.X, (float)selectedTriangle.N2.Y,
                (float)selectedTriangle.N3.X, (float)selectedTriangle.N3.Y);
        }

        private void generateNodes(float x, float y)
        {
            Random random = new Random();
            for (int i = 0; i < n; i++)
            {
                Node node = new Node();
                //generate random angles
                node.Alpha = random.Next(0, 359);

                //calculate x and y based on angle using trigonometric functions
                node.X = Convert.ToDouble(x) + Math.Cos(node.Alpha) * RADIUS / 2;
                node.Y = Convert.ToDouble(y) + Math.Sin(node.Alpha) * RADIUS / 2;

                //add node to list of nodes
                nodes.Add(node);
            }
        }

        private void generateTriangles()
        {
            //go through all nodes and create triangles
            for(int i = 1; i < n; i++)
            {
                int j = i - 1;
                for (int k = 0; k < n; k++)
                {
                    if (k != i && k != j)
                    {
                        Node n1 = nodes[j];
                        Node n2 = nodes[i];
                        Node n3 = nodes[k];

                        //here it's checked whether nodes are non-collinear
                        if (Triangle.canFormTriangle(n1, n2, n3))
                        {
                            Triangle triangle = new Triangle() { N1 = n1, N2 = n2, N3 = n3 };
                            triangle.Diameter = Triangle.calculateDiameter(triangle);

                            //here it's checked whether there is already triangle with the same diameter
                            //only unique diameteres are saved
                            if (!triangles.ContainsKey(triangle.Diameter))
                                triangles.Add(triangle.Diameter, triangle);
                        }
                    }
                }
            }

            //updating listbox
            listItems();
        }

      
        private void listBoxData_SelectedIndexChanged(object sender, EventArgs e)
        {
            //get value from selected item
            string item = listBoxData.SelectedItems[0].ToString();
            string[] data = item.Split(new string[] { "   " }, StringSplitOptions.None);
            
            //recalculate x and y, based on angle

            Node n1 = new Node();
            n1.Alpha = Convert.ToInt32(data[1]);
            n1.X = Convert.ToDouble(x) + Math.Cos(n1.Alpha) * RADIUS / 2;
            n1.Y = Convert.ToDouble(y) + Math.Sin(n1.Alpha) * RADIUS / 2;

            Node n2 = new Node();
            n2.Alpha = Convert.ToInt32(data[2]);
            n2.X = Convert.ToDouble(x) + Math.Cos(n2.Alpha) * RADIUS / 2;
            n2.Y = Convert.ToDouble(y) + Math.Sin(n2.Alpha) * RADIUS / 2;

            Node n3 = new Node();
            n3.Alpha = Convert.ToInt32(data[3]);
            n3.X = Convert.ToDouble(x) + Math.Cos(n3.Alpha) * RADIUS / 2;
            n3.Y = Convert.ToDouble(y) + Math.Sin(n3.Alpha) * RADIUS / 2;

            //updated selected triangle

            selectedTriangle.Diameter = Convert.ToDouble(data[0]);
            selectedTriangle.N1 = n1;
            selectedTriangle.N2 = n2;
            selectedTriangle.N3 = n3;

            selected = true;

            //when selected triangle is changed, its required to refresh the panel
            this.Refresh();

        }

        private void listItems()
        {
            this.listBoxData.Items.Clear();
            //take diameter and angles(Alpha) and display them in a row
            for (int i = triangles.Count - 1; i >= 0; i--)
            {
                
                string row = triangles.Values[i].Diameter.ToString() + "   " + triangles.Values[i].N1.Alpha.ToString() + "   "
                    + triangles.Values[i].N2.Alpha.ToString() + "   " + triangles.Values[i].N3.Alpha.ToString();
                listBoxData.Items.Add(row);
                
            }
        }

    }
}
