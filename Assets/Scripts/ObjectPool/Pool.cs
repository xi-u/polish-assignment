using System;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectPool
{
    public abstract class PoolBase : Entity
    {
        protected PoolableType poolableType;

        protected PoolBase(string name, PoolableType poolableType) : base(name)
        {
            this.poolableType = poolableType;
        }
        public abstract void CreatePool(int initialSize);

        public abstract Entity[] GetAllActiveEntities();

        public abstract Entity GetPoolObject(Vector2 position);

        public abstract void SetEntityParentToPool(Transform transform);

        public abstract int TotalSize { get; }
        public abstract int ActiveSize { get; }
    }

    public class Pool<T> : PoolBase where T : Entity
    {
        private const int GrowBatchSize = 5;

        public override int TotalSize { get { return pool.Count; } }

        public override int ActiveSize
        {
            get
            {
                int size = 0;
                for (int i = 0; i < TotalSize; i++)
                {
                    if (pool[i].IsActive)
                    {
                        size++;
                    }
                }
                return size;
            }
        }

        private List<Entity> pool;

        public Pool(PoolableType poolableType) : base($"{typeof(T)} Pool", poolableType)
        {

        }

        public override Entity[] GetAllActiveEntities()
        {
            int activeSize = ActiveSize;
            Entity[] entities = new Entity[activeSize];

            for (int i = 0; i < TotalSize; i++)
            {
                if (pool[i].IsActive)
                {
                    entities[activeSize -1] = pool[i];
                    if (activeSize-- == 0)
                    {
                        break;
                    }
                }
            }
            return entities;
        }

        public override void CreatePool(int initialSize)
        {
            pool = new List<Entity> ();
            for (int i = 0; i < initialSize; i++)
            {
                CreateAndAddPoolObject();
            }
        }

        private Entity CreateAndAddPoolObject()
        {
            Entity e = Activator.CreateInstance(typeof(T), new object[] { typeof(T).Name }) as Entity;

            e.Transform.SetParent(GameObject.transform, false);
            e.Deactivate();
            
            e.PoolableType = poolableType;
            pool.Add(e);

            return e;
        }

        public override void SetEntityParentToPool(Transform transform)
        {
            transform.SetParent(GameObject.transform, false);
        }

        public override Entity GetPoolObject(Vector2 position) 
        {
            foreach (Entity e in pool)
            {
                if (!e.IsActive)
                {
                    e.Activate(position);
                    return e;
                }
            }
            // No inactive pool objects left: grow the pool by a batch instead of
            // just one, so there's already spare capacity for the next request too.
            for (int i = 0; i < GrowBatchSize; i++)
            {
                CreateAndAddPoolObject();
            }

            Entity newEntity = pool[pool.Count - GrowBatchSize];
            newEntity.Activate(position);
            return newEntity;
        }
    }
}