using ObjectPool;
using System.Collections;
using UnityEngine;

public enum CollisionLayer
{
    Enemy = 6,
    Projectile = 7,
    Player = 8
}

public class GenerateWorldBehaviour : MonoBehaviour
{
    private Player player;    

    // The world is intentionally much larger than the viewport so it feels endless without a hard wrap.
    private static Rect playArea = new Rect(-30f, -20f, 60f, 40f);
    private static Rect worldArea = new Rect(-200f, -200f, 400f, 400f);
    public static Rect PlayArea => playArea;
    public static Rect WorldArea => worldArea;
    
    // Field to determine whether to spawn the full roster of enemies at the start of the game or to introduce them gradually over time.
    [SerializeField]
    private bool spawnFullRosterAtStart = false;
    
    private void Start()
	{
        CreateEnvironment();
        GenerateWorld();
    }

    private IEnumerator WaitAndRestartGame()
    {
        yield return new WaitForSeconds(2);

        player.Activate(new Vector2(playArea.width / 2 + playArea.xMin, playArea.height / 2 + playArea.yMin));

        CreateEnemies();
    }

    private void Player_OnPlayerDied(GameObject sender)
    {
        StopAllCoroutines();

        StartCoroutine(WaitAndRestartGame());

        PoolManager.Instance.DestroyAllEnemies();
    }

    public void GenerateWorld()
	{        
        player = new Player("Player", new Vector2(playArea.width / 2 + playArea.xMin, playArea.height / 2 + playArea.yMin));        

        ServiceLocator.Instance.Register(player);

        CreateEnemies();
    }
    // Enemies are introduced one at a time, easiest to hardest, so the full roster
    // is only present after about 5 minutes. Each gap is 5s longer than the last,
    // giving the player more time to adjust as the difficulty ramps up.
    private void CreateEnemies()
    {
        // if (spawnFullRosterAtStart)
        // {
        // SpawnEnemyImmediately(PoolableType.TailGator, 1f);
        // SpawnEnemyImmediately(PoolableType.SwarmSparrow, 1f);
        // SpawnEnemyImmediately(PoolableType.WebWeaver, 1f);
        // SpawnEnemyImmediately(PoolableType.SeekerHawk, 1f);
        // SpawnEnemyGroupImmediately(PoolableType.BlinkWolf, 2, 4, 1f);
        // SpawnEnemyImmediately(PoolableType.MirrageManta, 1f);
        // SpawnEnemyGroupImmediately(PoolableType.HiveRaptor, 3, 5, 1f);
        SpawnEnemyImmediately(PoolableType.BlastBadger, 1f);
        //     return;
        // }

        StartCoroutine(CreateSingleEnemy(PoolableType.TailGator, 3.6f, 0, 0f));
        StartCoroutine(CreateSingleEnemy(PoolableType.SwarmSparrow, 2.4f, 0, 25f));
        StartCoroutine(CreateSingleEnemy(PoolableType.WebWeaver, 5, 4, 55f));
        StartCoroutine(CreateSingleEnemy(PoolableType.SeekerHawk, 9, 0, 90f));
        StartCoroutine(CreateEnemyGroup(PoolableType.BlinkWolf, 2, 4, 10, 8, 130f));
        StartCoroutine(CreateSingleEnemy(PoolableType.MirrageManta, 6, 4, 175f));
        StartCoroutine(CreateEnemyGroup(PoolableType.HiveRaptor, 3, 5, 7, 3, 225f));
        StartCoroutine(CreateSingleEnemy(PoolableType.BlastBadger, 7, 5, 280f));
    }

    // The following methods are used to spawn enemies immediately, without waiting for the normal spawn intervals.
    private void SpawnEnemyImmediately(PoolableType enemyType, float delayBeforeSpawn = 0f)
    {
        StartCoroutine(CreateSingleEnemy(enemyType, delayBeforeSpawn, 0f, 0f));
    }

    private void SpawnEnemyGroupImmediately(PoolableType enemyType, int minAmount, int maxAmount, float delayBeforeSpawn = 0f)
    {
        StartCoroutine(CreateEnemyGroup(enemyType, minAmount, maxAmount, delayBeforeSpawn, 0f, 0f));
    }

    private Vector2 GetRandomPosition()
    {
        Vector2 playerPosition = player != null ? player.Transform.position : Vector2.zero;
        Camera camera = Camera.main;

        if (camera == null)
        {
            return playerPosition + new Vector2(Random.Range(-12f, 12f), Random.Range(-8f, 8f));
        }

        float halfHeight = camera.orthographicSize + 4f;
        float halfWidth = halfHeight * camera.aspect + 4f;
        float xMin = playerPosition.x - halfWidth;
        float xMax = playerPosition.x + halfWidth;
        float yMin = playerPosition.y - halfHeight;
        float yMax = playerPosition.y + halfHeight;

        float x = Random.Range(xMin, xMax);
        float y = Random.Range(yMin, yMax);

        if (Mathf.Abs(x - playerPosition.x) < halfWidth * 0.8f && Mathf.Abs(y - playerPosition.y) < halfHeight * 0.8f)
        {
            int side = Random.Range(0, 4);
            if (side == 0) x = playerPosition.x + halfWidth + Random.Range(1f, 4f);
            else if (side == 1) x = playerPosition.x - halfWidth - Random.Range(1f, 4f);
            else if (side == 2) y = playerPosition.y + halfHeight + Random.Range(1f, 4f);
            else y = playerPosition.y - halfHeight - Random.Range(1f, 4f);
        }

        return new Vector2(x, y);
    }

    private IEnumerator CreateSingleEnemy(PoolableType enemyType, float waitBefore, float waitAfter, float initialDelay = 0f)
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            yield return new WaitForSeconds(waitBefore);
            Vector2 randomPosition = GetRandomPosition();
            PoolManager.Instance.GetEntity(enemyType, randomPosition, (Entity e) => { });
            yield return new WaitForSeconds(waitAfter);
        }
    }
    private IEnumerator CreateEnemyGroup(PoolableType enemyType, int minAmount, int maxAmount, float waitBefore, float waitAfter, float initialDelay = 0f)
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            yield return new WaitForSeconds(waitBefore);

            int numberOfEnemies = Random.Range(minAmount, maxAmount);
            Vector2 randomPosition = GetRandomPosition();
            
            for (int i = numberOfEnemies; i >= 0; i--)
            {
                Vector2 randomOffset = new Vector2(Random.Range(-6, 6), Random.Range(-3, 3));
                PoolManager.Instance.GetEntity(enemyType, randomPosition + randomOffset, (Entity e) => 
                {
                    
                });
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(waitAfter);
        }
    }

    private void CreateEnvironment()
    {
        // No visible walls. We use world wrapping to create the illusion of an endless space.
    }

    public static bool IsPositionInPlayArea(Vector2 position)
    {
        return PlayArea.Contains(position);
    }

    private Rectangle CreateWall(Vector2 position, Vector2 size)
    {
        Rectangle wall = new Rectangle("Wall");
        wall.CreatePrimitiveShape();
        wall.GameObject.GetComponent<SpriteRenderer>().color = Color.red;
        wall.Transform.localScale = size;
        wall.GameObject.GetComponent<SpriteRenderer>().sortingOrder = 5;
        wall.Activate(position);

        return wall;
    }

    private void OnEnable()
    {
        Player.OnPlayerDied += Player_OnPlayerDied;
    }

    private void OnDisable()
    {
        Player.OnPlayerDied -= Player_OnPlayerDied;
    }
}
