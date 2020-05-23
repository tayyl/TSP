using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Salesman.Model
{
    public enum AlgorithmType { NearestNeighbour=0, Astar=1, SimulatedAnnealing=2, AntColony=3}
    public class SalesmanM
    {
        public List<City> Cities { get; private set; }
        public List<Edge> Edges { get; private set; }
       
        public int[,] NeighbourMatrix { get; private set; }
        int areaWidth, areaHeight;
        Random rnd;
        public SalesmanM(int width, int height,int seed)
        {
            rnd = new Random(seed);
            areaHeight = height;
            areaWidth = width;
            Cities = new List<City>();
            Edges = new List<Edge>();
        }
        public Tuple<int,string> TSP(AlgorithmType algorithmType,int delay, ICollection<City> VisitedCities, ICollection<Edge> CurrentEdges, ICollection<Edge> CurrentBestEdge, ICollection<Edge> CurrentFinalEdges, ICollection<Edge> FinalEdges)
        {
            switch (algorithmType)
            {
                case AlgorithmType.Astar: 
                    Astar astar = new Astar(NeighbourMatrix, Cities, delay);
                   return  astar.TSP(VisitedCities, CurrentEdges, FinalEdges);
                   
                case AlgorithmType.NearestNeighbour: 
                    NearestNeighbour nearest = new NearestNeighbour(NeighbourMatrix, Cities, delay);
                    return nearest.TSP(VisitedCities, CurrentEdges, CurrentBestEdge, CurrentFinalEdges, FinalEdges);
                    
                case AlgorithmType.SimulatedAnnealing:
                    Annealing annealing = new Annealing(NeighbourMatrix, Cities, delay);
                    annealing.GetRandomInstance(rnd);
                   return annealing.TSP(VisitedCities, CurrentBestEdge, FinalEdges);
                case AlgorithmType.AntColony:
                    AntColony antColony = new AntColony(NeighbourMatrix, Cities, delay);
                    antColony.GetRandomInstance(rnd);
                    return antColony.TSP(FinalEdges, CurrentFinalEdges);
                    
                 
            }
            return new Tuple<int, string>(0,"");
        }
        public City GetRandomCity(int number)
        {
            return new City(rnd.Next(0, areaWidth), rnd.Next(areaHeight), number);
        }
        public void GenerateRandomCities(int amount)
        {
            Cities.Clear();
            for (int i = 0; i < amount; i++)
                Cities.Add(GetRandomCity(i));
        }
        public void GenerateRandomDistances()
        {
            Edges.Clear();
            NeighbourMatrix = new int[Cities.Count, Cities.Count];
            for(int i=0; i<Cities.Count; i++)
                for(int j=0; j<Cities.Count; j++)
                {
                    if (j>i)
                    {
                        int value = rnd.Next(1, 20);
                        if (rnd.Next(0, 100) > -1)
                        {
                            NeighbourMatrix[i, j] = value;
                            NeighbourMatrix[j, i] = value;
                            if(NeighbourMatrix[i,j]>0)
                              Edges.Add(new Edge(Cities[i], Cities[j], NeighbourMatrix[i, j]));
                        }
                    }
                }
            
        }
    }
}
