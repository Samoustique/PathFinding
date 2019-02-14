using System;
using System.Diagnostics;
using System.Threading;

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
            
            Genetic.GenerateFirstPopulation();
            NewGeneration();
            OnIsReadyChanged(new IsReadyChangedArgs(true));
        }

        private void NewGeneration()
        {
            Genetic.Selection();
            Genetic.Reproduction();
            Genetic.Mutation();

            OnBestIndividualMapChanged(new GenerationChangedArgs($"Generation #{_currentGeneration}"));
            if (_currentGeneration++ < GenerationCount)
            {
                Thread.Sleep(100);
                NewGeneration();
            }
        }

        public event EventHandler<IsReadyChangedArgs> IsReadyChanged;

        protected virtual void OnIsReadyChanged(IsReadyChangedArgs e)
        {
            EventHandler<IsReadyChangedArgs> handler = IsReadyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
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
