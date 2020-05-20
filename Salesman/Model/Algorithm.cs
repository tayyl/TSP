using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesman.Model
{
    public abstract class Algorithm
    {
        public bool Empty { get => cities.Any(); }
        protected int delay;
        protected List<City> cities;
        protected int[,] neighbourMatrix;

        public Algorithm(int[,] matrix, List<City> cities, int delay = 200)
        {
            neighbourMatrix = (int[,])matrix.Clone();
            this.cities = new List<City>(cities);
            this.delay = delay;
        }
        public string CreatePath(ICollection<Edge> edges)
        {
            string path = "";
            path += "{ ";
            foreach (Edge edge in edges)
            {
                path += edge.City1.Number + " =>";
            }
            path += edges.First().City1.Number + " } ";
            return path;
        }
    }
}
