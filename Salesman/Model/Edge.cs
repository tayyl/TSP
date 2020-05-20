using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesman.Model
{
    public class Edge
    {
        public City City1 { get; private set; }
        public City City2 { get; private set; }

        public float CenterX
        {
            get
            {
                return (City1.X + City2.X) / 2-7;
            }
        }
        public float CenterY
        {
            get
            {
                return (City1.Y + City2.Y) / 2-9;
            }
        }
        public int Weight { get; private set; }
        public Edge(City city1, City city2, int weight)
        {
            this.City1 = new City(city1.X, city1.Y, city1.Number);
            this.City2 = new City(city2.X, city2.Y, city2.Number);
            Weight = weight;
        }
    }
}
