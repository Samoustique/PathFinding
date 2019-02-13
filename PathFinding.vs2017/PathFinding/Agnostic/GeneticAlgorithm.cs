using System;
using System.Diagnostics;

namespace PathFinding
{
    class GeneticAlgorithm
    {
        private int _currentGeneration;
        private IGenetic Genetic;

        public int GenerationCount { get; set; } = 30;

        public GeneticAlgorithm(IGenetic genetic)
        {
            Genetic = genetic;
        }

        public void Launch()
        {
            _currentGeneration = 1;

            OnBestIndividualMapChanged(new GenerationChangedArgs($"Generation #{_currentGeneration}"));

            Genetic.GenerateFirstPopulation();
            NewGeneration();
        }

        private void NewGeneration()
        {
            Genetic.Selection();

            Debug.WriteLine($"=> Generation {_currentGeneration}");
            Genetic.DisplayGeneration();
            Debug.WriteLine("");

            Genetic.Reproduction();
            Genetic.Mutation();

            if (_currentGeneration++ < GenerationCount)
            {
                NewGeneration();
            }
            OnBestIndividualMapChanged(new GenerationChangedArgs($"Generation #{_currentGeneration}"));
        }

        public event EventHandler<GenerationChangedArgs> GenerationChanged;

        protected virtual void OnBestIndividualMapChanged(GenerationChangedArgs e)
        {
            EventHandler<GenerationChangedArgs> handler = GenerationChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
