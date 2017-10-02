using UnityEngine;

namespace Phys2D
{
    public class Phys2DTransform
    {
        public Vector2 Position { get; private set; }
        public float Rotation { get; private set; }

        public delegate void MoveHandler();
        public event MoveHandler OnMove = delegate { };

        public Phys2DTransform()
        {
            this.Reset();
        }

        public Phys2DTransform(Vector2 position, float rotation)
        {
            this.Set(position, rotation);
        }

        public Phys2DTransform Clone()
        {
            return new Phys2DTransform(this.Position, this.Rotation);
        }

        public void Reset()
        {
            this.Set(Vector2.zero, 0f);
        }

        public void Set(Vector2 position)
        {
            this.Position = position;
            this.OnMove();
        }

        public void Set(float rotation)
        {
            this.Rotation = rotation;
            this.OnMove();
        }

        public void Set(Vector2 position, float rotation)
        {
            this.Position = position;
            this.Rotation = rotation;
            this.OnMove();
        }

        public void Set(Phys2DTransform transform)
        {
            this.Set(transform.Position, transform.Rotation);
        }

        public static Phys2DTransform Lerp(Phys2DTransform a, Phys2DTransform b, float t)
        {
            var position = Vector2.Lerp(a.Position, b.Position, t);
            var rotation = Mathf.Lerp(a.Rotation, b.Rotation, t);

            return new Phys2DTransform(position, rotation);
        }
    }
}
