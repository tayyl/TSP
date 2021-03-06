﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using Salesman.Model;
namespace Salesman.ViewModel
{
    public class AlgorithmCB
    {
        private AlgorithmType id;
        public AlgorithmType Id { get { return id; } set { id = value; } }
        private string name;
        public string Name { get { return name; } set { name = value; } }
    }
    public class SalesmanVM : INotifyPropertyChanged
    {
        private int citiesAmount;
        private string bestDistance;
        SalesmanM model;
        private string path;
        #region Attributes
        public int AreaWidth { get; set; } = 900;
        public int AreaHeight { get; set; } = 650;
        public int Delay { get; set; } = 50;
        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
                OnPropertyChanged("Path");
            }
        }
        public string BestDistance
        {
            get { return "Best Distance: " + bestDistance; }
            set { bestDistance = value; OnPropertyChanged(nameof(BestDistance)); }
        }
        public int CitiesAmount
        {
            get
            {
                return citiesAmount;
            }
            set
            {
                citiesAmount = value;
                OnPropertyChanged(nameof(CitiesAmount));
            }
        }
        #endregion
        #region ObservableCollections for printing graph
        public AlgorithmCB AlgorithmCBPicked { get; set; }
        public ObservableCollection<AlgorithmCB> AlgorithmCBs { get; set; }
        public ObservableCollection<City> Cities { get; set; }
        public ObservableCollection<City> VisitedCities { get; set; }
        public ObservableCollection<Edge> Edges { get; set; }
        public ObservableCollection<Edge> CurrentEdges { get; set; }
        public ObservableCollection<Edge> CurrentBestEdge { get; set; }
        public ObservableCollection<Edge> FinalEdges { get; set; }
        public ObservableCollection<Edge> CurrentFinalEdges { get; set; }
        #endregion
        #region Commands
        ICommand drawGraph;
        public ICommand DrawGraph
        {
            get
            {
                return drawGraph;
            }
        }
        ICommand runCommand;
        public ICommand RunCommand
        {
            get
            {
                return runCommand;
            }
        }
        #endregion
        #region Locks for async updates of ObservableCollections
        private object _lockCurrentEdges = new object();
        private object _lockCurrentBestEdges = new object();
        private object _lockFinalEdges = new object();
        private object _lockCurrentFinalEdges = new object();
        private object _lockVisitedCities= new object();
        #endregion
        public SalesmanVM()
        {
            model = new SalesmanM(AreaWidth-30, AreaHeight-30, 0);
            AlgorithmCBs = new ObservableCollection<AlgorithmCB>()
            {
                new AlgorithmCB()
                {
                    Id=AlgorithmType.Astar,
                    Name="A*"
                },
                new AlgorithmCB()
                {
                    Id=AlgorithmType.NearestNeighbour,
                    Name="Nearest Neighbour"
                },
                new AlgorithmCB()
                {
                    Id=AlgorithmType.SimulatedAnnealing,
                    Name="Simulated Annealing"
                },
                new AlgorithmCB()
                {
                    Id=AlgorithmType.AntColony,
                    Name="AntColony"
                }
            };
            Cities = new ObservableCollection<City>();
            Edges = new ObservableCollection<Edge>();
            VisitedCities = new ObservableCollection<City>();
            CurrentEdges = new ObservableCollection<Edge>();
            CurrentBestEdge = new ObservableCollection<Edge>();
            FinalEdges = new ObservableCollection<Edge>();
            CurrentFinalEdges = new ObservableCollection<Edge>();
            BindingOperations.EnableCollectionSynchronization(CurrentEdges, _lockCurrentEdges);
            BindingOperations.EnableCollectionSynchronization(FinalEdges, _lockFinalEdges);
            BindingOperations.EnableCollectionSynchronization(CurrentFinalEdges, _lockCurrentFinalEdges);
            BindingOperations.EnableCollectionSynchronization(VisitedCities, _lockVisitedCities);
            BindingOperations.EnableCollectionSynchronization(CurrentBestEdge, _lockCurrentBestEdges);
            drawGraph = new RelayCommand()
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x =>
                {
                    VisitedCities.Clear();
                    FinalEdges.Clear();
                    CurrentEdges.Clear();
                    CurrentBestEdge.Clear();
                    model.GenerateRandomCities(CitiesAmount);
                    model.GenerateRandomDistances();
                    Cities = new ObservableCollection<City>(model.Cities);
                    Edges = new ObservableCollection<Edge>(model.Edges);
                    OnPropertyChanged(nameof(Cities));
                    OnPropertyChanged(nameof(Edges));
                    OnPropertyChanged(nameof(VisitedCities));
                    OnPropertyChanged(nameof(FinalEdges));
                    OnPropertyChanged(nameof(CurrentEdges));
                    OnPropertyChanged(nameof(CurrentBestEdge));
                }
            };
            runCommand = new RelayCommand()
            {
                CanExecuteDelegate = x => Cities.Any(),
                ExecuteDelegate = x =>
                {
                    VisitedCities.Clear();
                    FinalEdges.Clear();
                    CurrentEdges.Clear();
                    CurrentBestEdge.Clear();
                    OnPropertyChanged(nameof(VisitedCities));
                    OnPropertyChanged(nameof(FinalEdges));
                    OnPropertyChanged(nameof(CurrentEdges));
                    OnPropertyChanged(nameof(CurrentBestEdge));

                    Task.Factory.StartNew(() => { Tuple<int,string> result= model.TSP(AlgorithmCBPicked.Id,Delay,VisitedCities, CurrentEdges, CurrentBestEdge, CurrentFinalEdges, FinalEdges);
                        Path = result.Item2;
                        BestDistance = result.Item1.ToString();
                    });
      
                }
            };
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
