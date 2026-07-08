using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public struct PolygonInformation
{
	public Vector2[] Vertices { get; set; }
	public ushort[] triangles { get; set; }
}

public static class Utility
{
    public static Color32 PURPLE = new Color32(143, 0, 254, 255);
    public static Color32 DARK_GREEN = new Color32(1, 50, 32, 255);
    public static Color32 LIGHT_GREEN = new Color32(144, 238, 144, 255);
    public static Color32 DEEP_ORANGE = new Color32(204, 102, 0, 255);
    public static Color32 RED_VIOLET = new Color32(199, 21, 133, 255);

    public static Sprite GenerateSpritePolygon2D(Vector2[] vertices, ushort[] triangles)
	{
		//convert coordinates to local space
		float smallestX = Mathf.Infinity, largestX = -Mathf.Infinity;
		float smallestY = Mathf.Infinity, largestY = -Mathf.Infinity;
		foreach (var vertex in vertices)
		{
			if (vertex.x > largestX)
				largestX = vertex.x;
			if (vertex.x < smallestX)
				smallestX = vertex.x;

			if (vertex.y > largestY)
				largestY = vertex.y;
			if (vertex.y < smallestY)
				smallestY = vertex.y;
		}

		// calculate pixel count for texture and sprite size.
		var pixelCountX = Mathf.CeilToInt((largestX - smallestX));
		var pixelCountY = Mathf.CeilToInt((largestY - smallestY));

		var texture = new Texture2D(pixelCountX, pixelCountY); // create a texture larger than your maximum polygon size

		var cols = new Color[texture.width * texture.height];
		Array.Fill(cols, Color.white);

		texture.SetPixels(cols);
		texture.Apply();
		
		var sprite = Sprite.Create(texture, new Rect(0, 0, pixelCountX, pixelCountY), new Vector2(0.5f, 0.5f), 1); //create a sprite with the texture we just created and colored in
		//convert coordinates to local space
		float lx = Mathf.Infinity, ly = Mathf.Infinity;
		foreach (var vertex in vertices)
		{
			if (vertex.x < lx)
				lx = vertex.x;
			if (vertex.y < ly)
				ly = vertex.y;
		}

		var localV = new Vector2[vertices.Length];
		for (var i = 0; i < vertices.Length; i++)
		{
			localV[i] = vertices[i] - new Vector2(lx, ly);
		}

		// https://docs.unity3d.com/ScriptReference/Sprite.OverrideGeometry.html
		// The size of the triangle array must always be a multiple of 3.
		// The vertices connected to the triangle can be shared by simply indexing into the same vertex.
		sprite.OverrideGeometry(localV, triangles); // set the vertices and triangles

		return sprite;
	}

    public static void GenerateSpherePolygonSprite(int segments, float radius, out Vector2[] vertices, out ushort[] triangles, float degrees = 360)
    {
        int numVertices = Mathf.CeilToInt((segments * degrees) / 360) + 1;
        int numTriangles = numVertices - 1;

        vertices = new Vector2[numVertices];
        triangles = new ushort[numTriangles * 3];

        vertices[0] = Vector2.zero; // Center vertex

        // Create the sphere vertices
        float angleStep = degrees * Mathf.Deg2Rad / numTriangles;
        for (int i = 1; i < numVertices; i++)
        {
            float angle = (i - 1) * angleStep;
            vertices[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        }

        // Create the triangles
        for (int i = 0; i < numTriangles; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = (ushort)(i + 1);
            triangles[i * 3 + 2] = (ushort)((i + 2 >= numVertices) ? 1 : (i + 2));
        }
    }

    public static void GenerateCirclePolygonSprite(int segments, float outerRadius, float innerRadius, out Vector2[] vertices, out ushort[] triangles, float degrees = 360)
    {
        vertices = new Vector2[(segments + 1) * 2];
        triangles = new ushort[segments * 6];

        float angleStep = degrees * Mathf.Deg2Rad / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep;
            vertices[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * outerRadius;
            vertices[segments + 1 + i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * innerRadius;
        }

        for (int i = 0; i < segments; i++)
        {
            triangles[i * 6] = (ushort)i;
            triangles[i * 6 + 1] = (ushort)((i + 1) % (segments + 1));
            triangles[i * 6 + 2] = (ushort)(segments + 1 + i);

            triangles[i * 6 + 3] = (ushort)(segments + 1 + i);
            triangles[i * 6 + 4] = (ushort)((i + 1) % (segments + 1));
            triangles[i * 6 + 5] = (ushort)(segments + 2 + i);
        }
    }
    
    public static Vector2 GetRandomPositionOutsideRect(Rect rect, float margin)
    {
        float x, y;
        x = Random.Range(rect.xMin - margin, rect.xMax + margin);
        y = Random.Range(rect.yMin - margin, rect.yMax + margin);

        if (x > rect.xMin && x < rect.xMax)
        {
            if (Random.value > 0.5f)
                x = rect.xMin - margin;
            else
                x = rect.xMax + margin;
        }

        if (y > rect.yMin && y < rect.yMax)
        {
            if (Random.value > 0.5f)
                y = rect.yMin - margin;
            else
                y = rect.yMax + margin;
        }

        return new Vector2(x, y);
    }

    public static Vector2 GetRandomPositionInsideRect(Rect rect)
    {
        float x, y;
        x = Random.Range(rect.xMin, rect.xMax);
        y = Random.Range(rect.yMin, rect.yMax);

        return new Vector2(x, y);
    }

    public static void Rotate(this Transform transform, float angle)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = rotation;
    }

    public static void RotateSlerp(this Transform transform, float angle, float t)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * t);
    }

    public static T PopLastItem<T>(this LinkedList<T> linkedList)
    {
        T item = linkedList.Last.Value;
        linkedList.RemoveLast();
        return item;
    }

    public static T PopFirstItem<T>(this LinkedList<T> linkedList)
    {
        T item = linkedList.First.Value;
        linkedList.RemoveFirst();
        return item;
    }

    public static bool IsObjectVisible(this Camera cam, SpriteRenderer renderer)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }

    public static bool IsFullyInView(this Camera camera, Collider2D collider)
    {
        Bounds bounds = collider.bounds;
        Vector3[] corners = new Vector3[4];
        corners[0] = camera.WorldToViewportPoint(new Vector3(bounds.min.x, bounds.min.y, 0));
        corners[1] = camera.WorldToViewportPoint(new Vector3(bounds.max.x, bounds.min.y, 0));
        corners[2] = camera.WorldToViewportPoint(new Vector3(bounds.min.x, bounds.max.y, 0));
        corners[3] = camera.WorldToViewportPoint(new Vector3(bounds.max.x, bounds.max.y, 0));

        for (int i = 0; i < 4; i++)
        {
            Vector3 corner = corners[i];
            if (corner.x < 0 || corner.x > 1 || corner.y < 0 || corner.y > 1)
            {
                return false;
            }
        }

        return true;
    }

}