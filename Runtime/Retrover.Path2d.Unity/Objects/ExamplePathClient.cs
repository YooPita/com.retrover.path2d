using UnityEngine;

namespace Retrover.Path2d.Unity
{
    public class ExamplePathClient : MonoBehaviour, IPathClient
    {
        [SerializeField] private CurvedPath _initialPath;
        [SerializeField, Range(-5, 5)] private float _speed = 1f;
        private IPathGrip _grip;

        private void Awake()
        {
            _grip = new PathGrip(this, _initialPath);
            _grip.Attach(new Vector2(transform.position.x, transform.position.z));
        }

        private void Update()
        {
            _grip.Move(_speed * Time.deltaTime);
        }

        public void SetPosition(PathPosition position)
        {
            transform.SetPositionAndRotation(
                new Vector3(position.Position.x, transform.position.y, position.Position.y),
                Quaternion.LookRotation(new Vector3(position.Normal.X, 0, position.Normal.Y)));
        }

        public void UpdatePosition(PathPosition position)
        {
            transform.SetPositionAndRotation(
                new Vector3(position.Position.x, transform.position.y, position.Position.y),
                Quaternion.LookRotation(new Vector3(position.Normal.X, 0, position.Normal.Y)));
        }

        [ContextMenu("Connect to path")]
        private void ServiceConnectToPath()
        {
            _initialPath.Attach(this, new Vector2(transform.position.x, transform.position.z));
        }
    }
}