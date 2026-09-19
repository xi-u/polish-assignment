using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Circle : PrimitiveShape
{
    public const int SEGMENTS = 30;
    public const float DEGREES = 360;
    public const float INNER_RADIUS = 2f, OUTER_RADIUS = 2.5f;

    private float innerRadius, outerRadius, degrees;
    private int segments;
    public float InnerRadius => innerRadius;
    public float OuterRadius => outerRadius;
    public float Degrees => degrees;
    public int Segments => segments;

    public Circle(
        string name,        
        float innerRadius = INNER_RADIUS, 
        float outerRadius = OUTER_RADIUS,
        float degrees = DEGREES,
        int segments = SEGMENTS) : base(name)
    { 
        this.innerRadius = innerRadius;
        this.outerRadius = outerRadius;
        this.degrees = degrees;
        this.segments = segments;
    }

    public override Collider2D AddCollider(bool isTrigger = true)
    {
        CircleCollider2D circleCollider2D = GameObject.AddComponent<CircleCollider2D>();
        circleCollider2D.isTrigger = isTrigger;
        return circleCollider2D;
    }

    public override void CreatePrimitiveShape(Vector2[] vertices = null, ushort[] triangles = null)
    {
        if (vertices == null || triangles == null)
        {
            Utility.GenerateCirclePolygonSprite(segments, outerRadius, innerRadius, out vertices, out triangles, degrees);
        }

        Sprite sprite = Utility.GenerateSpritePolygon2D(vertices, triangles);
        SpriteRenderer spriteRenderer = GameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }
}