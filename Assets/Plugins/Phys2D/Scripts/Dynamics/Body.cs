using UnityEngine;

namespace Phys2D
{
    public class Body
    {
        public readonly Shape Shape;
        public readonly float Mass;

        public BodyMovement Target;

        private Phys2DTransform initialTransform;
        private Phys2DTransform lastTransform;

        public Body(Shape shape, float mass)
        {
            Debug.Assert(shape != null);
            Debug.Assert(mass > 0f);

            this.initialTransform = shape.Transform.Clone();

            this.Shape = shape;
            this.Mass = mass;

            this.Reset();
        }

        public void Reset()
        {
            this.Shape.Transform.Set(this.initialTransform);

            this.lastTransform = this.Shape.Transform.Clone();
        }

        public Phys2DTransform LerpedTransform(float t)
        {
            return Phys2DTransform.Lerp(this.lastTransform, this.Shape.Transform, t);
        }

        public void DrawGizmos(Color mainColor)
        {
            this.Shape.DrawGizmos(mainColor);
        }
    }
}
