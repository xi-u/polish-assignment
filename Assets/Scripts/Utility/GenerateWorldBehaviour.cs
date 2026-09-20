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

    private static Rect playArea = new Rect(0, 0, 25, 25);
    private static Rect worldArea = new Rect(-20, -10, 65, 45);
    public static Rect PlayArea => playArea; 
    
    // Field to determine whether to spawn the full roster of enemies at the start of the game or to introduce them gradually over time.
    [SerializeField]
    private bool spawnFullRosterAtStart = true;
    
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
        if (spawnFullRosterAtStart)
        {
            // SpawnEnemyImmediately(PoolableType.TailGator, 10f);
            // SpawnEnemyImmediately(PoolableType.SwarmSparrow, 10f);
            // SpawnEnemyImmediately(PoolableType.WebWeaver, 10f);
            // SpawnEnemyImmediately(PoolableType.SeekerHawk, 3f);
            // SpawnEnemyGroupImmediately(PoolableType.BlinkWolf, 2, 4, 1f);
            // SpawnEnemyImmediately(PoolableType.MirrageManta, 1f);
            // SpawnEnemyGroupImmediately(PoolableType.HiveRaptor, 3, 5, 1f);
            SpawnEnemyImmediately(PoolableType.BlastBadger, 1f);
            return;
        }

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
        Vector2 randomPosition = Utility.GetRandomPositionOutsideRect(playArea, 10f);
        return randomPosition;
    }

    private IEnumerator CreateSingleEnemy(PoolableType enemyType, float waitBefore, float waitAfter, float initialDelay = 0f)
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            yield return new WaitForSeconds(waitBefore);
            Vector2 randomPosition = Utility.GetRandomPositionOutsideRect(playArea, 10f);
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
        CreateWall(new Vector2(-1, 12.506f), new Vector2(0.1f, 26.68815f));
        CreateWall(new Vector2(26, 12.506f), new Vector2(0.1f, 26.68815f));
        CreateWall(new Vector2(12.49997f, 25.85f), new Vector2(27.10007f, 0.1f));
        CreateWall(new Vector2(12.49997f, -0.89f), new Vector2(27.10007f, 0.1f));
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
