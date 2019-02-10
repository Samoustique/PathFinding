using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinding
{
    class GeneticAlgorithm
    {
        private const int GENERATION_LIMIT = 30;
        private int _currentGeneration;
        private IGenetic Genetic;

        public GeneticAlgorithm(IGenetic genetic)
        {
            Genetic = genetic;
        }

        public void Launch()
        {
            _currentGeneration = 0;

            Genetic.GenerateFirstPopulation();
            NewGeneration();
        }

        private void NewGeneration()
        {
            Genetic.Selection();
            Genetic.Reproduction();
            Genetic.Mutation();

            Debug.WriteLine($"=> Generation {_currentGeneration}");
            Genetic.DisplayGeneration();
            Debug.WriteLine("");

            if (_currentGeneration++ < GENERATION_LIMIT)
            {
                NewGeneration();
            }
        }
    }
}
