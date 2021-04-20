using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/Manager/Wave Manager")]
public class WaveManager : MonoBehaviour
{
    [Header("Important")]
    public WaveConfig waveConfig;
    
    public int CurrentWave { get; set; }

    List<Enemy> enemies = new List<Enemy>();
    Coroutine wave_coroutine;

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

    public Dictionary<EFace, List<Enemy>> enemiesOnFace { get; private set; } = new Dictionary<EFace, List<Enemy>>();

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
        //end assault phase
        GameManager.instance.levelManager.EndAssaultPhase();

        //if there aren't other waves
        if (waveConfig.Waves == null || CurrentWave >= waveConfig.Waves.Length -1 || CurrentWave < 0)
        {
            //win
            GameManager.instance.levelManager.EndGame(true);
            return;
        }

        //else go to next wave
        CurrentWave++;
    }

    #region events

    void AddEvents()
    {
        GameManager.instance.levelManager.onStartStrategicPhase += OnStartStrategicPhase;
        GameManager.instance.levelManager.onStartAssaultPhase += OnStartAssaultPhase;
        GameManager.instance.levelManager.onEndAssaultPhase += OnEndAssaultPhase;
    }

    void RemoveEvents()
    {
        GameManager.instance.levelManager.onStartStrategicPhase -= OnStartStrategicPhase;
        GameManager.instance.levelManager.onStartAssaultPhase -= OnStartAssaultPhase;
        GameManager.instance.levelManager.onEndAssaultPhase -= OnEndAssaultPhase;
    }

    void OnStartStrategicPhase()
    {
        //remove all enemies
        ClearEnemies();

        SetNewWave();
    }

    void OnStartAssaultPhase()
    {
        //start wave
        StartWave();
    }

    void OnEndAssaultPhase()
    {
        //stop coroutine if still running
        if (wave_coroutine != null)
            StopCoroutine(wave_coroutine);
    }

    #endregion

    #region private API

    void ClearEnemies()
    {
        //remove every child
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        //clear list
        enemies.Clear();
        enemiesOnFace.Clear();
    }

    IEnumerator Wave_Coroutine()
    {
        //current wave
        WaveStruct wave = waveConfig.Waves[CurrentWave];

        //enemies to spawn
        List<EnemyStruct> enemiesToSpawn = new List<EnemyStruct>();

        //foreach enemy in this wave, instantiate but deactivate
        foreach (EnemyStruct enemyStruct in wave.Enemies)
        {
            Enemy enemy = InstantiateNewEnemy(enemyStruct.enemy);

            //add to list enemies to spawn
            enemiesToSpawn.Add(new EnemyStruct(enemy, enemyStruct.enemyTimer));

            yield return null;
        }

        //queue to not spawn on same face
        Queue<EFace> facesQueue = new Queue<EFace>();

        //wait before instantiate first enemy
        yield return new WaitForSeconds(1);

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

        //if there are no other enemies, end wave
        if (enemies.Count <= 0 && GameManager.instance.levelManager.CurrentPhase == EPhase.assault)
        {
            EndWave();
        }
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

    #endregion
}
