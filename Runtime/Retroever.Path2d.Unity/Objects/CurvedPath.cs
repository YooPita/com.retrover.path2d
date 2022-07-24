using System.Collections.Generic;
using UnityEngine;

namespace Retroever.Path2d.Unity
{
    public class CurvedPath : MonoBehaviour, IPath
    {
        [field: SerializeField, HideInInspector] public int CurrentPointId { get; private set; } = 0;
        [field: SerializeField, HideInInspector] public List<EditableCurvePoint> Points { get; private set; } = new List<EditableCurvePoint>();
        [field: SerializeField] public bool IsLoop { get; private set; }
        private float NewHandleDistance => _newPointDistance * 0.5f;
        private IPath _path;
        [SerializeField, Range(1, 20)] private int _precision = 1;
        [SerializeField, Range(1, 10)] private float _newPointDistance = 5f;
        [SerializeField] private Vector3 _currentPosition = Vector3.zero;

        private void Awake()
        {
            if(_path == null) BakePoints();
        }

        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < Points.Count; i++)
            {
                if (i == CurrentPointId)
                {
                    Gizmos.color = Color.yellow;
                    if (IsLoop || i != 0) Gizmos.DrawSphere(Points[i].LeftHandle, 0.05f);
                    if (IsLoop || i != Points.Count - 1) Gizmos.DrawSphere(Points[i].RightHandle, 0.05f);
                }
                else Gizmos.color = Color.white;
                Gizmos.DrawSphere(Points[i].Position, 0.1f);
            }
        }

        public void CheckPosition()
        {
            if (_currentPosition == Vector3.zero) _currentPosition = transform.position;
            if (transform.position != _currentPosition) MovePoints();
            _currentPosition = transform.position;
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
            if (Points.Count <= 1 && _precision <= 0)
            {
                _path = null;
                return;
            }

            var points = new List<CurvePoint>();
            for (int i = 0; i < Points.Count; i++)
                points.Add(Points[i].CastToCurvePoint());
            _path = new Path(points, _precision, IsLoop);
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

        private void MovePoints()
        {
            var offset = transform.position - _currentPosition;
            for (int i = 0; i < Points.Count; i++)
            {
                Points[i].Position = Points[i].Position + offset;
                Points[i].SetRightHand(Points[i].RightHandle + offset);
            }
            BakePoints();
        }
    }
}