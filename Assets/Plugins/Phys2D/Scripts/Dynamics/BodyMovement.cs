using UnityEngine;

namespace Phys2D
{
    public class BodyMovement
    {
        public Vector2 Heading;
        public float Distance;

        public float AngleOffset;

        public void Reset()
        {
            this.Heading = Vector2.up;
            this.Distance = 0f;

            this.AngleOffset = 0f;
        }
    }
}
