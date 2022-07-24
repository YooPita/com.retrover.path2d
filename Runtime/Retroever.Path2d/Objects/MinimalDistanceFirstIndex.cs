namespace Retroever.Path2d
{
    public class MinimalDistanceFirstIndex
    {
        public int Index { get; private set; }
        public float Distance { get; private set; }

        public MinimalDistanceFirstIndex(float distance, int index)
        {
            ReplaceDistanceIndex(distance, index);
        }

        public void NewDistanceFinded(float distance, int index)
        {
            if (distance < Distance) ReplaceDistanceIndex(distance, index);
        }

        private void ReplaceDistanceIndex(float distance, int index)
        {
            Distance = distance;
            Index = index;
        }
    }
}