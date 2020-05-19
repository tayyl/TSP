using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesman.Model
{
    public class Annealing : Algorithm
    {
        List<City> nextCities;
        Random rnd=new Random();
        public Annealing(int[,] matrix, List<City> cities, int delay = 200) 
            : base(matrix, cities, delay)
        {
        }
        public void GetRandomInstance(Random random)
        {
            rnd = random;
        }
        public Tuple<int,string> TSP(ICollection<City> VisitedCities, ICollection<Edge> CurrentBestEdges, ICollection<Edge> FinalEdges)
        {
            int iteration = -1;
            //the probability
            double proba;
            double alpha = 0.999;
            double temperature = 400.0;
            double epsilon = 0.01;
            int delta;
            int distance = ComputeDistance(cities);
            nextCities = new List<City>();
            while (temperature > epsilon)
            {
                iteration++;

                Permutation(cities, out nextCities);
                delta = ComputeDistance(nextCities) - distance;
                if (delta < 0)
                {
                    cities = new List<City>(nextCities);

                    distance = delta + distance;
                }
                else
                {
                    proba = rnd.NextDouble();
                    if (proba < Math.Exp(-delta / temperature))
                    {
                        cities = new List<City>(nextCities);
                        distance = delta + distance;
                    }
                }
                temperature *= alpha;
                if (iteration % 100 == 0)
                {
                    System.Threading.Thread.Sleep(delay);
                    CurrentBestEdges.Clear();
                    for(int i=0; i<cities.Count-1;i++)
                        CurrentBestEdges.Add(new Edge(cities[i],cities[i+1],neighbourMatrix[cities[i].Number,cities[i+1].Number]));

                    App.Current.Dispatcher.BeginInvoke((Action)delegate
                    {
                        CurrentBestEdges.Add(new Edge(cities[0],  cities[cities.Count - 1], neighbourMatrix[cities[0].Number, cities[cities.Count - 1].Number]));
                    });
                }
            }
            App.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                for(int i=0; i<cities.Count-1; i++)
                    FinalEdges.Add(new Edge(cities[i],cities[i+1],neighbourMatrix[cities[i].Number,cities[i+1].Number]));
                FinalEdges.Add(new Edge(cities.Last(),cities.First(),neighbourMatrix[cities.First().Number,cities.Last().Number]));
                CurrentBestEdges.Clear();
                foreach (City city in cities)
                    VisitedCities.Add(city);
            });
            System.Threading.Thread.Sleep(delay);
            return new Tuple<int, string>(distance, CreatePath(FinalEdges));
        }
        int ComputeDistance(List<City> cities)
        {
            int distance = 0;
            for(int i=0; i<cities.Count-1; i++)
            {
                distance += neighbourMatrix[cities[i].Number, cities[i + 1].Number];
            }
            distance += neighbourMatrix[cities[0].Number, cities[cities.Count-1].Number];

            return distance;
        }
        void Permutation(List<City> currentCities, out List<City> nextCities)
        {
            nextCities = new List<City>(currentCities);
            int i1 = (int)(rnd.Next(currentCities.Count));
            int i2 = (int)(rnd.Next(currentCities.Count));
            City aux = nextCities[i1];
            nextCities[i1] = nextCities[i2];
            nextCities[i2] = aux;

        }
    }
}
