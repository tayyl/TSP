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
        public override int TSP(ObservableCollection<City> VisitedCities, ObservableCollection<Edge> CurrentEdges, ObservableCollection<Edge> CurrentBestEdges, ObservableCollection<Edge> FinalEdges)
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

                //get the next random permutation of distances 
                Permutation(cities, out nextCities);
                //compute the distance of the new permuted configuration
                delta = ComputeDistance(nextCities) - distance;
                //if the new distance is better accept it and assign it
                if (delta < 0)
                {
                    cities = new List<City>(nextCities);

                    distance = delta + distance;
                }
                else
                {
                    proba = rnd.NextDouble();
                    //if the new distance is worse accept 
                    //it but with a probability level
                    //if the probability is less than 
                    //E to the power -delta/temperature.
                    //otherwise the old value is kept
                    if (proba < Math.Exp(-delta / temperature))
                    {
                        cities = new List<City>(nextCities);
                        distance = delta + distance;
                    }
                }
                //cooling process on every iteration
                temperature *= alpha;
                //print every 400 iterations
                if (iteration % 100 == 0)
                {
                    System.Threading.Thread.Sleep(delay);
                    CurrentBestEdges.Clear();
                    for(int i=0; i<cities.Count-1;i++)
                        CurrentBestEdges.Add(new Edge(cities[i].X,cities[i].Y,cities[i+1].X,cities[i+1].Y,neighbourMatrix[cities[i].Number,cities[i+1].Number]));

                    App.Current.Dispatcher.BeginInvoke((Action)delegate
                    {
                        CurrentBestEdges.Add(new Edge(cities[0].X, cities[0].Y, cities[cities.Count - 1].X, cities[cities.Count - 1].Y, neighbourMatrix[cities[0].Number, cities[cities.Count - 1].Number]));
                    });
                }
            }
            App.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                foreach (Edge edge in CurrentBestEdges)
                    FinalEdges.Add(edge);
                CurrentBestEdges.Clear();
            });
        
            return distance;
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
