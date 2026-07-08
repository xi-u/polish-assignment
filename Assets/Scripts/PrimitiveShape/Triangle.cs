using System;
using UnityEngine;

public class Triangle : PrimitiveShape
{
    private Vector2[] vertices;
	public Triangle(string name = "") : base(name)
	{
		
	}

    public override void CreatePrimitiveShape(Vector2[] vertices = null, ushort[] triangles = null)
    {
        if (vertices == null)
        {
            vertices = new Vector2[3]
            {
                new (0, 0),
                new (1, 0),
                new (0.5f, 1)
            };
        }
        this.vertices = vertices;

        if (triangles == null)
        {
            triangles = new ushort[3] { 0, 1, 2 };
        }

        Sprite sprite = Utility.GenerateSpritePolygon2D(vertices, triangles);

        SpriteRenderer spriteRenderer = GameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }

    public override Collider2D AddCollider(bool isTrigger = true)
	{
		//There is no triangle collider so we create our own with specific coordinates
		//These coordinates are the precise shape of the triangle 
		PolygonCollider2D polygonCollider2D = GameObject.AddComponent<PolygonCollider2D>();
        polygonCollider2D.points = this.vertices;
		polygonCollider2D.isTrigger = isTrigger;
        return polygonCollider2D;
	}   
}