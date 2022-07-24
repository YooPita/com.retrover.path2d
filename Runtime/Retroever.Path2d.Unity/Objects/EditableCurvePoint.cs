using System;
using UnityEngine;

namespace Retroever.Path2d.Unity
{
    [Serializable]
    public class EditableCurvePoint
    {
        public EditableCurvePoint(Vector3 position, Vector3 rightHandle)
        {
            Position = position;
            RightHandle = rightHandle;
            LeftHandle = InverseHandle(rightHandle);
        }

        [field: SerializeField] public Vector3 Position { get; set; }
        [field: SerializeField] public Vector3 RightHandle { get; private set; }
        [field: SerializeField] public Vector3 LeftHandle { get; private set; }

        public void SetRightHand(Vector3 position)
        {
            RightHandle = position;
            LeftHandle = InverseHandle(position);
        }

        public void SetLeftHand(Vector3 position)
        {
            LeftHandle = position;
            RightHandle = InverseHandle(position);
        }

        public CurvePoint CastToCurvePoint()
        {
            return new CurvePoint(CastTo2dVectorXZ(Position), CastTo2dVectorXZ(RightHandle));
        }

        private Vector3 InverseHandle(Vector3 position)
        {
            return (position - Position) * -1 + Position;
        }

        private Vector2 CastTo2dVectorXZ(Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.z);
        }
    }
}
