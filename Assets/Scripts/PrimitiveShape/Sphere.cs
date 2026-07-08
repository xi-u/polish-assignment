using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : PrimitiveShape
{
    public const int SEGMENTS = 20;
    public const float RADIUS = 0.3f;

    private int segments;
    private float radius;

    public Sphere(string name = "", int segments = SEGMENTS, float radius = RADIUS) : base(name)
    {
        this.segments = segments;
        this.radius = radius;
    }

    public override void CreatePrimitiveShape(Vector2[] vertices = null, ushort[] triangles = null)
    {
        if (vertices == null || triangles == null)
        {
            Utility.GenerateSpherePolygonSprite(segments, radius, out vertices, out triangles);
        }

        Sprite sprite = Utility.GenerateSpritePolygon2D(vertices, triangles);
        SpriteRenderer spriteRenderer = GameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }

    public override Collider2D AddCollider(bool isTrigger = true)
    {
        CircleCollider2D circleCollider2D = GameObject.AddComponent<CircleCollider2D>();
        circleCollider2D.isTrigger = isTrigger;
        return circleCollider2D;
    }
}
