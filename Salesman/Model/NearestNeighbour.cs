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
        int delay;
        public NearestNeighbour(int[,] matrix, List<City> cities, List<Edge> edges, int delay=200)
        {
            neighbourMatrix = (int[,])matrix.Clone();
            this.cities = new List<City>(cities);
            this.edges = new List<Edge>(edges);
            this.delay = delay;
        }
        //Every Thread Sleep is for having good animation of every step in normal algorithm it obviously can be avoided
        //every Current.Dispatcher.BeginInvoke is for asynchronously update Lists, it's here also for the purpose of good visual representation
        public override int TSP(ObservableCollection<City> VisitedCities, ObservableCollection<Edge> CurrentEdges, ObservableCollection<Edge> CurrentBestEdge, ObservableCollection<Edge> FinalEdges)
        {
            int bestDistance=0;
            City currentCity = cities.First();
            cities.RemoveAt(0); 
            App.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                VisitedCities.Add(new City(currentCity.X, currentCity.Y, currentCity.Number));
            });
            int minWeight = int.MaxValue;
            City tmp = currentCity;
            do
            {
                foreach(City city in cities)
                {
                   if (minWeight > neighbourMatrix[currentCity.Number, city.Number] && neighbourMatrix[currentCity.Number,city.Number] !=0)
                   {
                       minWeight = neighbourMatrix[currentCity.Number, city.Number];
                       tmp = city;
                        App.Current.Dispatcher.BeginInvoke((Action)delegate
                        {
                            CurrentBestEdge.Clear();
                            CurrentBestEdge.Add(new Edge(currentCity.X, currentCity.Y, city.X, city.Y, neighbourMatrix[currentCity.Number, city.Number]));
                        });
                    }
                    App.Current.Dispatcher.BeginInvoke((Action)delegate
                    {
                        CurrentEdges.Add(new Edge(currentCity.X, currentCity.Y, city.X, city.Y, neighbourMatrix[currentCity.Number, city.Number]));
                    });
                    System.Threading.Thread.Sleep(delay);
                }
                App.Current.Dispatcher.BeginInvoke((Action)delegate
                {
                    FinalEdges.Add(new Edge(currentCity.X, currentCity.Y, tmp.X, tmp.Y, neighbourMatrix[currentCity.Number, tmp.Number]));
                });
                bestDistance += neighbourMatrix[currentCity.Number, tmp.Number];

                System.Threading.Thread.Sleep(delay);
                App.Current.Dispatcher.BeginInvoke((Action)delegate
                {
                    CurrentEdges.Clear();
                });
                System.Threading.Thread.Sleep(delay);
                App.Current.Dispatcher.BeginInvoke((Action)delegate
                {
                    VisitedCities.Add(new City(tmp.X, tmp.Y, tmp.Number));
                } );
                currentCity = new City(tmp.X, tmp.Y, tmp.Number);
                cities.Remove(tmp);
                minWeight = int.MaxValue;
                System.Threading.Thread.Sleep(delay);

            } while (cities.Any());
            System.Threading.Thread.Sleep(200);
            App.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                FinalEdges.Add(new Edge(VisitedCities.Last().X, VisitedCities.Last().Y, VisitedCities.First().X, VisitedCities.First().Y, neighbourMatrix[VisitedCities.Last().Number, VisitedCities.First().Number]));

            });
            return bestDistance;
        }        
    }
}
