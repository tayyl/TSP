using System;
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
    public class SalesmanVM : INotifyPropertyChanged
    {
        private int citiesAmount;
        private string bestDistance;
        SalesmanM model;
        public int AreaWidth { get; set; } = 900;
        public int AreaHeight { get; set; } = 650;
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
        public ObservableCollection<City> Cities { get; set; }
        public ObservableCollection<City> VisitedCities { get; set; }
        public ObservableCollection<Edge> Edges { get; set; }
        public ObservableCollection<Edge> CurrentEdges { get; set; }
        public ObservableCollection<Edge> FinalEdges { get; set; }
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
        private object _lockCurrentEdges = new object();
        private object _lockFinalEdges = new object();
        private object _lockVisitedCities= new object();
        public SalesmanVM()
        {
            model = new SalesmanM(AreaWidth-30, AreaHeight-30, 0);
            Cities = new ObservableCollection<City>();
            Edges = new ObservableCollection<Edge>();
            VisitedCities = new ObservableCollection<City>();
            CurrentEdges = new ObservableCollection<Edge>();
            FinalEdges = new ObservableCollection<Edge>();
            BindingOperations.EnableCollectionSynchronization(CurrentEdges, _lockCurrentEdges);
            BindingOperations.EnableCollectionSynchronization(FinalEdges, _lockFinalEdges);
            BindingOperations.EnableCollectionSynchronization(VisitedCities, _lockVisitedCities);
            drawGraph = new RelayCommand()
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x =>
                {
                    VisitedCities.Clear();
                    FinalEdges.Clear();
                    CurrentEdges.Clear();
                    model.GenerateRandomCities(CitiesAmount);
                    model.GenerateRandomDistances();
                    Cities = new ObservableCollection<City>(model.Cities);
                    Edges = new ObservableCollection<Edge>(model.Edges);
                    OnPropertyChanged(nameof(Cities));
                    OnPropertyChanged(nameof(Edges));
                    OnPropertyChanged(nameof(VisitedCities));
                    OnPropertyChanged(nameof(FinalEdges));
                    OnPropertyChanged(nameof(CurrentEdges));
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
                    OnPropertyChanged(nameof(VisitedCities));
                    OnPropertyChanged(nameof(FinalEdges));
                    OnPropertyChanged(nameof(CurrentEdges));

                    model.ChoseAlgorithm(AlgorithmType.NearestNeighbour);
                    Task.Factory.StartNew(() => { BestDistance = model.Algorithm.TSP(VisitedCities, CurrentEdges, FinalEdges).ToString(); });
                      

                   
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
