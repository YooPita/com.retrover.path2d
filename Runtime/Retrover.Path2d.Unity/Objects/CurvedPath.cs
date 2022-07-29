using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Retrover.Path2d.Unity
{
    public class CurvedPath : MonoBehaviour, IPath
    {
        [field: SerializeField, HideInInspector] public int CurrentPointId { get; private set; } = 0;
        [field: SerializeField, HideInInspector] public List<EditableCurvePoint> Points { get; private set; } = new List<EditableCurvePoint>();
        [field: SerializeField] public bool IsLoop { get; private set; }
        [field: SerializeField] public CurveBakeOptions BakeOptions { get; private set; }
        [field: SerializeField, HideInInspector] public List<Vector2> BakedPoints { get; private set; }
        private float NewHandleDistance => _newPointDistance * 0.2f;
        private IPath _path;

        [SerializeField, HideInInspector] private float _newPointDistance = 5f;
        [SerializeField, HideInInspector] private GhostTransform _lastTranform;

        private void Awake()
        {
            if(_path == null) BakePoints();
        }

        public void CheckPosition()
        {
            if (_lastTranform == null)
            {
                _lastTranform = new GhostTransform(transform);
                return;
            }
            if (_lastTranform.Update(transform)) ApplyTransformToPoints();
        }

        public void AddPoint()
        {
            if (Points.Count == 0)
            {
                Vector3 position = transform.position;
                Vector3 rightHandle = position + Vector3.forward * NewHandleDistance;
                Points.Add(new EditableCurvePoint(position, rightHandle));
            }
            else
            {
                Vector3 direction = (Points[^1].RightHandle - Points[^1].Position).normalized;
                Vector3 position = Points[^1].Position +  direction * _newPointDistance;
                Vector3 rightHandle = position + direction * NewHandleDistance;
                Points.Add(new EditableCurvePoint(position, rightHandle));
            }
            CurrentPointId = Points.Count - 1;
            BakePoints();
        }

        public void RemoveCurrentPoint()
        {
            if (Points.Count == 0) return;
            Points.RemoveAt(CurrentPointId);
            CurrentPointId = CurrentPointId - 1 < 0 ? 0 : CurrentPointId - 1;
            if (Points.Count == 1)
            {
                CurrentPointId = 0;
                Points.Clear();
            }
            BakePoints();
        }

        public void Reverse()
        {
            if (Points.Count == 0) return;
            Points.Reverse();
            for (int i = 0; i < Points.Count; i++)
                Points[i].SetRightHand(Points[i].LeftHandle);
            CurrentPointId = Points.Count - 1 - CurrentPointId;
            BakePoints();
        }

        public void BakePoints()
        {
            if (Points.Count <= 1)
            {
                _path = null;
                return;
            }

            var points = new List<CurvePoint>();
            for (int i = 0; i < Points.Count; i++)
                points.Add(Points[i].CastToCurvePoint());
            var path = new Path(points, IsLoop, BakeOptions);
            BakedPoints = path.BakedPoints.ToList();
            _path = path;
        }

        public void SetCurrentPointId(int id)
        {
            if (id < 0 || id >= Points.Count)
                throw new System.IndexOutOfRangeException("CurrentPoint out of range");
            CurrentPointId = id;
        }

        public void Attach(IPathClient client, Vector2 position)
        {
            if (_path == null) BakePoints();
            _path.Attach(client, position);
        }

        public void Move(IPathClient client, float position)
        {
            if (_path == null) BakePoints();
            _path.Move(client, position);
        }

        private void ApplyTransformToPoints()
        {
            var differencePosition = _lastTranform.GetDifferencePosition();
            var differenceRotation = _lastTranform.GetDifferenceRotation();
            var differenceScale = _lastTranform.GetDifferenceScale();

            for (int i = 0; i < Points.Count; i++)
            {
                ApplyPosition(Points[i], differencePosition);
                ApplyScale(Points[i], differenceScale);
                ApplyRotation(Points[i], differenceRotation);
            }
            BakePoints();
        }

        private void ApplyPosition(EditableCurvePoint point, Vector3 position)
        {
            point.Position += position;
            point.SetRightHand(point.RightHandle + position);
        }

        private void ApplyScale(EditableCurvePoint point, Vector3 scale)
        {
            var position = point.Position - transform.position;
            var handPosition = point.RightHandle - transform.position;

            point.Position = new Vector3(
                    position.x * scale.x,
                    position.y * scale.y,
                    position.z * scale.z) + transform.position;
            point.SetRightHand(new Vector3(
                handPosition.x * scale.x,
                handPosition.y * scale.y,
                handPosition.z * scale.z) + transform.position);
        }

        private void ApplyRotation(EditableCurvePoint point, Quaternion rotation)
        {
            var position = point.Position - transform.position;
            var handPosition = point.RightHandle - transform.position;
            position = rotation * position;
            handPosition = rotation * handPosition;
            point.Position = position + transform.position;
            point.SetRightHand(handPosition + transform.position);
        }
    }
}