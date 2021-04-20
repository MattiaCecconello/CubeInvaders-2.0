using System.Collections.Generic;
using UnityEngine;
using redd096;

[SelectionBase]
[AddComponentMenu("Cube Invaders/Turret/Radar")]
[RequireComponent(typeof(RadarGraphics))]
public class Radar : BuildableObject
{
    public Enemy EnemyToAttack { get; private set; }

    void Update()
    {
        //if active, find enemy
        if (IsActive)
        {
            EnemyToAttack = FindEnemy();
        }
    }

    Enemy FindEnemy()
    {
        //find enemies attacking this face and get the nearest
        List<Enemy> enemies = GameManager.instance.waveManager.EnemiesOnFace(CellOwner.coordinates.face);
        return enemies.FindNearest(transform.position);
    }
}
