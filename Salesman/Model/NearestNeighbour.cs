﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salesman.Model
{
    public class NearestNeighbour : Algorithm
    {
        public NearestNeighbour(int[,] matrix, List<City> cities, int delay = 200)
            : base(matrix, cities, delay)
        {
        }

        //Every Thread Sleep is for having good animation of every step in normal algorithm it obviously can be avoided
        //every Current.Dispatcher.BeginInvoke is for asynchronously update Lists, it's here also for the purpose of good visual representation
        public Tuple<int,string> TSP(ICollection<City> VisitedCities, ICollection<Edge> CurrentEdges, ICollection<Edge> CurrentBestEdge, ICollection<Edge> CurrentFinalEdges, ICollection<Edge> FinalEdges)
        {
            int bestDistance = 100000000, currentBestDistance = 0;
            List<City> allCities = new List<City>(cities);
            foreach (City startingCity in allCities)
            {
                VisitedCities.Clear();
                CurrentBestEdge.Clear();
                CurrentEdges.Clear();
                cities = new List<City>(allCities);
                City currentCity = startingCity;
                cities.Remove(startingCity);
                App.Current.Dispatcher.BeginInvoke((Action)delegate
                {
                    VisitedCities.Add(currentCity);
                });
                int minWeight = int.MaxValue;
                City tmp = currentCity;
                do
                {
                    foreach (City city in cities)
                    {
                        if (minWeight > neighbourMatrix[currentCity.Number, city.Number] && neighbourMatrix[currentCity.Number, city.Number] != 0)
                        {
                            minWeight = neighbourMatrix[currentCity.Number, city.Number];
                            tmp = city;
                            App.Current.Dispatcher.BeginInvoke((Action)delegate
                            {
                                CurrentBestEdge.Clear();
                                CurrentBestEdge.Add(new Edge(currentCity, city, neighbourMatrix[currentCity.Number, city.Number]));
                            });
                        }
                        App.Current.Dispatcher.BeginInvoke((Action)delegate
                        {
                            CurrentEdges.Add(new Edge(currentCity, city, neighbourMatrix[currentCity.Number, city.Number]));
                        });
                        System.Threading.Thread.Sleep(delay);
                    }
                    App.Current.Dispatcher.BeginInvoke((Action)delegate
                    {
                        CurrentFinalEdges.Add(new Edge(currentCity, tmp, neighbourMatrix[currentCity.Number, tmp.Number]));
                    });
                    currentBestDistance += neighbourMatrix[currentCity.Number, tmp.Number];

                    System.Threading.Thread.Sleep(delay);
                    App.Current.Dispatcher.BeginInvoke((Action)delegate
                    {
                        CurrentEdges.Clear();
                    });
                    System.Threading.Thread.Sleep(delay);
                    App.Current.Dispatcher.BeginInvoke((Action)delegate
                    {
                        VisitedCities.Add(tmp);
                    });
                    currentCity = new City(tmp.X, tmp.Y, tmp.Number);
                    cities.Remove(tmp);
                    minWeight = int.MaxValue;
                    System.Threading.Thread.Sleep(delay);

                } while (cities.Any());
                currentBestDistance += neighbourMatrix[CurrentFinalEdges.Last().City2.Number,
                 CurrentFinalEdges.First().City1.Number];
                App.Current.Dispatcher.BeginInvoke((Action)delegate
                {
                    CurrentBestEdge.Clear();
                    CurrentFinalEdges.Add(new Edge(CurrentFinalEdges.Last().City2, CurrentFinalEdges.First().City1,
                            neighbourMatrix[CurrentFinalEdges.Last().City2.Number,
                            CurrentFinalEdges.First().City1.Number]));


                });
                System.Threading.Thread.Sleep(delay*3);
                if (currentBestDistance < bestDistance)
                {
                    bestDistance = currentBestDistance;

                    App.Current.Dispatcher.BeginInvoke((Action)delegate
                    {
                        FinalEdges.Clear();
                        foreach (Edge edge in CurrentFinalEdges)
                            FinalEdges.Add(edge);
                    });
                }
                System.Threading.Thread.Sleep(delay);
                App.Current.Dispatcher.BeginInvoke((Action)delegate
                {
                    CurrentFinalEdges.Clear();
                });
                currentBestDistance = 0;
                System.Threading.Thread.Sleep(delay);
            }
            return new Tuple<int, string>(bestDistance, CreatePath(FinalEdges));
        }
    }
}
