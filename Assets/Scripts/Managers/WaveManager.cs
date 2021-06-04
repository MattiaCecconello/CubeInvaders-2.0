using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

#region enum

public enum ELevel
{
    normalLevel, bossLevel, lastPhaseBossLevel
}

#endregion

[AddComponentMenu("Cube Invaders/Manager/Wave Manager")]
public class WaveManager : MonoBehaviour
{
    [Header("Important")]
    public WaveConfig waveConfig;

    [Header("Loop Wave when there is a Boss?")]
    [SerializeField] bool loopWaveWithBoss = true;

    [Header("Debug")]
    [ReadOnly] [SerializeField] ELevel typeLevel = ELevel.normalLevel;

    public int CurrentWave { get; set; }

    List<Enemy> enemies = new List<Enemy>();
    Coroutine wave_coroutine;
    Coroutine restartWaveCoroutine;

    private Transform portalsParent;
    Transform PortalsParent
    {
        get
        {
            if (portalsParent == null)
                portalsParent = new GameObject("Portals Parent").transform;

            return portalsParent;
        }
    }

    Dictionary<EFace, List<Enemy>> enemiesOnFace = new Dictionary<EFace, List<Enemy>>();

    void Start()
    {
        //add events
        AddEvents();
    }

    void OnDestroy()
    {
        //remove events
        RemoveEvents();
    }

    void SetNewWave()
    {
        //current wave and update UI
        WaveStruct wave = waveConfig.Waves[CurrentWave];
        GameManager.instance.uiManager.UpdateCurrentLevelText(CurrentWave);

        //update level config (change level) and update resources for player
        GameManager.instance.levelManager.UpdateLevel(wave.LevelConfig);
        GameManager.instance.player.CurrentResources += wave.Resources;
    }

    void StartWave()
    {
        //start coroutine
        if (gameObject.activeInHierarchy)
        {
            if (wave_coroutine != null)
                StopCoroutine(wave_coroutine);

            wave_coroutine = StartCoroutine(Wave_Coroutine());
        }
    }

    void EndWave()
    {
        //if there aren't other waves
        if (waveConfig.Waves == null || CurrentWave >= waveConfig.Waves.Length -1 || CurrentWave < 0)
        {
            //remove all enemies
            ClearEnemies();

            //win
            GameManager.instance.levelManager.EndGame(true);
            return;
        }

        //else go to next wave
        CurrentWave++;

        //end assault phase
        GameManager.instance.levelManager.EndAssaultPhase();
    }

    #region events

    void AddEvents()
    {
        GameManager.instance.levelManager.onStartGame += OnStartGame;
        GameManager.instance.levelManager.onStartStrategicPhase += OnStartStrategicPhase;
        GameManager.instance.levelManager.onStartAssaultPhase += OnStartAssaultPhase;
        GameManager.instance.levelManager.onEndAssaultPhase += OnEndAssaultPhase;
    }

    void RemoveEvents()
    {
        GameManager.instance.levelManager.onStartGame -= OnStartGame;
        GameManager.instance.levelManager.onStartStrategicPhase -= OnStartStrategicPhase;
        GameManager.instance.levelManager.onStartAssaultPhase -= OnStartAssaultPhase;
        GameManager.instance.levelManager.onEndAssaultPhase -= OnEndAssaultPhase;
    }

    void OnStartGame()
    {
        //set wave, as in strategic phase (necesary if this scene start in assault phase)

        //remove all enemies
        ClearEnemies();

        SetNewWave();
    }

    void OnStartStrategicPhase()
    {
        //do only if not first wave, because first wave is initialized in OnStartGame

        if (CurrentWave > 0)
        {
            //remove all enemies
            ClearEnemies();

            SetNewWave();
        }
    }

    void OnStartAssaultPhase()
    {
        //start wave
        StartWave();
    }

    void OnEndAssaultPhase()
    {
        //be sure no enemies in scene
        ClearEnemies();
    }

    #endregion

    #region private API

    IEnumerator Wave_Coroutine()
    {
        //current wave
        WaveStruct wave = waveConfig.Waves[CurrentWave];

        //create list enemies to spawn
        List<EnemyStruct> waveEnemies = new List<EnemyStruct>();
        for(int i = 0; i < wave.Enemies.Length; i++)
        {
            //(remove boss if this wave is restarted)
            if (wave.Enemies[i].enemy is EnemyBoss && typeLevel != ELevel.normalLevel)
            {
                continue;
            }

            waveEnemies.Add(wave.Enemies[i]);
        }

        //enemies to spawn
        List<EnemyStruct> enemiesToSpawn = new List<EnemyStruct>();

        //foreach enemy in this wave, instantiate but deactivate
        foreach (EnemyStruct enemyStruct in waveEnemies)
        {
            Enemy enemy = InstantiateNewEnemy(enemyStruct.enemy);   //add also to enemies list

            //add to list enemies to spawn (use enemy instantiated instead of prefab)
            enemiesToSpawn.Add(new EnemyStruct(enemy, enemyStruct.enemyTimer));

            //if loaded a boss - set type of level (boss level or last phase boss)
            if (enemy is EnemyBoss)
            {
                typeLevel = ((EnemyBoss)enemy).LastPhaseBoss ? ELevel.lastPhaseBossLevel : ELevel.bossLevel;
            }

            yield return null;
        }

        //queue to not spawn on same face
        Queue<EFace> facesQueue = new Queue<EFace>();

        //wait before instantiate first enemy
        yield return new WaitForSeconds(GameManager.instance.levelManager.generalConfig.DelaySpawnFirstEnemyWave);

        //for every enemy
        foreach (EnemyStruct enemyStruct in enemiesToSpawn)
        {
            //randomize coordinates to attack
            EFace face = WorldUtility.GetRandomFace(facesQueue, waveConfig.Waves[CurrentWave].IgnorePreviousFacesAtSpawn);
            int x = Random.Range(0, GameManager.instance.world.worldConfig.NumberCells);
            int y = Random.Range(0, GameManager.instance.world.worldConfig.NumberCells);
            Coordinates coordinatesToAttack = new Coordinates(face, x, y);

            //get position and rotation
            Vector3 position;
            Quaternion rotation;
            GameManager.instance.world.GetPositionAndRotation(coordinatesToAttack, waveConfig.Waves[CurrentWave].DistanceFromWorld, out position, out rotation);

            //set enemy position and rotation, then activate
            enemyStruct.enemy.transform.position = position;
            enemyStruct.enemy.transform.rotation = rotation;

            //instantiate portal at position and rotation
            if (GameManager.instance.levelManager.generalConfig.PortalPrefab)
            {
                GameObject portal = Instantiate(GameManager.instance.levelManager.generalConfig.PortalPrefab, position, rotation);
                portal.transform.SetParent(PortalsParent);
            }

            //set enemy destination and activate
            enemyStruct.enemy.Init(coordinatesToAttack);
            AddEnemyToDictionary(enemyStruct.enemy);

            //wait for next enemy
            yield return new WaitForSeconds(wave.TimeBetweenSpawns + enemyStruct.enemyTimer);
        }

        wave_coroutine = null;
    }

    IEnumerator RestartWaveCoroutine()
    {
        //wait until wave is finished (because can be that the boss is not still active, is only spawned disabled)
        while(wave_coroutine != null)
        {
            yield return null;
        }

        //then check if ncessary to reload wave
        if (enemies.Count == 1 && enemies[0] is EnemyBoss)
            StartWave();
    }

    void CheckEndWave(Enemy lastEnemyKilled)
    {
        //in normal levels
        if (typeLevel == ELevel.normalLevel)
        {
            //if there are no other enemies, end wave
            if (enemies.Count <= 0 && GameManager.instance.levelManager.CurrentPhase == EPhase.assault)
            {
                EndWave();
            }
        }
        //in boss levels
        else
        {
            //in last phase
            if (typeLevel == ELevel.lastPhaseBossLevel)
            {
                //if kill the boss, end wave
                if (lastEnemyKilled is EnemyBoss && GameManager.instance.levelManager.CurrentPhase == EPhase.assault)
                {
                    EndWave();
                    return;
                }
            }

            //if there is only an enemy and that enemy is the boss, restart wave
            if (enemies.Count == 1 && enemies[0] is EnemyBoss)
            {
                if (loopWaveWithBoss)                                               //only if can loop
                {
                    if (restartWaveCoroutine != null)
                        StopCoroutine(restartWaveCoroutine);

                    restartWaveCoroutine = StartCoroutine(RestartWaveCoroutine());  //obviously in the wave, will be the check to not respawn the boss
                }
            }
        }
    }

    #endregion

    #region public API

    public Enemy InstantiateNewEnemy(Enemy enemyPrefab)
    {
        //instantiate and set parent but deactivate
        Enemy enemy = Instantiate(enemyPrefab, transform);
        //float size = GameManager.instance.world.worldConfig.CellsSize;
        //enemy.transform.localScale = new Vector3(size, size, size);
        enemy.gameObject.SetActive(false);

        //save in the list
        enemies.Add(enemy);

        return enemy;
    }

    public void OnEnemyDeath(Enemy enemy)
    {
        //remove from the list
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
            RemoveEnemyFromDictionary(enemy);       //remove from dictionary
        }

        //check if ended wave
        CheckEndWave(enemy);
    }

    public void AddEnemyToDictionary(Enemy enemy)
    {
        //add key if dictionary no contains
        if (enemiesOnFace.ContainsKey(enemy.CoordinatesToAttack.face) == false)
            enemiesOnFace.Add(enemy.CoordinatesToAttack.face, new List<Enemy>());

        //add enemy to the list
        enemiesOnFace[enemy.CoordinatesToAttack.face].Add(enemy);
    }

    public void RemoveEnemyFromDictionary(Enemy enemy)
    {
        //if dictionary has key && enemy is in the list, remove it
        if (enemiesOnFace.ContainsKey(enemy.CoordinatesToAttack.face) && enemiesOnFace[enemy.CoordinatesToAttack.face].Contains(enemy))
            enemiesOnFace[enemy.CoordinatesToAttack.face].Remove(enemy);
    }

    public List<Enemy> EnemiesOnFace(EFace face)
    {
        //return list - if no key in the dictionary, return clear list
        if (enemiesOnFace.ContainsKey(face))
            return new List<Enemy>(enemiesOnFace[face]);

        return new List<Enemy>();
    }

    public void ClearEnemies()
    {
        //stop coroutine if still running
        if (wave_coroutine != null)
            StopCoroutine(wave_coroutine);

        //remove every child
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        //clear list
        enemies.Clear();
        enemiesOnFace.Clear();
    }

    #endregion
}
