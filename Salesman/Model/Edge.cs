using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesman.Model
{
    public class Edge
    {
        public City city1 { get; private set; }
        public City city2 { get; private set; }

        public float CenterX
        {
            get
            {
                return (city1.X + city2.X) / 2-7;
            }
        }
        public float CenterY
        {
            get
            {
                return (city1.Y + city2.Y) / 2-9;
            }
        }
        public int Weight { get; private set; }
        public Edge(City city1, City city2, int weight)
        {
            this.city1 = new City(city1.X, city1.Y, city1.Number);
            this.city2 = new City(city2.X, city2.Y, city2.Number);
            Weight = weight;
        }
    }
}
