using UnityEngine;

namespace Retroever.Path2d
{
    public struct CurvePoint
    {
        public CurvePoint(Vector2 position, Vector2 rightHandle)
        {
            Position = position;
            RightHandle = rightHandle;
            LeftHandle = InverseHandle(RightHandle, Position);
        }

        public Vector2 Position { get; private set; }
        public Vector2 RightHandle { get; private set; }
        public Vector2 LeftHandle { get; private set; }

        private static Vector2 InverseHandle(Vector2 position, Vector2 pivot)
        {
            return (position - pivot) * -1 + pivot;
        }
    }
}