using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinding
{
    public enum Direction
    {
        LEFT = 0,
        UP,
        RIGHT,
        BOTTOM
    }

    class PathFindingGenetic : IGenetic
    {
        private const int POPULATION_COUNT = 1000;
        private const int INDIVIDUAL_MOVE_COUNT = 50;
        private const float SURVIVOR_RATE = 0.2f;
        private const float MUTATION_RATE = 0.1f;

        // MAP
        private const int START_X = 0;
        private const int START_Y = 0;
        private const int END_X = MAP_WIDTH - 1;
        private const int END_Y = MAP_HEIGHT - 1;
        private const int MAP_WIDTH = 10;
        private const int MAP_HEIGHT = 10;
        private const int MOVES_COUNT_LIMIT = MAP_WIDTH * MAP_HEIGHT;
        private const char WALL = '#';
        private readonly char[,] _map =
        {
            {WALL, '-', WALL, '-', '-', '-', '-', '-', '-', '-'},
            {'-', '-', WALL, '-', '-', '-', '-', '-', '-', '-'},
            {'-', '-', WALL, '-', '-', '-', '-', '-', '-', '-'},
            {'-', '-', '-', '-', '-', '-', WALL, '-', '-', '-'},
            {'-', '-', '-', '-', '-', '-', WALL, '-', '-', '-'},
            {'-', '-', '-', '-', '-', '-', WALL, '-', '-', '-'},
            {'-', '-', WALL, '-', '-', '-', WALL, '-', '-', '-'},
            {'-', '-', WALL, '-', '-', '-', WALL, '-', '-', '-'},
            {'-', '-', WALL, '-', '-', '-', WALL, '-', '-', '-'},
            {'-', '-', WALL, '-', '-', '-', WALL, '-', '-', '-'}
        };

        /*private readonly char[,] _map =
        {
            {WALL, '-', '-', '-', '-', '-', '-', '-', '-', '-'},
            {'-', '-', '-', '-', '-', '-', '-', '-', '-', '-'},
            {'-', '-', '-', '-', '-', '-', '-', '-', '-', '-'},
            {'-', '-', '-','-', '-', '-',  '-', '-', '-', '-'},
            {'-', '-', '-','-', '-', '-',  '-', '-', '-', '-'},
            {'-', '-', '-','-', '-', '-',  '-', '-', '-', '-'},
            {'-', '-', '-', '-', '-', '-',  '-', '-', '-', '-'},
            {'-', '-', '-', '-', '-', '-',  '-', '-', '-', '-'},
            {'-', '-', '-', '-', '-', '-',  '-', '-', '-', '-'},
            {'-', '-', '-', '-', '-', '-',  '-', '-', '-', '-'}
        };*/

        private Dictionary<Individual, Fitness> _generation;

        public void GenerateFirstPopulation()
        {
            _generation = new Dictionary<Individual, Fitness>(POPULATION_COUNT);
            CreateRandomIndividuals(POPULATION_COUNT);
        }

        public void Mutation()
        {
            foreach (KeyValuePair<Individual, Fitness> entry in _generation)
            {
                if(entry.Value == null)
                {
                    entry.Key.Mutate(MUTATION_RATE);
                }
            }
        }

        public void Reproduction()
        {
            int childrenCount = POPULATION_COUNT - _generation.Count;

            if (_generation.Count <= 1)
            {
                CreateRandomIndividuals(childrenCount);
                return;
            }

            int coupleCount = _generation.Count / 2;
            int childrenPerCouple = childrenCount / coupleCount;
            IEnumerable<Individual> parents = _generation.Keys;
            for (int i = 0; i < coupleCount; ++i)
            {
                List<Individual> children = Individual.Produce(parents.ElementAt(0), parents.ElementAt(1), childrenPerCouple);
                foreach(var child in children)
                {
                    _generation.Add(child, null);
                }
            }
        }

        private void CreateRandomIndividuals(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                Individual individual = new Individual(INDIVIDUAL_MOVE_COUNT);
                individual.GenerateRandom();
                _generation.Add(individual, null);
            }
        }

        public void Selection()
        {
            ClearFitness();

            FitnessComputation();
            SortByFitness();
            KeepTheBest();
        }

        private void ClearFitness()
        {
            foreach (var key in _generation.Keys.ToList())
            {
                _generation[key] = null;
            }
        }

        private void KeepTheBest()
        {
            int keepingCount = (int) (SURVIVOR_RATE * POPULATION_COUNT);
            if (_generation.Count > keepingCount)
            {
                _generation = _generation.Take(keepingCount).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        private void SortByFitness()
        {
            var individualsToRemove = _generation.Where(pair => !pair.Value.IsReachingEnd)
                         .Select(pair => pair.Key)
                         .ToList();

            if(individualsToRemove.Count == _generation.Count)
            {
                var tmp = from pair in _generation
                          orderby pair.Value.TotalSquareDistanceAverage ascending
                          select pair;
                _generation = tmp.ToDictionary(pair => pair.Key, pair => pair.Value);
            }
            else
            {
                foreach (var individualToRemove in individualsToRemove)
                {
                    _generation.Remove(individualToRemove);
                }

                var tmp = from pair in _generation
                          orderby pair.Value.TotalSquareDistanceAverage ascending
                          select pair;
                _generation = tmp.ToDictionary(pair => pair.Key, pair => pair.Value);
            }

            // Remove the individual with no fitness (which means they fail at the very first step)
            var individualsWithNoFitness = _generation.Where(pair => pair.Value.TotalSquareDistanceAverage == null)
                 .Select(pair => pair.Key)
                 .ToList();

            foreach (var individualWithNoFitness in individualsWithNoFitness)
            {
                _generation.Remove(individualWithNoFitness);
            }
        }

        private void FitnessComputation()
        {
            List<Individual> keys = new List<Individual>(_generation.Keys);
            foreach (var individual in keys)
            {
                _generation[individual] = LaunchOnMap(individual);
            }
        }

        private Fitness LaunchOnMap(Individual individual)
        {
            int currentX = START_X;
            int currentY = START_Y;
            int movesCount = 0;
            int totalSquareDistance = 0;
            char[,] localMap = _map.Clone() as char[,];
            IEnumerator directionIterator = individual.GetEnumerator();
            bool isReachingEnd = false;

            while (directionIterator.MoveNext())
            {
                Direction move = (Direction) directionIterator.Current;

                bool wrongDirection = false;
                switch(move)
                {
                    case Direction.LEFT:
                        wrongDirection = (currentX == 0 || localMap[currentY, currentX - 1] == WALL);
                        if (!wrongDirection)
                        {
                            --currentX;
                        }
                        break;
                    case Direction.UP:
                        wrongDirection = (currentY == 0 || localMap[currentY - 1, currentX] == WALL);
                        if (!wrongDirection)
                        {
                            --currentY;
                        }
                        break;
                    case Direction.RIGHT:
                        wrongDirection = (currentX == MAP_WIDTH - 1 || localMap[currentY, currentX + 1] == WALL);
                        if (!wrongDirection)
                        {
                            ++currentX;
                        }
                        break;
                    case Direction.BOTTOM:
                        wrongDirection = (currentY == MAP_HEIGHT - 1 || localMap[currentY + 1, currentX] == WALL);
                        if (!wrongDirection)
                        {
                            ++currentY;
                        }
                        break;
                }

                int distanceX = MAP_WIDTH - currentX;
                int distanceY = MAP_HEIGHT - currentY;
                totalSquareDistance += ((distanceX * distanceX) + (distanceY * distanceY));

                if (wrongDirection)
                {
                    break;
                }

                localMap[currentY, currentX] = WALL;
                ++movesCount;

                if(currentX == END_X && currentY == END_Y)
                {
                    isReachingEnd = true;
                    break;
                }

                if (movesCount == MOVES_COUNT_LIMIT)
                {
                    break;
                }
            }

            int? average = null;
            if(movesCount > 0)
            {
                average = totalSquareDistance / movesCount;
            }
            return new Fitness(isReachingEnd, movesCount, average);
        }

        public void DisplayGeneration()
        {
            int i = 0;
            foreach (KeyValuePair<Individual, Fitness> entry in _generation)
            {
                ++i;
                string fitness = "";
                if(entry.Value != null)
                {
                    fitness = $"{entry.Value.IsReachingEnd} {entry.Value.TotalSquareDistanceAverage} {entry.Value.MoveCount} -";
                }
                Debug.WriteLine($"{i}: {fitness} {entry.Key.ToString()}");
            }
        }
    }
}
