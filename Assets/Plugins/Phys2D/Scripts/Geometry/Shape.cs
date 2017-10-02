using UnityEngine;

namespace Phys2D
{
    public abstract class Shape
    {
        public readonly Phys2DTransform Transform;

        public AABB AABB { get; protected set; }

        protected Shape(Phys2DTransform transform)
        {
            Debug.Assert(transform != null);

            this.Transform = transform;
            this.Transform.OnMove += this.HandleTransformMove;
        }

        protected virtual void HandleTransformMove() { }

        public abstract bool Intersects(Shape other);

        public virtual void DrawGizmos(Color mainColour)
        {
            const float Size = 0.1f;

            Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
            Gizmos.DrawWireSphere(this.Transform.Position, Size);

            Gizmos.color = new Color(1f, 1f, 0f, 0.35f);
            var to = this.Transform.Position + MathUtility.ToHeading(this.Transform.Rotation) * Size;
            Gizmos.DrawLine(this.Transform.Position, to);

            Gizmos.color = new Color(0f, 0f, 1f, 0.15f);
            Gizmos.DrawWireCube(this.AABB.Centre, this.AABB.Size);
        }
    }
}
