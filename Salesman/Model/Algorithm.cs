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
        public bool Empty { get => cities.Any();}
        protected List<City> cities;
        protected List<Edge> edges;
        protected int[,] neighbourMatrix;
        public abstract int TSP(ObservableCollection<City> VisitedCities, ObservableCollection<Edge> CurrentEdges, ObservableCollection<Edge> FinalEdges);
      
    }
}
