using System.Collections.Generic;
using UnityEngine;

namespace Phys2D
{
    public sealed class Polyline : Shape
    {
        private readonly PointSet localPoints;
        private readonly List<LineSegment> localSegments;

        private readonly PointSet worldPoints;
        private readonly List<LineSegment> worldSegments;

        public Polyline(Phys2DTransform transform, PointSet localPoints)
            : base(transform)
        {
            Debug.Assert(localPoints != null && localPoints.Count >= 2);

            this.localPoints = localPoints;
            this.localSegments = new List<LineSegment>();

            FillMultiEdgeSegments(this.localPoints, this.localSegments);

            this.worldPoints = new PointSet();
            this.worldSegments = new List<LineSegment>();

            this.HandleTransformMove();
        }

        public Polyline(Phys2DTransform transform, List<Vector2> localPoints)
            : this(transform, new PointSet(localPoints))
        {
        }

        protected override void HandleTransformMove()
        {
            base.HandleTransformMove();

            this.worldPoints.Replace(this.localPoints);
            this.worldPoints.Transform(this.Transform);
            this.AABB = this.worldPoints.CalculateAABB();

            FillMultiEdgeSegments(this.worldPoints, this.worldSegments);
        }

        public override bool Intersects(Shape other)
        {
            var otherCircle = other as Circle;
            if (otherCircle != null)
            {
                return Polyline.Intersects(this, otherCircle);
            }

            var otherPolygon = other as Polygon;
            if (otherPolygon != null)
            {
                return Polyline.Intersects(this, otherPolygon);
            }

            var otherPolyline = other as Polyline;
            if (otherPolyline != null)
            {
                return Polyline.Intersects(this, otherPolyline);
            }

            throw new System.NotImplementedException();
        }

        public override void DrawGizmos(Color mainColour)
        {
            base.DrawGizmos(mainColour);

            Gizmos.color = mainColour;
            foreach (var s in this.worldSegments)
            {
                Gizmos.DrawLine(s.P1, s.P2);
            }
        }

        private static void FillMultiEdgeSegments(PointSet points, ICollection<LineSegment> segments)
        {
            segments.Clear();

            for (var i = 0; i < points.Count - 1; i++)
            {
                segments.Add(new LineSegment(points[i], points[i + 1]));
            }
        }

        public static bool Intersects(Polyline a, Polyline b)
        {
            if (!a.AABB.Intersects(b.AABB))
            {
                return false;
            }

            foreach (var aSegment in a.worldSegments)
            {
                foreach (var bSegment in b.worldSegments)
                {
                    if (aSegment.Intersects(bSegment))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool Intersects(Polyline a, Circle b)
        {
            if (!a.AABB.Intersects(b.AABB))
            {
                return false;
            }

            foreach (var aSegment in a.worldSegments)
            {
                if (aSegment.Distance(b.Transform.Position) < b.Radius)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Intersects(Polyline a, Polygon b)
        {
            if (!a.AABB.Intersects(b.AABB))
            {
                return false;
            }

            foreach (var p in a.worldPoints)
            {
                if (b.Contains(p))
                {
                    return true;
                }
            }

            foreach (var s in a.worldSegments)
            {
                if (b.Intersects(s))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
