using System;
using UnityEngine;
using Phys2D;

public class DynamicsTest : MonoBehaviour
{
    [Serializable]
    private class BodyDefinition
    {
        [Range(0.001f, 100f)]
        public float Mass = 1f;
    }

    [SerializeField, Range(0.0001f, 1f)]
    private float timestep = 1f / 60;

    [SerializeField]
    private ShapeConfig shapes;

    [SerializeField]
    private BodyDefinition[] bodyDefinitions;

    [SerializeField]
    private Phys2DSettings settings;

    private Phys2DSystem physics;
    private Body[] bodies;

    private float lastStepTime;

    private void Awake()
    {
        Debug.Assert(this.shapes.Count > 0 && this.bodyDefinitions.Length > 0, this);
        Debug.Assert(this.settings, this);

        this.physics = new Phys2DSystem(this.settings);
        this.bodies = new Body[this.shapes.Count];
        
        for (var i = 0; i < this.shapes.Count; i++)
        {
            var bodyDefinition = this.bodyDefinitions[i % this.bodyDefinitions.Length];
            var body = new Body(this.shapes[i], bodyDefinition.Mass);
            body.Target = new BodyMovement();
            this.bodies[i] = body;
            this.physics.AddDynamicBody(body);
        }
    }

    private void Update()
    {   
        var time = Time.time;

        while (lastStepTime < time)
        {
            foreach (var b in this.bodies)
            {
                b.Target.Heading = Vector2.right;
                b.Target.Distance = 0.01f * lastStepTime;
                b.Target.AngleOffset = 0.1f * lastStepTime;
            }

            this.physics.SolveBodyTargets(this.timestep);

            this.lastStepTime += this.timestep;
        }
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
            this.bodies[i].DrawGizmos(Color.green);
        }
    }
}
