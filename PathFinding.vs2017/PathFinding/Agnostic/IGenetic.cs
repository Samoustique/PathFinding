using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinding
{
    public interface IGenetic
    {
        int PopulationSize
        {
            get; set;
        }
        int IndividualMoveCount
        {
            get; set;
        }
        float SurvivorRate
        {
            get; set;
        }
        float MutationRate
        {
            get; set;
        }
        char[,] Map
        {
            get;
        }

        void GenerateFirstPopulation();
        void Mutation();
        void Reproduction();
        void Selection();
        void DisplayGeneration();
    }
}
