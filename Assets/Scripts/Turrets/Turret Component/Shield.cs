﻿using System.Collections;
using UnityEngine;
using redd096;

[SelectionBase]
[AddComponentMenu("Cube Invaders/Turret Component/Shield")]
public class Shield : MonoBehaviour
{
    [Header("Important")]
    [SerializeField] int health = 1;

    [Header("Graphics")]
    [Tooltip("Time for spawn animation")] [SerializeField] float timeSpawn = 1;
    [Tooltip("Time for despawn animation")] [SerializeField] float timeDespawn = 0.5f;

    public System.Action onShieldDestroyed;

    [Header("Debug")]
    [ReadOnly] public int CurrentHealth;

    Coroutine spawnShield_Coroutine;
    float distanceFromWorld;

    void Start()
    {
        //set distance from world using y local position in the prefab
        distanceFromWorld = transform.localPosition.z;
    }

    void OnTriggerEnter(Collider other)
    {
        //if hitted by enemy
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy)
        {
            //get damage
            ShieldGetDamage();

            //destroy enemy
            enemy.Die(this);
        }
    }

    #region private API

    IEnumerator SpawnShield_Coroutine(Vector3 finalSize, bool isSpawning)
    {
        float time = isSpawning ? timeSpawn : timeDespawn;

        //animation spawn
        float delta = 0;
        while(delta < 1)
        {
            delta += Time.deltaTime / time;

            transform.localScale = Vector3.Lerp(transform.localScale, finalSize, delta);

            yield return null;
        }

        //final size
        transform.localScale = finalSize;
    }

    void ShieldGetDamage()
    {
        CurrentHealth--;

        //if dead, shield destroyed
        if (CurrentHealth <= 0)
        {
            onShieldDestroyed?.Invoke();
        }
    }

    #endregion

    #region public API

    public void RegenHealth()
    {
        //regen health
        CurrentHealth = health;
    }

    public void ResetShieldSize()
    {
        //be sure there is no coroutine running
        if (spawnShield_Coroutine != null)
            StopCoroutine(spawnShield_Coroutine);

        //reset size immediatly cause this turret could be deactivate in hierarchy
        transform.localScale = Vector3.zero;
    }

    public void ActivateShield(Coordinates coordinates)
    {
        if (spawnShield_Coroutine != null)
            StopCoroutine(spawnShield_Coroutine);

        //set position
        Vector2Int centerCell = GameManager.instance.world.worldConfig.CenterCell;
        transform.position = GameManager.instance.world.CoordinatesToPosition(new Coordinates(coordinates.face, centerCell), distanceFromWorld);

        //get final scale
        float faceSize = GameManager.instance.world.worldConfig.FaceSize;
        float cellSize = GameManager.instance.world.worldConfig.CellsSize;
        Vector3 finalScale = new Vector3(faceSize, faceSize, cellSize);

        //start spawn
        if(gameObject.activeInHierarchy)
            spawnShield_Coroutine = StartCoroutine(SpawnShield_Coroutine(finalScale, true));
    }

    public void DeactivateShield()
    {
        if (spawnShield_Coroutine != null)
            StopCoroutine(spawnShield_Coroutine);

        //get final scale
        Vector3 finalScale = Vector3.zero;

        //start despawn
        if(gameObject.activeInHierarchy)
            spawnShield_Coroutine = StartCoroutine(SpawnShield_Coroutine(finalScale, false));
    }

    #endregion
}
