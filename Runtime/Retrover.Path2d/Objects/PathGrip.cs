using UnityEngine;

namespace Retrover.Path2d
{
    public class PathGrip : IPathGrip, IPathClient
    {
        public PathGrip(IPathClient client, IPath path)
        {
            _client = client;
            _path = path;
        }

        private IPathClient _client;
        private IPath _path;
        private PathPosition _pathPosition;

        public void UpdatePosition(PathPosition position)
        {
            _pathPosition = position;
            _client.UpdatePosition(position);
        }

        public void Attach(Vector2 position)
        {
            _path.Attach(this, position);
        }

        public void Move(float movement)
        {
            _path.Move(this, _pathPosition.Length + movement);
        }
    }
}