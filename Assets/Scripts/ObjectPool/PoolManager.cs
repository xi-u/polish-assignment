using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectPool
{
    public delegate void OnEntityReady(Entity entity);

    public enum PoolableType
    {
        TailGator,
        Player,
        Bullet,
        Star,
        HomingMissile,
        SeekerHawk, //shoots homing missiles
        HiveRaptor, //follows the playerTransform in flocks and then khamikazes to them when they are close
        BlinkWolf,
        WebWeaver,
        Mine,
        MirrageManta,
        MirrageMantaClone,
        BlastBadger,
        SwarmSparrow
    }

    public class PoolManager : MonoBehaviour
    {
        [Serializable]
        private class PoolData 
        {
            public int initialSize;
            public PoolableType poolableType;
        }
        [SerializeField] private List<PoolData> poolDataList;

        [SerializeField] CameraBehaviour cameraBehaviour;

        public static PoolManager Instance { get; private set; }
        private readonly Dictionary<PoolableType, PoolBase> poolDictionary = new Dictionary<PoolableType, PoolBase>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            CreatePools();
        }

        private void CreatePools()
        {
            foreach (PoolData poolData in poolDataList)
            {
                string className = poolData.poolableType.ToString().Replace(" ", "");
                Type type = Type.GetType(className);

                if (type != null)
                {
                    Type poolType = typeof(Pool<>).MakeGenericType(type);
                    PoolBase pool = (PoolBase)Activator.CreateInstance(poolType, poolData.poolableType);
                    poolDictionary.Add(poolData.poolableType, pool);
                }
            }
        }

        public void SetEntityParentToPool(Entity entity)
        {
            if (poolDictionary.TryGetValue(entity.PoolableType, out PoolBase poolBase))
            {
                poolBase.SetEntityParentToPool(entity.Transform);
            }
        }

        private void Start()
        {
            foreach (PoolData poolData in poolDataList)
            {
                string className = poolData.poolableType.ToString();
                Type type = Type.GetType(className);

                if (type != null && poolDictionary.ContainsKey(poolData.poolableType))
                {
                    poolDictionary[poolData.poolableType].CreatePool(poolData.initialSize);
                }
            }
        }

        private bool IsEntityWithinRange(Vector2 location, Entity entity, float range)
        {
            return ((Vector2)entity.Transform.position - location).sqrMagnitude < range * range;
        }

        public List<Entity> GetActiveEntitiesWithinRange(Vector2 location, float range, params PoolableType[] entityTypes)
        {
            Player player = ServiceLocator.Instance.GetService<Player>();

            List<Entity> entitiesWithinRange = new List<Entity>(25);

            if (IsEntityWithinRange(location, player, range))
            {
                entitiesWithinRange.Add(player);
            }

            foreach (PoolableType poolableType in entityTypes)
            {
                Entity[] entities = GetAllActiveEntities(poolableType);

                foreach (Entity entity in entities)
                {
                    if (IsEntityWithinRange(location, entity, range))
                    {
                        entitiesWithinRange.Add(entity);
                    }
                }
            }
            return entitiesWithinRange;
        }

        public int GetTotalEntityCountOfType(PoolableType type)
        {
            CheckTypeAndThrowExceptionIfNotExists(type);
            return poolDictionary[type].TotalSize; 
        }

        public int GetActiveEntityCountOfType(PoolableType type)
        {
            CheckTypeAndThrowExceptionIfNotExists(type);
            return poolDictionary[type].ActiveSize;
        }

        public void GetEntity(PoolableType type, Vector2 position, OnEntityReady onEntityReady)
        {
            StartCoroutine(WaitForEntity(type, position, onEntityReady));
        }

        public PoolableType[] InteractiveEntities 
        {
            get
            {
                return new PoolableType[12]
                {
                    PoolableType.TailGator,
                    PoolableType.Bullet,
                    PoolableType.HomingMissile,
                    PoolableType.SeekerHawk,
                    PoolableType.HiveRaptor,
                    PoolableType.BlinkWolf,
                    PoolableType.Mine,
                    PoolableType.WebWeaver,
                    PoolableType.MirrageManta,
                    PoolableType.MirrageMantaClone,
                    PoolableType.BlastBadger,
                    PoolableType.SwarmSparrow
                };
            }
        }

        public void DestroyAllEnemies()
        {
            DestroyEnemies(InteractiveEntities);
        }
        
        private void DestroyEnemies(params PoolableType[] enemyTypes)
        {
            Rect cameraRect = cameraBehaviour.CameraRect;
            foreach (PoolableType poolableType in enemyTypes)
            {         
                Entity[] entities = GetAllActiveEntities(poolableType);
                foreach (Entity entity in entities) 
                {
                    entity.Deactivate();
                }
            }
        }

        private Entity[] GetAllActiveEntities(PoolableType type)
        {
            CheckTypeAndThrowExceptionIfNotExists(type);
            return poolDictionary[type].GetAllActiveEntities();
        }

        private IEnumerator WaitForEntity(PoolableType type, Vector2 position, OnEntityReady onEntityReady)
        {
            yield return new WaitForEndOfFrame();
            CheckTypeAndThrowExceptionIfNotExists(type);

            Entity entity = poolDictionary[type].GetPoolObject(position);
            onEntityReady?.Invoke(entity);
        }

        private void CheckTypeAndThrowExceptionIfNotExists(PoolableType type)
        {
            if (!poolDictionary.ContainsKey(type))
            {
                throw new Exception($"Pool with tag {type} doesn't exist.");
            }
        }
    }
}