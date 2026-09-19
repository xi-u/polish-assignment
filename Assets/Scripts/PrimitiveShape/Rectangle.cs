using System;
using UnityEngine;

public class Rectangle : PrimitiveShape
{
	public Rectangle(string name = "") : base(name)
	{
		
	}

    public override void CreatePrimitiveShape(Vector2[] vertices = null, ushort[] triangles = null)
    {
        if (vertices == null)
        {
            vertices = new Vector2[4]
            {
                new (0, 0),
                new (1, 1),
                new (1, 0),
                new (0, 1)
            };
        }

        if (triangles == null)
        {
            triangles = new ushort[6] { 0, 1, 2, 0, 3, 1 };
        }

        Sprite sprite = Utility.GenerateSpritePolygon2D(vertices, triangles);

        SpriteRenderer spriteRenderer = GameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }

    public override Collider2D AddCollider(bool isTrigger = true)
	{
		BoxCollider2D boxCollider2D = GameObject.AddComponent<BoxCollider2D>();
		boxCollider2D.isTrigger = isTrigger;
        return boxCollider2D;
	}
}
