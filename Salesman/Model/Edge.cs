using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesman.Model
{
    public class Edge
    {

        public int X1 { get; private set; }
        public int Y1 { get; private set; }
        public int X2 { get; private set; }
        public int Y2 { get; private set; }
        public float CenterX
        {
            get
            {
                return (X1 + X2) / 2-7;
            }
        }
        public float CenterY
        {
            get
            {
                return (Y1 + Y2) / 2-9;
            }
        }
        public int Weight { get; private set; }
        public Edge(int x1, int y1,int x2, int y2, int weight)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            Weight = weight;
        }
    }
}
