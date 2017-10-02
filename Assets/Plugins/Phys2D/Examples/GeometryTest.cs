using System;
using UnityEngine;
using Phys2D;

namespace Phys2D
{
    public class GeometryTest : MonoBehaviour
    {
        [Serializable]
        private class AnimationDefinition
        {
            [Range(0.1f, 100f)]
            public float OffsetDuration = 10f;

            public Vector2 OffsetAmount = Vector2.one;

            public AnimationCurve OffsetCurve = null;

            [Range(-50f, 50f)]
            public float SpinSpeed;
        }

        [SerializeField]
        private ShapeConfig shapes;

        [SerializeField]
        private AnimationDefinition[] animations;

        private bool[] shapeCollisions;

        private void Awake()
        {
            Debug.Assert(this.shapes.Count > 0 && this.animations.Length > 0);
                
            this.shapeCollisions = new bool[this.shapes.Count];
        }

        private void Update()
        {
            this.UpdateAnimations(Time.smoothDeltaTime, Time.time);
            this.UpdateCollisions();
        }

        private void OnDrawGizmos()
        {
            #if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                return;
            }
            #endif

            for (var i = 0; i < this.shapes.Count; i++)
            {
                var colour = this.shapeCollisions[i] ? Color.red : Color.green;
                this.shapes[i].DrawGizmos(colour);
            }
        }

        private void UpdateAnimations(float deltaTime, float currentTime)
        {
            for (var i = 0; i < this.shapes.Count; i++)
            {
                var animIndex = i % this.animations.Length;
                var anim = this.animations[animIndex];
                var shape = this.shapes[i];

                var rotationDelta = deltaTime * anim.SpinSpeed;
                var newRotation = shape.Transform.Rotation + rotationDelta;

                var initPos = this.shapes.GetInitialPosition(i);
                var finalPos = initPos + anim.OffsetAmount;
                var offsetLerpT = currentTime / anim.OffsetDuration % 1f;
                offsetLerpT = anim.OffsetCurve.Evaluate(offsetLerpT);
                var newPosition = Vector2.Lerp(initPos, finalPos, offsetLerpT);

                shape.Transform.Set(newPosition, newRotation);
            }
        }

        // TODO: Copy to Phys2DSystem.IsAnythingIntersecting() and return true as soon as first collision detected 
        private void UpdateCollisions()
        {
            for (var i = 0; i < this.shapeCollisions.Length; i++)
            {
                this.shapeCollisions[i] = false;
            }
            
            for (var i1 = 0; i1 < this.shapes.Count; i1++)
            {
                var shape1 = this.shapes[i1];
                var isShape1Colliding = this.shapeCollisions[i1];

                for (var i2 = 0; i2 < this.shapes.Count; i2++)
                {
                    // Skip diagonal and one matrix half
                    if (i2 >= i1)
                    {
                        continue;
                    }
                    
                    var shape2 = this.shapes[i2];

                    var isShape2Colliding = this.shapeCollisions[i2];

                    if (isShape1Colliding && isShape2Colliding)
                    {
                        continue;
                    }

                    if (shape1.Intersects(shape2))
                    {
                        isShape1Colliding = true;
                        isShape2Colliding = true;
                    }

                    this.shapeCollisions[i2] = isShape2Colliding;
                }

                this.shapeCollisions[i1] = isShape1Colliding;
            }
        }
    }
}
