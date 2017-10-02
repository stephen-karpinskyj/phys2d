using System.Collections.Generic;
using UnityEngine;

namespace Phys2D
{
    public class PointSet : IEnumerable<Vector2>
    {
        private readonly List<Vector2> points;

        public int Count
        {
            get { return this.points.Count; }
        }

        public PointSet()
        {
            this.points = new List<Vector2>();
        }

        public PointSet(List<Vector2> points)
        {
            this.points = points;
        }

        public PointSet(float pointRotation, int numPoints, float radius)
        {
            this.points = new List<Vector2>();

            var p = (Vector2.up * radius).Rotate(pointRotation);
            var cornerAngle = 360f / numPoints;

            for (var i = 0; i < numPoints; i++)
            {
                p = p.Rotate(cornerAngle);
                this.points.Add(p);
            }
        }

        public void Clear()
        {
            this.points.Clear();
        }

        public void Replace(IEnumerable<Vector2> points)
        {
            Debug.Assert(points != null);

            this.Clear();

            this.points.AddRange(points);
        }

        public void Transform(Phys2DTransform transform)
        {
            Debug.Assert(transform != null);

            var shouldRotate = !Mathf.Approximately(transform.Rotation, 0f);
            var shouldOffset = transform.Position != Vector2.zero;

            for (var i = 0; i < this.points.Count; i++)
            {
                var newP = this.points[i];

                if (shouldRotate)
                {
                    newP = newP.Rotate(transform.Rotation);
                }

                if (shouldOffset)
                {
                    newP += transform.Position;
                }

                this.points[i] = newP;
            }
        }

        public AABB CalculateAABB()
        {
            var min = new Vector2(float.MinValue, float.MinValue);
            var max = new Vector2(float.MaxValue, float.MaxValue);

            foreach (var p in this.points)
            {
                min.x = Mathf.Max(min.x, p.x);
                max.x = Mathf.Min(max.x, p.x);
                min.y = Mathf.Max(min.y, p.y);
                max.y = Mathf.Min(max.y, p.y);
            }

            return AABB.FromMinMax(min, max);
        }


        #region IEnumerable


        public Vector2 this[int index]  
        {  
            get { return this.points[index]; }  
            set { this.points[index] = value; }  
        } 

        public IEnumerator<Vector2> GetEnumerator()
        {
            return this.points.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }


        #endregion
    }
}
