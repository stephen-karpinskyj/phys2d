using System.Collections.Generic;
using UnityEngine;

namespace Phys2D
{
    public class Phys2DSystem
    {
        private readonly Phys2DSettings settings;

        private readonly List<Shape> shapes;
        private readonly List<Body> bodies;

        public Phys2DSystem(Phys2DSettings settings)
        {
            this.settings = settings;

            this.shapes = new List<Shape>();
            this.bodies = new List<Body>();
        }

        public void AddShape(Shape shape)
        {
            Debug.Assert(shape != null && !this.shapes.Contains(shape));

            this.shapes.Add(shape);
        }

        public void AddDynamicBody(Body body)
        {
            Debug.Assert(!this.bodies.Contains(body));

            this.bodies.Add(body);
        }

        public void SolveBodyTargets(float deltaTime)
        {
            foreach (var b in this.bodies)
            {
                if (b.Target == null)
                {
                    continue;
                }

                var trans = b.Shape.Transform;

                var newPosition = trans.Position + b.Target.Heading * b.Target.Distance;
                var newRotation = trans.Rotation + b.Target.AngleOffset;

                // TODO: Move forward one step, check collisions, if collision, iterate for number of iterations

                trans.Set(newPosition, newRotation);

                b.Target.Reset();
            }
        }

        public void DrawAllGizmos()
        {
            foreach (var s in this.shapes)
            {
                s.DrawGizmos(Color.green);
            }
        }
    }
}
