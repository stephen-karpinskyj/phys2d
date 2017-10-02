using UnityEngine;

namespace Phys2D
{
    public struct AABB
    {
        public readonly Vector2 Centre;
        public readonly Vector2 Size;

        public readonly Vector2 Extents;
        public readonly Vector2 Min;
        public readonly Vector2 Max;

        public AABB(Vector2 centre, Vector2 size)
        {
            Debug.Assert(size.x > 0f && size.y > 0f);
            
            this.Centre = centre;
            this.Size = size;

            this.Extents = size / 2;
            this.Min = centre - size;
            this.Max = centre + size;
        }

        public static AABB FromMinMax(Vector2 min, Vector2 max)
        {
            var size = new Vector2(Mathf.Abs(min.x - max.x), Mathf.Abs(min.y - max.y));
            var centre = new Vector2(Mathf.Min(min.x, max.x), Mathf.Min(min.y, max.y)) + (size / 2);

            return new AABB(centre, size);
        }

        public bool Intersects(AABB other)
        {
            if (Mathf.Abs(this.Centre.x - other.Centre.x) > (this.Extents.x + other.Extents.x) ||
                Mathf.Abs(this.Centre.y - other.Centre.y) > (this.Extents.y + other.Extents.y))
            {
                return false;
            }

            return true;
        }

        public bool Contains(Vector2 point)
        {
            if (point.x < this.Min.x || point.x > this.Max.x ||
                point.y < this.Min.y || point.y > this.Max.y)
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is AABB && this == (AABB)obj;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;

                hash += 23 * this.Centre.GetHashCode();
                hash += 23 * this.Size.GetHashCode();

                return hash;
            }
        }

        public static bool operator ==(AABB a, AABB b)
        {
            return a.Centre == b.Centre && a.Size == b.Size;
        }

        public static bool operator !=(AABB a, AABB b)
        {
            return !(a == b);
        }
    }
}
