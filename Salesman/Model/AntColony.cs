using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Salesman.Model
{
    public class AntColony : Algorithm
    {
        public AntColony(int[,] matrix, List<City> cities, int delay = 200) : base(matrix, cities, delay)
        {
        }

        private Random random = new Random();
        // influence of pheromone on direction
        private int alpha = 3;
        // influence of adjacent node distance
        private int beta = 2;
        // pheromone decrease factor
        private double rho = 0.01;
        // pheromone increase factor
        private double Q = 2.0;
        public void GetRandomInstance(Random rnd)
        {
            random = rnd;
        }

        public Tuple<int, string> TSP(ICollection<Edge> FinalEdges, ICollection<Edge> CurrentFinalEdges, int antsAmount=10)
        {
            List<int[]> ants = InitAnts(antsAmount, cities.Count);
            int[] bestPath = BestPath(ants);
            int bestDistance = Distance(bestPath);
            double[,] pheromones = InitPheromones(cities.Count);
            int time = 0,maxTime=1000;

            while (time < maxTime)
            {
                UpdateAnts(ants, pheromones);
                UpdatePheromones(pheromones, ants);
                int[] currentBestPath = BestPath(ants);
                int currentBestDistance = Distance(currentBestPath);
                if (currentBestDistance < bestDistance)
                {
                    bestDistance = currentBestDistance;
                    bestPath = currentBestPath;
                }
                    App.Current.Dispatcher.BeginInvoke((Action)delegate
                    {
                        CurrentFinalEdges.Clear();
                        for (int i = 0; i < cities.Count; i++)
                            for (int j = 0; j < cities.Count; j++)
                            {
                                if (j > i)
                                {
                                    if (neighbourMatrix[i, j] > 0)
                                        CurrentFinalEdges.Add(new Edge(cities[i], cities[j], neighbourMatrix[i, j], Color.FromRgb(Math.Min((byte)(pheromones[i, j] * 50),(byte)255), 0, 0)));
                                }
                            }                     
                    });
                    System.Threading.Thread.Sleep(delay);
                
                time++;
            }
            App.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                for(int i=0; i<bestPath.Count()-1; i++)
                    FinalEdges.Add(new Edge(cities[bestPath[i]],cities[bestPath[i+1]],neighbourMatrix[bestPath[i],bestPath[i+1]]));
                FinalEdges.Add(new Edge(cities[bestPath[bestPath.Count() - 1]], cities[bestPath[0]], neighbourMatrix[bestPath[0], bestPath[bestPath.Count() - 1]]));
                //  CurrentFinalEdges.Clear();
            });

            System.Threading.Thread.Sleep(delay);
            return new Tuple<int, string>(bestDistance, CreatePath(FinalEdges));
        }
        private List<int[]> InitAnts(int antsAmount, int citiesAmount)
        {
            List<int[]> ants = new List<int[]>();
            for (int i = 0; i < antsAmount; i++)
                ants.Add(new int[citiesAmount]);

            for (int i = 0; i < antsAmount; i++)
            {
                int start = random.Next(0, citiesAmount);
                ants[i] = RandomTrail(citiesAmount);
            }
            return ants;
        }

        private int[] RandomTrail(int citiesAmount)
        {
            int[] path = new int[citiesAmount];

            for (int i = 0; i < citiesAmount; i++)
            {
                path[i] = i;
            }

            //creating random path using naive permutation
            for (int i = 1; i < citiesAmount; i++)
            {
                int r = random.Next(i, citiesAmount);
                int tmp = path[r];
                path[r] = path[i];
                path[i] = tmp;
            }
            return path;
        }

        private int IndexOfTarget(int[] trail, int target)
        {
            // helper for RandomTrail
            for (int i = 0; i < trail.Length; i++)
            {
                if (trail[i] == target)
                {
                    return i;
                }
            }
            throw new Exception("Target not found in IndexOfTarget");
        }

        private int Distance(int[] trail)
        {
            int result = 0;
            for (int i = 0; i < trail.Length - 1; i++)
            {
                result += neighbourMatrix[trail[i], trail[i + 1]];
            }
            result += neighbourMatrix[trail[0], trail[trail.Count()-1]];
            return result;
        }
        
        private int[] BestPath(List<int[]> ants)
        {
            // best trail has shortest total length
            double bestLength = Distance(ants[0]);
            int idxBestLength = 0;
            for (int k = 1; k < ants.Count; k++)
            {
                double len = Distance(ants[k]);
                if (len < bestLength)
                {
                    bestLength = len;
                    idxBestLength = k;
                }
            }
            return ants[idxBestLength];
        }

        private double[,] InitPheromones(int citiesAmount)
        {
            double[,] pheromones = new double[citiesAmount,citiesAmount];
          
            for (int i = 0; i < pheromones.GetLength(0); i++)
            {
                for (int j = 0; j < pheromones.GetLength(1); j++)
                {
                    pheromones[i,j] = 0.0001;
                }
            }
            return pheromones;
        }
        private void UpdateAnts(List<int[]> ants, double[,] pheromones)
        {
            for (int k = 0; k < ants.Count; k++)
            {
                int start = random.Next(0, cities.Count);
                int[] newTrail = BuildPath(start, pheromones);
                ants[k] = newTrail;
            }
        }

        private int[] BuildPath(int start, double[,] pheromones)
        {
            int[] trail = new int[cities.Count];
            bool[] visited = new bool[cities.Count];
            trail[0] = start;
            visited[start] = true;
            for (int i = 0; i < cities.Count - 1; i++)
            {
                int currCity = trail[i];
                int next = NextCity(currCity, visited, pheromones);
                trail[i + 1] = next;
                visited[next] = true;
            }
            return trail;
        }

        private int NextCity(int currCity, bool[] visited, double[,] pheromones)
        {
            double[] probs = CalculatePathChosingProbabilities(currCity, visited, pheromones);

            double[] cumul = new double[probs.Length + 1];
            for (int i = 0; i < cumul.Length; i++)
                cumul[i] = 0;

            for (int i = 0; i < probs.Length; i++)
            {
                cumul[i + 1] = cumul[i] + probs[i];
            }
            //all probabilities have to sum to 1.0, they cannot because of rounding problem so I'm setting it hard here
            cumul[cumul.Length - 1] = 1.0;
            double p = random.NextDouble();

            for (int i = 0; i < cumul.Length - 1; i++)
            {
                if (p >= cumul[i] && p < cumul[i + 1])
                {
                    return i;
                }
            }
            throw new Exception("Failure to return valid city in NextCity");
        }

        private double[] CalculatePathChosingProbabilities(int currCity, bool[] visited, double[,] pheromones)
        {
            // for ant located at nodeX, with visited[], return the prob of moving to each city
            double[] taueta = new double[cities.Count];
            // includes cityX and visited cities
            double sum = 0.0;
            // sum of all tauetas
            // i is the adjacent city
            for (int i = 0; i < taueta.Length; i++)
            {
                if (i == currCity)
                {
                    taueta[i] = 0.0;
                    // prob of moving to self is 0
                }
                else if (visited[i] == true)
                {
                    taueta[i] = 0.0;
                    // prob of moving to a visited city is 0
                }
                else
                {
                    taueta[i] = Math.Pow(pheromones[currCity,i], alpha) * Math.Pow((1.0 /neighbourMatrix[currCity, i]), beta);
                   
                    if (taueta[i] < 0.0001)
                    {
                        taueta[i] = 0.0001;
                    }
                    else if (taueta[i] > (double.MaxValue / (cities.Count * 100)))
                    {
                        taueta[i] = double.MaxValue / (cities.Count * 100);
                    }
                }
                sum += taueta[i];
            }

            for (int i = 0; i < taueta.Length ; i++)
            {
                taueta[i] /=  sum;
            }
            return taueta;
        }
        private void UpdatePheromones(double[,] pheromones, List<int[]> ants)
        {
            for (int i = 0; i < pheromones.GetLength(0); i++)
            {
                for (int j = i + 1; j < pheromones.GetLength(1); j++)
                {
                    for (int k = 0; k < ants.Count; k++)
                    {
                        double length = Distance(ants[k]);
                        // length of ant k path
                        double decrease = (1.0 - rho) * pheromones[i,j];
                        double increase = 0.0;
                        if (EdgeInTrail(i, j, ants[k]) == true)
                        {
                            increase = (Q / length);
                        }
                        pheromones[i,j] = decrease + increase;
                        pheromones[j,i] = pheromones[i,j];
                    }
                }
            }
        }

        private bool EdgeInTrail(int cityX, int cityY, int[] trail)
        {
            // are cityX and cityY adjacent to each other in trail[]?
            int lastIndex = trail.Length - 1;
            int idx = IndexOfTarget(trail, cityX);

            if (idx == 0 && trail[1] == cityY)
            {
                return true;
            }
            else if (idx == 0 && trail[lastIndex] == cityY)
            {
                return true;
            }
            else if (idx == 0)
            {
                return false;
            }
            else if (idx == lastIndex && trail[lastIndex - 1] == cityY)
            {
                return true;
            }
            else if (idx == lastIndex && trail[0] == cityY)
            {
                return true;
            }
            else if (idx == lastIndex)
            {
                return false;
            }
            else if (trail[idx - 1] == cityY)
            {
                return true;
            }
            else if (trail[idx + 1] == cityY)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}

