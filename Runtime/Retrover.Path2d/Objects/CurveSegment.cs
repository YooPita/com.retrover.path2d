using UnityEngine;

namespace Retrover.Path2d
{
    public class CurveSegment : IPathPart, IPathPartTotalLength, IPathPartNormal
    {
        public CurveSegment(Vector2 position, Vector2 nextPosition)
        {
            _position = position;
            _nextPosition = nextPosition;
            _normal = new NormalizedVector2(nextPosition - position);
            _nextNormal = _normal;
            _length = Vector2.Distance(position, nextPosition);
        }

        private float _totatlLength = 0f;
        private float _length;
        private NormalizedVector2 _normal;
        private NormalizedVector2 _nextNormal;
        private Vector2 _position;
        private Vector2 _nextPosition;

        public void SayTotalLength(IPathPartTotalLength pathPart)
        {
            pathPart.SetTotalLength(_totatlLength + _length);
        }

        public void SayNormal(IPathPartNormal pathPart)
        {
            pathPart.SetNormal(_normal);
        }

        public float GetApproximateDistance(Vector2 position)
        {
            var distance = Vector2.Distance(position, _position);
            var nextDistance = Vector2.Distance(position, _nextPosition);
            return distance < nextDistance ? distance : nextDistance;
        }

        public PathPosition GetNearestPathPosition(Vector2 position)
        {
            var nearestPosition = GetClosestPoint(position);
            var distance = Vector2.Distance(nearestPosition, _position);
            var lerp = distance / _length;
            return new PathPosition()
            {
                Position = nearestPosition,
                Length = _totatlLength + distance,
                Normal = _normal.Lerp(_nextNormal, lerp)
            };
        }

        public void SetTotalLength(float totalLength)
        {
            _totatlLength = totalLength;
        }

        public void SetNormal(NormalizedVector2 vector)
        {
            _nextNormal = vector;
        }

        public bool CheckCanGivePoint(float length)
        {
            return length - _totatlLength >= 0 && length - _totatlLength <= _length;
        }

        public PathPosition GetPoint(float length)
        {
            var localLength = length - _totatlLength;
            var lerp = localLength / _length;
            var nearestPosition = Vector2.Lerp(_position, _nextPosition, lerp);
            return new PathPosition()
            {
                Position = nearestPosition,
                Length = length,
                Normal = _normal.Lerp(_nextNormal, lerp)
            };
        }

        private Vector2 GetClosestPoint(Vector2 point)
        {
            Vector2 aToPoint = point - _position;
            Vector2 aToB = _nextPosition - _position;
            float magnitudeAB = aToB.magnitude;
            float ABAPproduct = Vector2.Dot(aToPoint, aToB);
            float distance = ABAPproduct / magnitudeAB;
            if (distance < 0)
                return _position;
            else if (distance > 1)
                return _nextPosition;
            else
                return _position + aToB * distance;
        }
    }
}