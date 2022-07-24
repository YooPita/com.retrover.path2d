using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Retrover.Path2d
{
    public class Path : IPath, IPathPartTotalLength
    {
        public Path(List<CurvePoint> points, int precision, bool isLoop)
        {
            if (precision < 1) throw new ArgumentOutOfRangeException("Precision cannot be less than one.");
            if (points.Count < 2) throw new ArgumentOutOfRangeException("The number of points cannot be less than two.");
            _isLoop = isLoop;
            var curves = new List<Curve>();
            for (int i = 1; i < points.Count; i++)
            {
                curves.Add(new Curve(points[i - 1], points[i], precision));
                if (i > 1)
                {
                    curves[i - 2].SayTotalLength(curves[i - 1]);
                    curves[i - 1].SayNormal(curves[i - 2]);
                }
            }
            if (isLoop)
            {
                int lastIndex = curves.Count - 1;
                curves.Add(new Curve(points[^1], points[0], precision));
                curves[^1].SayNormal(curves[lastIndex]);
                curves[lastIndex].SayTotalLength(curves[^1]);
                curves[0].SayNormal(curves[^1]);
            }
            _pathParts = curves.ToList<IPathPart>();
            curves[^1].SayTotalLength(this);
        }

        private List<IPathPart> _pathParts = new List<IPathPart>();
        private float _totatlLength = 0f;
        private bool _isLoop;

        public void Attach(IPathClient client, Vector2 position)
        {
            var multiDistance = new MinimalDistanceMultiplyIndex(
                _pathParts[0].GetApproximateDistance(position), 0);
            for (int i = 1; i < _pathParts.Count; i++)
                multiDistance.NewDistanceFinded(_pathParts[i].GetApproximateDistance(position), i);
            var indices = multiDistance.GetIndexList();
            var paths = new List<PathPosition>();
            for (int i = 0; i < indices.Length; i++)
                paths.Add(_pathParts[indices[i]].GetNearestPathPosition(position));
            var minimalDistance = new MinimalDistanceFirstIndex(
                Vector2.Distance(paths[0].Position, position), 0);
            for (int i = 1; i < paths.Count; i++)
                minimalDistance.NewDistanceFinded(Vector2.Distance(paths[i].Position, position), i);
            client.UpdatePosition(paths[minimalDistance.Index]);
        }

        public void Move(IPathClient client, float position)
        {
            position = ClampLength(position);
            for (int i = 0; i < _pathParts.Count; i++)
                if (_pathParts[i].CheckCanGivePoint(position))
                {
                    client.UpdatePosition(_pathParts[i].GetPoint(position));
                    return;
                }
            throw new Exception("The length lies outside the curve.");
        }

        public void SetTotalLength(float totalLength)
        {
            _totatlLength = totalLength;
        }

        private float ClampLength(float length)
        {
            if (length >= 0 && length < _totatlLength) return length;
            else if (_isLoop)
            {
                length = length % _totatlLength;
                return length < 0 ? _totatlLength + length : length;
            }
            else return Math.Clamp(length, 0, _totatlLength);
        }
    }
}