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
        public abstract int TSP(ObservableCollection<City> VisitedCities, ObservableCollection<Edge> CurrentEdges, ObservableCollection<Edge> CurrentBestEdge, ObservableCollection<Edge> FinalEdges);
      
    }
}
