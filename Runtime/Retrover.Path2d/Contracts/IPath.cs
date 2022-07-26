using UnityEngine;

namespace Retrover.Path2d
{
    public interface IPath
    {
        public void Attach(IPathClient client, Vector2 position);
        public void Move(IPathClient client, float position);
    }
}