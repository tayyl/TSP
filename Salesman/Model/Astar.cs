﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesman.Model
{
    public class Astar : Algorithm
    {
        public Astar(int[,] matrix, List<City> cities, int delay = 200) : base(matrix, cities, delay)
        {
        }

        public override int TSP(ObservableCollection<City> VisitedCities, ObservableCollection<Edge> CurrentEdges, ObservableCollection<Edge> CurrentBestEdge, ObservableCollection<Edge> FinalEdges)
        {
            throw new NotImplementedException();
        }
    }
}
