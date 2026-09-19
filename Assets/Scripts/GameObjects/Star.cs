
using UnityEngine;

public class Star : Sphere
{
    public Star(string name = "") : base(name)
    {
        Vector2[] vertices;
        ushort[] triangles;
        Utility.GenerateSpherePolygonSprite(SEGMENTS, Random.Range(0.01f, 0.05f), out vertices, out triangles);
        CreatePrimitiveShape(vertices, triangles);

        GameObject.GetComponent<SpriteRenderer>().sortingOrder = -2;
    }
}
