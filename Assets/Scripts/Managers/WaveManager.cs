﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/Manager/Wave Manager")]
public class WaveManager : MonoBehaviour
{
    public WaveConfig waveConfig;
    [SerializeField] int currentWave = -1;

    List<Enemy> enemies = new List<Enemy>();
    Coroutine wave_coroutine;

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

    #region events

    void AddEvents()
    {
        GameManager.instance.levelManager.onStartStrategicPhase += OnStartStrategicPhase;
        GameManager.instance.levelManager.onEndStrategicPhase += OnEndStrategicPhase;
        GameManager.instance.levelManager.onStartAssaultPhase += OnStartAssaultPhase;
    }

    void RemoveEvents()
    {
        GameManager.instance.levelManager.onStartStrategicPhase -= OnStartStrategicPhase;
        GameManager.instance.levelManager.onEndStrategicPhase -= OnEndStrategicPhase;
        GameManager.instance.levelManager.onStartAssaultPhase -= OnStartAssaultPhase;
    }

    void OnStartStrategicPhase()
    {
        //wave +1
        currentWave++;
        ClearEnemies();

        //if there aren't other waves
        if(waveConfig.Waves == null || currentWave >= waveConfig.Waves.Length)
        {
            //win
            GameManager.instance.levelManager.EndGame(true);
        }
    }

    void OnEndStrategicPhase()
    {
        //do only if there are waves
        if (currentWave < 0 || currentWave >= waveConfig.Waves.Length)
            return;

        //foreach enemy in this wave
        WaveStruct wave = waveConfig.Waves[currentWave];
        foreach(Enemy enemyPrefab in wave.EnemiesPrefabs)
        {
            //instantiate and set parent but deactivate
            Enemy enemy = Instantiate(enemyPrefab, transform);
            enemy.gameObject.SetActive(false);

            //save in the list and add to the event
            enemies.Add(enemy);
            enemy.onEnemyDeath += OnEnemyDeath;
        }

        //update level config and biomes config
        if (GameManager.instance.levelManager.levelConfig != wave.LevelConfig)
        {
            GameManager.instance.levelManager.levelConfig = wave.LevelConfig;
        }
        if(GameManager.instance.world.biomesConfig != wave.BiomesConfig)
        {
            GameManager.instance.world.biomesConfig = wave.BiomesConfig;
            GameManager.instance.world.RegenWorld();
        }
    }

    void OnStartAssaultPhase()
    {
        //start wave
        if (wave_coroutine != null)
            StopCoroutine(wave_coroutine);

        wave_coroutine = StartCoroutine(Wave_Coroutine());
    }

    void OnEnemyDeath(Enemy enemy)
    {
        //remove from the list
        enemies.Remove(enemy);

        //if there are no other enemies, end assault phase
        if(enemies.Count <= 0)
        {
            GameManager.instance.levelManager.EndAssaultPhase();
        }
    }

    #endregion

    void ClearEnemies()
    {
        //remove every child
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        //clear list
        enemies.Clear();
    }

    IEnumerator Wave_Coroutine()
    {
        //create a copy, so we can touch the original without affect the wave coroutine
        Enemy[] enemiesThisWave = enemies.CreateCopy().ToArray();

        //for every enemy
        foreach(Enemy enemy in enemiesThisWave)
        {
            //randomize coordinates to attack
            EFace face = (EFace)Random.Range(0, 6);
            int x = Random.Range(0, GameManager.instance.world.worldConfig.NumberCells);
            int y = Random.Range(0, GameManager.instance.world.worldConfig.NumberCells);
            Coordinates coordinatesToAttack = new Coordinates(face, x, y);

            //set enemy position
            float distanceFromWorld = waveConfig.distanceFromWorld;
            enemy.transform.position = GameManager.instance.world.CoordinatesToPosition(coordinatesToAttack, distanceFromWorld);

            //set enemy destination and activate
            enemy.coordinatesToAttack = coordinatesToAttack;
            enemy.gameObject.SetActive(true);

            //wait for next enemy
            yield return new WaitForSeconds(waveConfig.TimeBetweenSpawns);
        }
    }
}
