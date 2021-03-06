﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PathFinding
{
    public enum Direction
    {
        NONE= 0,
        LEFT,
        UP,
        RIGHT,
        BOTTOM
    }

    class PathFindingGeneration : IGenetic
    {
        // MAP
        private const int START_X = 0;
        private const int START_Y = 0;
        public const int MAP_WIDTH = 10;
        public const int MAP_HEIGHT = 10;
        private const int END_X = MAP_WIDTH - 1;
        private const int END_Y = MAP_HEIGHT - 1;
        private const int MOVES_COUNT_LIMIT = MAP_WIDTH * MAP_HEIGHT;
        private const char WALL = '#';
        private const char START = 'S';
        private const char END = 'E';
        private const char WAY = 'W';

        private Dictionary<Individual, Fitness> _generation;

        public char[,] Map => _map;
        private readonly char[,] _map =
        {
            {START, '-', WALL, '-', '-', '-', '-', '-', '-', '-'},
            {'-', '-', WALL, '-', '-', '-', '-', '-', '-', '-'},
            {'-', '-', WALL, '-', '-', '-', '-', '-', '-', '-'},
            {'-', '-', '-', '-', '-', '-', WALL, '-', '-', '-'},
            {'-', '-', '-', '-', '-', '-', WALL, '-', '-', '-'},
            {'-', '-', '-', '-', '-', '-', WALL, '-', '-', '-'},
            {'-', '-', WALL, '-', '-', '-', WALL, '-', '-', '-'},
            {'-', '-', WALL, '-', '-', '-', WALL, '-', '-', '-'},
            {'-', '-', WALL, '-', '-', '-', WALL, '-', '-', '-'},
            {'-', '-', WALL, '-', '-', '-', WALL, '-', '-', END}
        };

        public int PopulationSize { get; set; } = 600;

        public int IndividualMoveCount { get; set; } = 50;

        public float SurvivorRate { get; set; } = 0.1f;

        public float MutationRate { get; set; } = 0.1f;

        public event EventHandler<BestIndividualMapArgs> BestIndividualMapChanged;

        protected virtual void OnBestIndividualMapChanged(BestIndividualMapArgs e)
        {
            EventHandler<BestIndividualMapArgs> handler = BestIndividualMapChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void GenerateFirstPopulation()
        {
            _generation = new Dictionary<Individual, Fitness>(PopulationSize);
            CreateRandomIndividuals(PopulationSize);
        }

        public void Mutation()
        {
            foreach (KeyValuePair<Individual, Fitness> entry in _generation)
            {
                if(entry.Value == null)
                {
                    entry.Key.Mutate(MutationRate);
                }
            }
        }

        public void Reproduction()
        {
            int childrenCount = PopulationSize - _generation.Count;

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
                Individual individual = new Individual(IndividualMoveCount);
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
            GenerateBestIndividualMap();
        }

        private void GenerateBestIndividualMap()
        {
            Direction[,] bestIndividualMap = new Direction[MAP_HEIGHT, MAP_WIDTH];

            int currentX = START_X;
            int currentY = START_Y;
            char[,] localMap = _map.Clone() as char[,];
            IEnumerator directionIterator = _generation.First().Key.GetEnumerator();

            while (directionIterator.MoveNext())
            {
                Direction move = (Direction)directionIterator.Current;
                bestIndividualMap[currentY, currentX] = move;

                bool wrongDirection = false;
                switch (move)
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

                if (wrongDirection || localMap[currentY, currentX] == END)
                {
                    break;
                }

                localMap[currentY, currentX] = WALL;
            }

            OnBestIndividualMapChanged(new BestIndividualMapArgs(bestIndividualMap));
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
            int keepingCount = (int) (SurvivorRate * PopulationSize);
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
                totalSquareDistance = ((distanceX * distanceX) + (distanceY * distanceY));

                if (wrongDirection)
                {
                    break;
                }

                ++movesCount;
                if (localMap[currentY, currentX] == END)
                {
                    isReachingEnd = true;
                    break;
                }

                localMap[currentY, currentX] = WALL;

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
