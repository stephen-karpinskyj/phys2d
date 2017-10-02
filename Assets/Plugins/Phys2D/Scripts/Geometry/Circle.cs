using UnityEngine;

namespace Phys2D
{
    public sealed class Circle : Shape
    {
        public readonly float Radius;

        public Circle(Phys2DTransform transform, float radius)
            : base(transform)
        {
            Debug.Assert(radius > 0f);

            this.Radius = radius;

            this.HandleTransformMove();
        }

        public override bool Intersects(Shape other)
        {
            var otherCircle = other as Circle;
            if (otherCircle != null)
            {
                return Circle.Intersects(this, otherCircle);
            }

            var otherPolygon = other as Polygon;
            if (otherPolygon != null)
            {
                return Circle.Intersects(this, otherPolygon);
            }

            var otherPolyline = other as Polyline;
            if (otherPolyline != null)
            {
                return Circle.Intersects(this, otherPolyline);
            }

            throw new System.NotImplementedException();
        }

        protected override void HandleTransformMove()
        {
            base.HandleTransformMove();

            this.AABB = new AABB(this.Transform.Position, Vector2.one * this.Radius * 2);
        }

        public override void DrawGizmos(Color mainColour)
        {
            base.DrawGizmos(mainColour);

            Gizmos.color = mainColour;
            Gizmos.DrawWireSphere(this.Transform.Position, this.Radius);
        }

        public static bool Intersects(Circle a, Circle b)
        {
            if (!a.AABB.Intersects(b.AABB))
            {
                return false;
            }

            var dist = Vector2.Distance(a.Transform.Position, b.Transform.Position);
            return dist < (a.Radius + b.Radius);
        }

        public static bool Intersects(Circle a, Polygon b)
        {
            return Polygon.Intersects(b, a);
        }

        public static bool Intersects(Circle a, Polyline b)
        {
            return Polyline.Intersects(b, a);
        }
    }
}