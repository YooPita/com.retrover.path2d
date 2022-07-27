using UnityEngine;

namespace Retrover.Path2d.Unity
{
    public class GhostTransform
    {
        public GhostTransform(Transform transform)
        {
            _position = transform.position;
            _rotation = transform.rotation;
            _scale = transform.localScale;
            ApplyTransform(transform);
        }

        private Vector3 _position;
        private Vector3 _previousPosition;
        private Quaternion _rotation;
        private Quaternion _previousRotation;
        private Vector3 _scale;
        private Vector3 _previousScale;

        public bool Update(Transform transform)
        {
            var result = Compare(transform);
            if(result) ApplyTransform(transform);
            return result;
        }

        public Vector3 GetDifferencePosition()
        {
            return _position - _previousPosition;
        }

        public Quaternion GetDifferenceRotation()
        {
            return _rotation * Quaternion.Inverse(_previousRotation);
        }

        public Vector3 GetDifferenceScale()
        {
            return new Vector3(
                _previousScale.x == 0 ? 0 : _scale.x / _previousScale.x,
                _previousScale.y == 0 ? 0 : _scale.y / _previousScale.y,
                _previousScale.z == 0 ? 0 : _scale.z / _previousScale.z);
        }

        private void ApplyTransform(Transform transform)
        {
            _previousPosition = _position;
            _previousRotation = _rotation;
            _previousScale = _scale;
            _position = transform.position;
            _rotation = transform.rotation;
            _scale = transform.localScale;
        }

        private bool Compare(Transform transform)
        {
            return _position != transform.position ||
                _rotation != transform.rotation ||
                _scale != transform.localScale;
        }
    }
}