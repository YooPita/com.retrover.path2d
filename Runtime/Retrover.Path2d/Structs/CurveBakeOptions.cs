using System;
using UnityEngine;

namespace Retrover.Path2d
{
    [Serializable]
    public class CurveBakeOptions
    {
        public CurveBakeOptions(float maximumAngleError, float minimumVertexDistance)
        {
            if (maximumAngleError < 0 || maximumAngleError > 45) throw new ArgumentOutOfRangeException("Maximum Angle Error not in range 0..45");
            if (minimumVertexDistance < 0 || minimumVertexDistance > 1) throw new ArgumentOutOfRangeException("Maximum Angle Error not in range 0..1");
            MaximumAngleError = maximumAngleError;
            MinimumVertexDistance = minimumVertexDistance;
        }

        [field: SerializeField, Range(0, 45)] public float MaximumAngleError { get; private set; } = 0.1f;
        [field: SerializeField, Range(0, 1)] public float MinimumVertexDistance { get; private set; } = 1;
    }
}