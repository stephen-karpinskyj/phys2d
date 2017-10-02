using System;
using System.Collections.Generic;
using UnityEngine;
using Phys2D;

[Serializable]
public class ShapeConfig : IEnumerable<Shape>
{
    [Serializable]
    private abstract class ShapeDefinition
    {
        public Vector2 Position = Vector2.zero;

        [Range(-180f, 180f)]
        public float Rotation;
    }

    [Serializable]
    private class CircleDefinition : ShapeDefinition
    {
        [Range(0.001f, 100f)]
        public float Radius = 1f;
    }

    [Serializable]
    private class PointSetDefinition : ShapeDefinition
    {
        [Range(-180f, 180f)]
        public float PointRotation;

        [Range(2, 32)]
        public int NumPoints = 3;

        [Range(0.001f, 100f)]
        public float Radius = 1f;
    }

    [SerializeField]
    private CircleDefinition[] circles;

    [SerializeField]
    private PointSetDefinition[] polygons;

    [SerializeField]
    private PointSetDefinition[] polylines;

    private readonly List<Shape> shapes = new List<Shape>();
    private readonly List<ShapeDefinition> definitions = new List<ShapeDefinition>();

    public int Count
    {
        get
        {
            this.CheckInitialisation();
            return this.shapes.Count;
        }
    }

    public Shape this[int index]
    {
        get
        {
            this.CheckInitialisation();
            return this.shapes[index];
        }

        set
        {
            this.CheckInitialisation();
            this.shapes[index] = value;
        }
    }

    private bool isInitialised;

    private void CheckInitialisation()
    {
        if (this.isInitialised)
        {
            return;
        }

        this.isInitialised = true;
        
        foreach (var c in this.circles)
        {
            var trans = new Phys2DTransform(c.Position, c.Rotation);
            var newCircle = new Circle(trans, c.Radius);
            this.shapes.Add(newCircle);
            this.definitions.Add(c);
        }

        foreach (var p in this.polygons)
        {
            var trans = new Phys2DTransform(p.Position, p.Rotation);
            var points = new PointSet(p.PointRotation, p.NumPoints, p.Radius);
            var newPolygon = new Polygon(trans, points);
            this.shapes.Add(newPolygon);
            this.definitions.Add(p);
        }

        foreach (var p in this.polylines)
        {
            var trans = new Phys2DTransform(p.Position, p.Rotation);
            var points = new PointSet(p.PointRotation, p.NumPoints, p.Radius);
            var newPolyline = new Polyline(trans, points);
            this.shapes.Add(newPolyline);
            this.definitions.Add(p);
        }
    }

    public Vector2 GetInitialPosition(int shapeIndex)
    {
        this.CheckInitialisation();
        return this.definitions[shapeIndex].Position;
    }

    public IEnumerator<Shape> GetEnumerator()
    {
        this.CheckInitialisation();
        return this.shapes.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
