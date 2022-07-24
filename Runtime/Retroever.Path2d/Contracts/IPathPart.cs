using UnityEngine;

namespace Retroever.Path2d
{
    public interface IPathPart
    {
        public float GetApproximateDistance(Vector2 position);
        public PathPosition GetNearestPathPosition(Vector2 position);
        public bool CheckCanGivePoint(float length);
        public PathPosition GetPoint(float length);
    }
}