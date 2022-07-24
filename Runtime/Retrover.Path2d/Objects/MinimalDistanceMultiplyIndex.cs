using System.Collections.Generic;
using System.Linq;

namespace Retrover.Path2d
{
    public class MinimalDistanceMultiplyIndex
    {
        public float Distance { get; private set; }
        private List<int> _indices = new List<int>();

        public MinimalDistanceMultiplyIndex(float distance, int index)
        {
            ReplaceDistanceIndex(distance, index);
        }

        public int[] GetIndexList()
        {
            return _indices.Distinct().ToArray();
        }

        public void NewDistanceFinded(float distance, int index)
        {
            if (distance < Distance) ReplaceDistanceIndex(distance, index);
            else if (distance == Distance) AddIndexWithSameDistance(index);
        }

        private void AddIndexWithSameDistance(int index)
        {
            _indices.Add(index);
        }

        private void ReplaceDistanceIndex(float distance, int index)
        {
            Distance = distance;
            if (_indices.Count == 0) _indices.Add(index);
            else if (_indices.Count == 1) _indices[0] = index;
            else
            {
                _indices.Clear();
                _indices.Add(index);
            }
        }
    }
}