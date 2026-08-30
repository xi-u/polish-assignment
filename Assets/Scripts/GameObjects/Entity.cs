using ObjectPool;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Entity
{
    public PoolableType PoolableType { get; set; }

    public delegate void OnEntityDestroyedHandler(Entity entity);
    public static event OnEntityDestroyedHandler OnEntityDestroyed;

    public Entity(string name = null)
    {
        //Setting this name is important to be able to debug from de inspector.
        name ??= $"Object {SceneManager.GetActiveScene().GetRootGameObjects().Length}";
        this.GameObject = new GameObject
        {
            name = name,
            transform = { position = Vector2.zero }
        };
        Transform = GameObject.transform;
    }

    public EntityId InstanceId => GameObject.GetEntityId();

    public bool IsActive
    {
        get { return isActive; }
        set { isActive = value; GameObject.SetActive(isActive); }
    }

    protected bool isActive = false;

    public Transform Transform { get; protected set; }

    public GameObject GameObject
    {
        get;
        protected set;
    }

    public virtual void Activate(Vector2 position)
    {
        IsActive = true;

        if (position == default)
        {
            position = Vector2.zero;
        }
        Transform.position = position;
    }

    public virtual void Deactivate()
    {
        IsActive = false;
    }

    public virtual void DestroySelf(bool raiseEntityDestroyedEvent = true)
    {
        if (raiseEntityDestroyedEvent)
        {
            if (IsActive)
            {
                OnEntityDestroyed?.Invoke(this);
            }
        }
        Deactivate();
    }
}