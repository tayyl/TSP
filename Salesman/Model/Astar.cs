using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesman.Model
{
    public class DuplicateKeyComparer<TKey>
                   :
                IComparer<TKey> where TKey : IComparable
    {
        #region IComparer<TKey> Members

        public int Compare(TKey x, TKey y)
        {
            int result = x.CompareTo(y);

            if (result == 0)
                return 1;   // Handle equality as beeing greater
            else
                return result;
        }

        #endregion
    }

    public class Astar : Algorithm
    {
        public Astar(int[,] matrix, List<City> cities, int delay = 200) : base(matrix, cities, delay)
        {
        }

        public override int TSP(ObservableCollection<City> OpenCities, ObservableCollection<Edge> CurrentEdges, ObservableCollection<Edge> CurrentBestEdge, ObservableCollection<Edge> CurrentFinalEdges, ObservableCollection<Edge> FinalEdges)
        {
            int bestDistance = 0;
            int citiesAmount = cities.Count;
            City startingCity = cities.First();
            SortedList<double, List<City>> paths = new SortedList<double, List<City>>(new DuplicateKeyComparer<double>());
            
            paths.Add(0, new List<City>() { startingCity });
            do
            {
                List<City> currentBestDistancePath = paths.First().Value;
                paths.RemoveAt(0);

                for (int i=0; i<citiesAmount; i++)
                {
                    if (!currentBestDistancePath.Contains(cities[i]))
                    {
                        double F = pathDistance(currentBestDistancePath) + heuristic(startingCity, cities[i]);
                        paths.Add(F, new List<City>(currentBestDistancePath) { cities[i]});
                    }
                }
                if (paths.First().Value.Count == citiesAmount)
                {
                    double F = pathDistance(paths.First().Value) + heuristic(paths.First().Value.Last(), startingCity);
                    paths.Add(F, new List<City>(paths.First().Value) { startingCity });

                    paths.RemoveAt(0);
                }
            } while (paths.First().Value.Count!=citiesAmount+1);
            paths.First().Value.RemoveAt(citiesAmount);
            bestDistance = pathDistance(paths.First().Value);
            return bestDistance;
        }
        double heuristic(City city1, City city2)
        {
            return Math.Sqrt(Math.Pow((city1.X-city2.X),2) 
                 + Math.Pow((city1.Y - city2.Y),2));
        }
        int pathDistance(List<City> cities)
        {            
            int dist = 0;
            for(int i=0; i<cities.Count-1; i++)
                dist += neighbourMatrix[cities[i].Number, cities[i + 1].Number];

            return dist;
        }
    }
}
