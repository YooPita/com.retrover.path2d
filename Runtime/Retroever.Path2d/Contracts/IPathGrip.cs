using UnityEngine;

namespace Retroever.Path2d
{
    public interface IPathGrip
    {
        public void Attach(Vector2 position);
        public void Move(float movement);
    }
}