using UnityEngine;

namespace Retrover.Path2d
{
    public struct PathPosition
    {
        public Vector2 Position { get; set; }
        public NormalizedVector2 Normal { get; set; }
        public float Length { get; set; }
    }
}