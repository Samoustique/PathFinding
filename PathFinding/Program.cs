using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinding
{
    class Program
    {
        static void Main(string[] args)
        {
            GeneticAlgorithm algo = new GeneticAlgorithm(new PathFindingGenetic());
            algo.Launch();
        }
    }
}
