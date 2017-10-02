using System.Collections.Generic;
using UnityEngine;

namespace Phys2D
{
    /// <remarks>While not explicitly asserted, behaviour will only be correct if this is a concave polygon.</remarks>
    public sealed class Polygon : Shape
    {
        private readonly PointSet localPoints;
        private readonly List<LineSegment> localEdges;

        private readonly PointSet worldPoints;
        private readonly List<LineSegment> worldEdges;

        public Polygon(Phys2DTransform transform, PointSet localPoints)
            : base(transform)
        {
            Debug.Assert(localPoints != null && localPoints.Count >= 3);

            this.localPoints = localPoints;
            this.localEdges = new List<LineSegment>();

            FillPolygonEdges(this.localPoints, this.localEdges);
            // TODO: Assert whether any edges intersect

            this.worldPoints = new PointSet();
            this.worldEdges = new List<LineSegment>();

            this.HandleTransformMove();
        }

        public Polygon(Phys2DTransform transform, List<Vector2> localPoints)
            : this(transform, new PointSet(localPoints))
        {
        }

        protected override void HandleTransformMove()
        {
            base.HandleTransformMove();

            this.worldPoints.Replace(this.localPoints);
            this.worldPoints.Transform(this.Transform);
            this.AABB = this.worldPoints.CalculateAABB();

            FillPolygonEdges(this.worldPoints, this.worldEdges);
        }

        public override bool Intersects(Shape other)
        {
            var otherCircle = other as Circle;
            if (otherCircle != null)
            {
                return Polygon.Intersects(this, otherCircle);
            }

            var otherPolygon = other as Polygon;
            if (otherPolygon != null)
            {
                return Polygon.Intersects(this, otherPolygon);
            }

            var otherPolyline = other as Polyline;
            if (otherPolyline != null)
            {
                return Polygon.Intersects(this, otherPolyline);
            }

            throw new System.NotImplementedException();
        }

        public override void DrawGizmos(Color mainColour)
        {
            base.DrawGizmos(mainColour);

            Gizmos.color = mainColour;
            foreach (var e in this.worldEdges)
            {
                Gizmos.DrawLine(e.P1, e.P2);
            }
        }

        public bool Intersects(LineSegment segment)
        {
            if (!this.AABB.Contains(segment.P1) && !this.AABB.Contains(segment.P2))
            {
                return false;
            }

            return this.CountIntersections(segment) > 0;
        }

        /// <remarks>Based on: http://stackoverflow.com/questions/217578/how-can-i-determine-whether-a-2d-point-is-within-a-polygon</remarks>
        public bool Contains(Vector2 point)
        {
            if (!this.AABB.Contains(point))
            {
                return false;
            }

            var raySegment = new LineSegment(point, new Vector2(this.AABB.Max.x + Mathf.Epsilon, point.y));

            return this.CountIntersections(raySegment) % 2 == 1;
        }

        private int CountIntersections(LineSegment segment)
        {
            var numIntersections = 0;

            foreach (var e in this.worldEdges)
            {
                if (e.Intersects(segment))
                {
                    numIntersections++;
                }
            }

            return numIntersections;
        }

        private static void FillPolygonEdges(PointSet points, ICollection<LineSegment> edges)
        {
            edges.Clear();

            for (var i = 0; i < points.Count; i++)
            {                
                var start = points[i];
                var end = points[(i + 1) % points.Count];

                edges.Add(new LineSegment(start, end));
            }
        }

        /// <remarks>Source: http://www.bitlush.com/posts/circle-vs-polygon-collision-detection-in-c-sharp</remarks>
        public static bool Intersects(Polygon a, Circle b)
        {
            if (!a.AABB.Intersects(b.AABB))
            {
                return false;
            }

            var points = a.worldPoints;
            var radiusSquared = b.Radius * b.Radius;
            var vertex = points[points.Count - 1];
            var circlePosition = b.Transform.Position;

            var nearestDistance = float.MaxValue;
            var nearestIsInside = false;
            var nearestVertex = -1;
            var lastIsInside = false;

            for (int i = 0; i < points.Count; i++)
            {
                var nextVertex = points[i];
                var axis = circlePosition - vertex;

                var distance = axis.sqrMagnitude - radiusSquared;

                if (distance <= 0)
                {
                    return true;
                }

                var isInside = false;
                var edge = nextVertex - vertex;

                float edgeLengthSquared = edge.sqrMagnitude;

                if (!Mathf.Approximately(edgeLengthSquared, 0f))
                {
                    float dot = Vector2.Dot(edge, axis);

                    if (dot >= 0 && dot <= edgeLengthSquared)
                    {
                        var projection = vertex + (dot / edgeLengthSquared) * edge;

                        axis = projection - circlePosition;

                        if (axis.sqrMagnitude <= radiusSquared)
                        {
                            return true;
                        }
                        else
                        {
                            if (edge.x > 0)
                            {
                                if (axis.y > 0)
                                {
                                    return false;
                                }
                            }
                            else if (edge.x < 0)
                            {
                                if (axis.y < 0)
                                {
                                    return false;
                                }
                            }
                            else if (edge.y > 0)
                            {
                                if (axis.x < 0)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (axis.x > 0)
                                {
                                    return false;
                                }
                            }

                            isInside = true;
                        }
                    }
                }

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestIsInside = isInside || lastIsInside;
                    nearestVertex = i;
                }

                vertex = nextVertex;
                lastIsInside = isInside;
            }

            if (nearestVertex == 0)
            {
                return nearestIsInside || lastIsInside;
            }

            return nearestIsInside;
        }

        public static bool Intersects(Polygon a, Polygon b)
        {
            if (!a.AABB.Intersects(b.AABB))
            {
                return false;
            }

            if (FindSeparatingAxis(a.worldPoints, b.worldPoints))
            {
                return false;
            }

            if (FindSeparatingAxis(b.worldPoints, a.worldPoints))
            {
                return false;
            }

            return true;
        }

        public static bool Intersects(Polygon a, Polyline b)
        {
            return Polyline.Intersects(b, a);
        }

        /// <remarks>Source: http://gamemath.com/2011/09/detecting-whether-two-convex-polygons-overlap/</remarks>
        private static bool FindSeparatingAxis(PointSet a, PointSet b)
        {
            var prev = a.Count - 1;

            for (var i = 0; i < a.Count; ++i)
            {
                // Get edge vector
                var edge = a[i] - a[prev];

                // Rotate vector 90 degrees (doesn't matter which way) to get
                // candidate separating axis.
                var axis = new Vector2(edge.y, -edge.x);

                // Gather extents of both polygons projected onto this axis
                float aMin, aMax, bMin, bMax;
                GatherPolygonProjectionExtents(a, axis, out aMin, out aMax);
                GatherPolygonProjectionExtents(b, axis, out bMin, out bMax);

                // Is this a separating axis?
                if (aMax < bMin)
                {
                    return true;
                }
                if (bMax < aMin)
                {
                    return true;
                }

                prev = i;
            }

            // No separating axis
            return false;
        }

        /// <remarks>Source: http://gamemath.com/2011/09/detecting-whether-two-convex-polygons-overlap/</remarks>
        private static void GatherPolygonProjectionExtents(PointSet points, Vector2 axis, out float outMin, out float outMax)
        {
            // Initialize extents to a single point, the first vertex
            outMin = outMax = Vector2.Dot(axis, points[0]);

            // Now scan all the rest, growing extents to include them
            foreach (var p in points)
            {
                var d = Vector2.Dot(axis, p);

                if (d < outMin)
                {
                    outMin = d;
                }
                else if (d > outMax)
                {
                    outMax = d;
                }
            }
        }
    }
}
