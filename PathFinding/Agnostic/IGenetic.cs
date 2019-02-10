using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinding
{
    interface IGenetic
    {
        void GenerateFirstPopulation();
        void Mutation();
        void Reproduction();
        void Selection();
        void DisplayGeneration();
    }
}
