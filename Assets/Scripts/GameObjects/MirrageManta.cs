using UnityEngine;

public class MirrageManta : DoubleWingedTriangularShip
{
    private MirrageMantaBehaviour mirrageMantaBehaviour;
    private EntityId[] childIds = new EntityId[0];
    private Sprite shipSprite;

    public MirrageManta(string name) : base(name)
    {
        SetCollisionLayer(CollisionLayer.Enemy);
        AddCollider();

        SetColor(new Color32(93, 63, 211, 255));

        GameObject.AddComponent<RoamingBehaviour>().MovementSpeed = UnitStats.MirrageMantaSpeed;
        mirrageMantaBehaviour = GameObject.AddComponent<MirrageMantaBehaviour>();        
        SetSprite();
    }
    
    private void SetSprite()
    {
        shipSprite = Resources.Load<Sprite>("Sprites/purple-ship");

        SpriteRenderer spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GameObject.AddComponent<SpriteRenderer>();
        }

        spriteRenderer.transform.localScale = new  Vector3(0.5f, 0.5f, 0.5f);
        spriteRenderer.sprite = shipSprite;
        spriteRenderer.sortingOrder = 10;
        spriteRenderer.color = Color.white;

        if (leftWingSpriteRenderer != null)
            leftWingSpriteRenderer.enabled = false;

        if (rightWingSpriteRenderer != null)
            rightWingSpriteRenderer.enabled = false;
    }

    public override void Activate(Vector2 position)
    {
        base.Activate(position);
        mirrageMantaBehaviour.Activate(this);
    }

    public void SetChildIds(params EntityId[] childIds)
    {
        this.childIds = childIds;
        collisionBehaviour.StartIgnoringTriggerEventsFor(childIds);
    }

    public override void Deactivate()
    {
        base.Deactivate();
        collisionBehaviour.StopIgnoringTriggerEventsFor(childIds);
    }
}
