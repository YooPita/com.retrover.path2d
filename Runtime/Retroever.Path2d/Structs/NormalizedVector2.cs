using UnityEngine;

namespace Retroever.Path2d
{
    public struct NormalizedVector2
    {
        public NormalizedVector2(Vector2 vector)
        {
            vector.Normalize();
            X = vector.x;
            Y = vector.y;
        }

        public float X { get; private set; }
        public float Y { get; private set; }

        public NormalizedVector2 Lerp(NormalizedVector2 vector, float amount)
        {
            return new NormalizedVector2(Vector2.Lerp(ToVector2(), vector.ToVector2(), amount));
        }

        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }
    }
}