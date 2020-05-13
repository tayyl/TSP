using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Salesman.Model
{
    public enum AlgorithmType { NearestNeighbour=0, Astar=1, SimulatedAnnealing=2}
    public class SalesmanM
    {
        public Algorithm Algorithm { get; private set; }
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
        public void ChoseAlgorithm(AlgorithmType algorithmType)
        {
            switch (algorithmType)
            {
                case AlgorithmType.Astar: Algorithm = new Astar(); break;
                case AlgorithmType.NearestNeighbour: Algorithm = new NearestNeighbour(NeighbourMatrix, Cities, Edges); break;
                case AlgorithmType.SimulatedAnnealing: Algorithm = new Annealing(); break;
            }
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
                              Edges.Add(new Edge(Cities[i].X, Cities[i].Y, Cities[j].X, Cities[j].Y, NeighbourMatrix[i, j]));
                        }
                    }
                }
            
        }
    }
}
