namespace PathFinding
{
    public class Fitness
    {
        public bool IsReachingEnd;
        public int MovesCount;

        public Fitness(bool isReachingEnd, int movesCount)
        {
            IsReachingEnd = isReachingEnd;
            MovesCount = movesCount;
        }
    }
}