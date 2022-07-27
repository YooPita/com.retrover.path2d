using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Retrover.Path2d.Unity
{
    [CustomEditor(typeof(CurvedPath))]
    public class CurvedPathEditor : Editor
    {
        private CurvedPath Curve => (CurvedPath)target;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button(Curve.EditPath ? "Transform path" : "Edit path"))
                Curve.EditPath = !Curve.EditPath;

            Tools.hidden = Curve.EditPath;
            if (Curve.EditPath)
            {
                if (GUILayout.Button("Add point"))
                {
                    UpdateGUI();
                    Undo.RecordObject(Curve, "Add point");
                    Curve.AddPoint();
                }
                if (Curve.Points.Count > 0)
                {
                    if (GUILayout.Button("Delete selected point"))
                    {
                        UpdateGUI();
                        Undo.RecordObject(Curve, "Remove point");
                        Curve.RemoveCurrentPoint();
                    }
                    if (GUILayout.Button("Reverse"))
                    {
                        UpdateGUI();
                        Undo.RecordObject(Curve, "Reverse");
                        Curve.Reverse();
                    }
                }
            }
        }

        void OnSceneGUI()
        {
            Curve.CheckPosition();
            DrawBezierCurve();
            DrawPoints();
            if (Curve.EditPath)
                for (int i = 0; i < Curve.Points.Count; i++)
                {
                    bool isFirst = i == 0;
                    bool istLast = i + 1 == Curve.Points.Count;

                    if (i == Curve.CurrentPointId)
                    {
                        if (isFirst && !Curve.IsLoop)
                            MoveFirstPoint(Curve.Points[i]);
                        else if (istLast && !Curve.IsLoop)
                            MoveLastPoint(Curve.Points[i]);
                        else
                            MovePoint(Curve.Points[i]);
                    }

                    if (i != Curve.CurrentPointId)
                        DrawButton(i, Curve.Points[i].Position);
                }
        }

        private void DrawBezierCurve()
        {
            for (int i = 0; i < Curve.Points.Count; i++)
            {
                if (i != 0)
                    DrawBezier(Curve.Points[i - 1], Curve.Points[i]);
                else if (Curve.IsLoop)
                    DrawBezier(Curve.Points[Curve.Points.Count - 1], Curve.Points[i]);
            }
        }

        private void DrawPoints()
        {
            for (int i = 0; i < Curve.Points.Count; i++)
            {
                if (i == Curve.CurrentPointId && Curve.EditPath)
                {
                    Handles.color = Color.yellow;
                    if (Curve.IsLoop || i != 0) Handles.DrawSolidDisc(Curve.Points[i].LeftHandle, Vector3.up, 0.05f);
                    if (Curve.IsLoop || i != Curve.Points.Count - 1) Handles.DrawSolidDisc(Curve.Points[i].RightHandle, Vector3.up, 0.05f);
                }
                else Handles.color = Color.white;
                Handles.DrawSolidDisc(Curve.Points[i].Position, Vector3.up, 0.1f);
                //Gizmos.DrawSphere(Curve.Points[i].Position, 0.1f);
            }
        }

        private void UpdateGUI()
        {
            EditorUtility.SetDirty(Curve);
            if (!EditorApplication.isPlaying)
                EditorSceneManager.MarkSceneDirty(Curve.gameObject.scene);
        }

        private void MovePosition(EditableCurvePoint point)
        {
            Vector3 newPointPosition = Handles.PositionHandle(point.Position, Quaternion.identity);
            if (point.Position != newPointPosition)
            {
                Undo.RecordObject(Curve, "Move point");
                var offset = point.RightHandle - point.Position;
                point.Position = newPointPosition;
                point.SetRightHand(point.Position + offset);
                Curve.BakePoints();
                UpdateGUI();
            }
        }

        private void MoveRightHand(EditableCurvePoint point)
        {
            Handles.color = Color.yellow;
            Handles.DrawLine(point.Position, point.RightHandle, 3);
            Vector3 newPointRightPosition = Handles.PositionHandle(point.RightHandle, Quaternion.identity);
            if (newPointRightPosition != point.RightHandle)
            {
                Undo.RecordObject(Curve, "Move right handle");
                point.SetRightHand(newPointRightPosition);
                Curve.BakePoints();
                UpdateGUI();
            }
        }

        private void MoveLeftHand(EditableCurvePoint point)
        {
            Handles.color = Color.yellow;
            Handles.DrawLine(point.Position, point.LeftHandle, 3);
            Vector3 newPointLeftPosition = Handles.PositionHandle(point.LeftHandle, Quaternion.identity);
            if (newPointLeftPosition != point.LeftHandle)
            {
                Undo.RecordObject(Curve, "Move left handle");
                point.SetLeftHand(newPointLeftPosition);
                Curve.BakePoints();
                UpdateGUI();
            }
        }

        private void MoveFirstPoint(EditableCurvePoint point)
        {
            MovePosition(point);
            MoveRightHand(point);
        }

        private void MoveLastPoint(EditableCurvePoint point)
        {
            MovePosition(point);
            MoveLeftHand(point);
        }

        private void MovePoint(EditableCurvePoint point)
        {
            MovePosition(point);
            MoveLeftHand(point);
            MoveRightHand(point);
        }

        private void DrawBezier(EditableCurvePoint previousPoint, EditableCurvePoint nextPoint)
        {
            Handles.DrawBezier(
                previousPoint.Position,
                nextPoint.Position,
                previousPoint.RightHandle,
                nextPoint.LeftHandle,
                Color.red,
                null,
                5);
        }

        private void DrawButton(int id, Vector3 position)
        {
            CurvedPath curve = (CurvedPath)target;
            var isClick = Handles.Button(position, new Quaternion(), 0f, .1f, Handles.DotHandleCap);
            if (isClick)
                curve.SetCurrentPointId(id);
        }
    }
}