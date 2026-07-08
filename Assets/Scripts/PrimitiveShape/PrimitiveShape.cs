using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class PrimitiveShape : Entity
{	
	protected PrimitiveShape(string name = null) : base(name)
	{
		
    }
	
	public abstract Collider2D AddCollider(bool isTrigger = true);

	public abstract void CreatePrimitiveShape(Vector2[] vertices, ushort[] triangles);
}
