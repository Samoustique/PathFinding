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

        public Individual(int moveCount, List<Direction> moves) : this(moveCount)
        {
            _moves = moves;
        }

        internal void GenerateRandom()
        {
            var directions = Enum.GetValues(typeof(Direction));

            for (int i = 0; i < MoveCount; ++i)
            {
                Direction randomValue = (Direction) directions.GetValue(GetRandomNumber(directions.Length));
                _moves.Add(randomValue);
            }
        }

        private static readonly Random getrandom = new Random();

        public static int GetRandomNumber(int max)
        {
            lock (getrandom)
            {
                return getrandom.Next(max);
            }
        }

        public static int GetRandomNumber(int min, int max)
        {
            lock (getrandom)
            {
                return getrandom.Next(min, max);
            }
        }

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
                List<Direction> moves = new List<Direction>(parent1.MoveCount);

                for (int j = 0; j < parent1.MoveCount; ++j)
                {
                    Individual parent;
                    parent = (GetRandomNumber(1, 3) == 1) ? parent1 : parent2;
                    moves.Add(parent.GetMoveAt(j));
                }

                inviduals.Add(new Individual(parent1.MoveCount, moves));
            }

            return inviduals;
        }

        internal void Mutate(float rate)
        {
            int changingDirectionCount = (int)(MoveCount * rate);
            List<int> randomIndexes = new List<int>(changingDirectionCount);
            var directions = Enum.GetValues(typeof(Direction));

            for (int i = 0; i < changingDirectionCount; ++i)
            {
                int random = GetRandomNumber(0, MoveCount);
                while(randomIndexes.Contains(random))
                {
                    random = GetRandomNumber(0, MoveCount);
                }

                Direction randomDirection = (Direction)directions.GetValue(GetRandomNumber(directions.Length));
                while(randomDirection == _moves[random])
                {
                    randomDirection = (Direction)directions.GetValue(GetRandomNumber(directions.Length));
                }

                _moves[random] = randomDirection;
                randomIndexes.Add(random);
            }
        }

        private Direction GetMoveAt(int j)
        {
            return _moves[j];
        }
    }
}
