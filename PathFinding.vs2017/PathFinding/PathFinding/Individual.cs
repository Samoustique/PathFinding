using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinding
{
    class Individual
    {
        private List<Direction> _moves;
        public int MoveCount;

        public Individual(int moveCount)
        {
            MoveCount = moveCount;
            _moves = new List<Direction>(MoveCount);
        }

        public Individual(int moveCount, IEnumerable<Direction> moves) : this(moveCount)
        {
            _moves = moves.ToList();
        }

        internal void GenerateRandom()
        {
            var directions = Enum.GetValues(typeof(Direction));

            for (int i = 0; i < MoveCount; ++i)
            {
                Direction randomValue = (Direction) directions.GetValue(_rand.Next(1, directions.Length));
                _moves.Add(randomValue);
            }
        }

        private static readonly int RANDOM_SEED = 99;
        private static readonly Random _rand = new Random();// (RANDOM_SEED);

        internal IEnumerator GetEnumerator()
        {
            return _moves.GetEnumerator();
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            foreach(var direction in _moves)
            {
                str.Append(direction.ToString()).Append(" ");
            }
            return str.ToString();
        }

        public static List<Individual> Produce(Individual parent1, Individual parent2, int childrenCount)
        {
            List<Individual> inviduals = new List<Individual>(childrenCount);
            for (int i = 0; i < childrenCount; ++i)
            {
                int[] numberList = Enumerable.Range(0, parent1.MoveCount).OrderBy(x => _rand.Next()).ToArray();
                Direction[] moves = numberList.Select(x => Direction.BOTTOM).ToArray();

                foreach(var randomID in numberList)
                {
                    Individual parent;
                    parent = (_rand.Next(0, 2) == 1) ? parent1 : parent2;
                    moves[randomID] = parent.GetMoveAt(randomID);
                }
                
                inviduals.Add(new Individual(parent1.MoveCount, moves));
            }

            return inviduals;
        }

        internal void Mutate(float rate)
        {
            for (int i = 0; i < _moves.Count; ++i)
            {
                if(_rand.Next(100) < (int)(rate * 100))
                {
                    var directions = Enum.GetValues(typeof(Direction)).OfType<Direction>().Except(new Direction[] { _moves[i] }).ToArray();

                    Direction randomDirection = (Direction)directions.GetValue(_rand.Next(1, directions.Length));
                    _moves[i] = randomDirection;
                }
            }
        }

        private Direction GetMoveAt(int j)
        {
            return _moves[j];
        }
    }
}
