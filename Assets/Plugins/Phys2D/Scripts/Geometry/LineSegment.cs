using UnityEngine;

namespace Phys2D
{
    public struct LineSegment
    {
        public readonly Vector2 P1;
        public readonly Vector2 P2;

        public LineSegment(Vector2 start, Vector2 end)
        {
            this.P1 = start;
            this.P2 = end;
        }

        /// <remarks>Based on: http://www.codeproject.com/Tips/862988/Find-the-Intersection-Point-of-Two-Line-Segments</remarks>
        public bool Intersects(LineSegment other)
        {
            var p = this.P1;
            var p2 = this.P2;
            var q = other.P1;
            var q2 = other.P2;
            
            var r = p2 - p;
            var s = q2 - q;
            var rXs = r.Cross(s);
            var qpXr = (q - p).Cross(r);

            // If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
            if (Mathf.Approximately(rXs, 0f) && Mathf.Approximately(qpXr, 0f))
            {
                // 1. If either  0 <= (q - p) * r <= r * r or 0 <= (p - q) * s <= * s
                // then the two lines are overlapping,
                var qpMr = (q - p).Multiply(r);
                var pqMs = (p - q).Multiply(s);
                var rMr = r.Multiply(r);
                var sMs = s.Multiply(s);
                if ((0 <= qpMr.x && 0 <= qpMr.y && qpMr.x <= rMr.x && qpMr.y <= rMr.y) ||
                    (0 <= pqMs.x && 0 <= pqMs.y && pqMs.x <= sMs.x && pqMs.y <= sMs.y))
                {
                    return true;
                }

                // 2. If neither 0 <= (q - p) * r = r * r nor 0 <= (p - q) * s <= s * s
                // then the two lines are collinear but disjoint.
                // No need to implement this expression, as it follows from the expression above.
                return false;
            }

            // 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
            if (Mathf.Approximately(rXs, 0f) && !Mathf.Approximately(qpXr, 0f))
            {
                return false;
            }

            // t = (q - p) x s / (r x s)
            var qpXs = (q - p).Cross(s);
            var t = qpXs / rXs;

            // u = (q - p) x r / (r x s)
            var u = qpXr / rXs;

            // 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1
            // the two line segments meet at the point p + t r = q + u s.
            if (!Mathf.Approximately(rXs, 0f) && (0 <= t && t <= 1) && (0 <= u && u <= 1))
            {
                // An intersection was found.
                return true;
            }

            // 5. Otherwise, the two line segments are not parallel but do not intersect.
            return false;
        }

        /// <remarks>Based on: http://forum.unity3d.com/threads/how-do-i-find-the-closest-point-on-a-line.340058/</remarks>
        public Vector2 FindNearestPoint(Vector2 point)
        {
            var line = (this.P2 - this.P1);
            var len = line.magnitude;
            line.Normalize();

            var v = point - this.P1;
            var d = Vector3.Dot(v, line);
            d = Mathf.Clamp(d, 0f, len);
            return this.P1 + line * d;
        }

        public float Distance(Vector2 point)
        {
            return Vector2.Distance(this.FindNearestPoint(point), point);
        }

        public override bool Equals(object obj)
        {
            return obj is LineSegment && this == (LineSegment)obj;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;

                hash += 23 * this.P1.GetHashCode();
                hash += 23 * this.P2.GetHashCode();

                return hash;
            }
        }

        public static bool operator ==(LineSegment a, LineSegment b)
        {
            return a.P1 == b.P1 && a.P2 == b.P2;
        }

        public static bool operator !=(LineSegment a, LineSegment b)
        {
            return !(a == b);
        }
    }
}
