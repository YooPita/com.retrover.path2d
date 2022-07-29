using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Retrover.Path2d
{
    public class Curve : IPathPart, IPathPartTotalLength, IPathPartNormal
    {
        public Curve(CurvePoint point, CurvePoint nextPoint, CurveBakeOptions options)
        {
            BakePoints(point, nextPoint, options);
            var segments = new List<CurveSegment>();
            for (int i = 0; i < BakedPoints.Length - 1; i++)
            {
                segments.Add(new CurveSegment(BakedPoints[i], BakedPoints[i + 1]));
                if (i > 0)
                {
                    segments[i - 1].SayTotalLength(segments[i]);
                    segments[i].SayNormal(segments[i - 1]);
                }
            }
            _pathParts = segments.ToList<IPathPart>();
            segments[^1].SayTotalLength(this);
            segments[0].SayNormal(this);
            segments[^1].SayNormal(this);
        }

        public Vector2[] BakedPoints { get; private set; }

        private float _totatlLength = 0f;
        private float _length = 0f;
        private List<IPathPart> _pathParts;
        private NormalizedVector2 _normal;
        private NormalizedVector2 _nextNormal;

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
            var distance = new MinimalDistanceFirstIndex(
                _pathParts[0].GetApproximateDistance(position), 0);
            for (int i = 1; i < _pathParts.Count; i++)
                distance.NewDistanceFinded(_pathParts[i].GetApproximateDistance(position), i);
            return distance.Distance;
        }

        public PathPosition GetNearestPathPosition(Vector2 position)
        {
            var multiDistance = new MinimalDistanceMultiplyIndex(
                _pathParts[0].GetApproximateDistance(position), 0);
            for (int i = 1; i < _pathParts.Count; i++)
                multiDistance.NewDistanceFinded(_pathParts[i].GetApproximateDistance(position), i);
            var indices = multiDistance.GetIndexList();
            var paths = new List<PathPosition>();
            for (int i = 0; i < indices.Length; i++)
            {
                var point = _pathParts[indices[i]].GetNearestPathPosition(position);
                paths.Add(new PathPosition()
                {
                    Length = point.Length + _totatlLength,
                    Normal = point.Normal,
                    Position = point.Position
                });
            }
            var minimalDistance = new MinimalDistanceFirstIndex(
                Vector2.Distance(paths[0].Position, position), 0);
            for (int i = 1; i < paths.Count; i++)
                minimalDistance.NewDistanceFinded(Vector2.Distance(paths[i].Position, position), i);
            return paths[minimalDistance.Index];
        }

        public void SetTotalLength(float totalLength)
        {
            if (_length == 0) _length = totalLength;
            else _totatlLength = totalLength;
        }

        public void SetNormal(NormalizedVector2 vector)
        {
            if (_normal.X == 0 && _normal.Y == 0) _normal = vector;
            else
            {
                _nextNormal = vector;
                ((IPathPartNormal)_pathParts[^1]).SetNormal(_nextNormal);
            }
        }

        public bool CheckCanGivePoint(float length)
        {
            return length - _totatlLength >= 0 && length - _totatlLength <= _length;
        }

        public PathPosition GetPoint(float length)
        {
            length -= _totatlLength;
            for (int i = 0; i < _pathParts.Count; i++)
            {
                if (_pathParts[i].CheckCanGivePoint(length))
                {

                    var point = _pathParts[i].GetPoint(length);
                    return new PathPosition()
                    {
                        Length = point.Length + _totatlLength,
                        Normal = point.Normal,
                        Position = point.Position
                    };
                }
            }
                
            throw new Exception("The length lies outside the curve.");
        }

        private Vector2 GetCubicCurvePoint(CurvePoint point, CurvePoint nextPoint, float lerp)
        {
            var part1 = Vector2.Lerp(point.Position, point.RightHandle, lerp);
            var part2 = Vector2.Lerp(point.RightHandle, nextPoint.LeftHandle, lerp);
            var part3 = Vector2.Lerp(nextPoint.LeftHandle, nextPoint.Position, lerp);

            return Vector2.Lerp(
                Vector2.Lerp(part1, part2, lerp),
                Vector2.Lerp(part2, part3, lerp),
                lerp);
        }

        private void BakePoints(CurvePoint point, CurvePoint nextPoint, CurveBakeOptions options)
        {
            float length = CalculateLength(point, nextPoint);
            int accuracy = 10;
            float step = 1f / Mathf.CeilToInt(length * accuracy);
            List<Vector2> points = new List<Vector2>();
            float errorDot = 1f - options.MaximumAngleError / 90f;
            float distanceLastVertex = 0;
            points.Add(point.Position);
            Vector2 findedPoint = GetCubicCurvePoint(point, nextPoint, step);
            for (float i = step; i <= 1f - step; i += step)
            {
                Vector2 findedNextPoint = GetCubicCurvePoint(point, nextPoint, i + step);
                var dot = Vector2.Dot((findedPoint - points[^1]).normalized, (findedNextPoint - findedPoint).normalized);
                if (dot <= errorDot && distanceLastVertex >= options.MinimumVertexDistance)
                {
                    points.Add(findedPoint);
                    distanceLastVertex = 0;
                }
                else
                    distanceLastVertex += Vector2.Distance(points[^1], findedPoint);
                findedPoint = findedNextPoint;
            }
            points.Add(nextPoint.Position);
            BakedPoints = points.ToArray();
        }

        private float CalculateLength(CurvePoint point, CurvePoint nextPoint)
        {
            float length = (point.Position - point.RightHandle).magnitude +
                (point.RightHandle - nextPoint.LeftHandle).magnitude +
                (nextPoint.LeftHandle - nextPoint.Position).magnitude;
            return (point.Position - nextPoint.Position).magnitude + length / 2f;
        }
    }
}