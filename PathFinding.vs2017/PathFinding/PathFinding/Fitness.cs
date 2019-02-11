namespace PathFinding
{
    public class Fitness
    {
        public bool IsReachingEnd;
        public int MoveCount;
        public int? TotalSquareDistanceAverage;

        public Fitness(bool isReachingEnd, int moveCount, int? totalSquareDistanceAverage)
        {
            IsReachingEnd = isReachingEnd;
            MoveCount = moveCount;
            TotalSquareDistanceAverage = totalSquareDistanceAverage;
        }
    }
}