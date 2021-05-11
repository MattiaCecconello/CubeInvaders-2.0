using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[AddComponentMenu("Cube Invaders/Enemy/Enemy")]
[RequireComponent(typeof(EnemyGraphics))]
public class Enemy : EnemyBase
{
    public Coordinates CoordinatesToAttack
    {
        get
        {
            return coordinatesToAttack;
        }
        set
        {
            GameManager.instance.waveManager.RemoveEnemyFromDictionary(this);
            coordinatesToAttack = value;
            GameManager.instance.waveManager.AddEnemyToDictionary(this);

        }
    }

    //used by wave manager
    public System.Action<Enemy> onEnemyDeath;
    public System.Action onShowHealth;
    public System.Action onHideHealth;

    List<float> effectsOnEnemy = new List<float>();

    float previousDistanceFromCube;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        //try change max speed
        TryChangeMaxSpeed();
    }

    public override void Die<T>(T hittedBy)
    {
        if (StillAlive)
        {
            //call wave manager
            GameManager.instance.waveManager.OnEnemyDeath(this);

            //call event
            onEnemyDeath?.Invoke(this);

            base.Die(hittedBy);
        }
    }

    float SetSpeed()
    {
        float newSpeed = maxSpeed;

        foreach (float slowPercentage in effectsOnEnemy)
        {
            //slow based on max speed
            float speedToDecrease = maxSpeed / 100 * slowPercentage;
            newSpeed -= speedToDecrease;

            //if reached 0, stop
            if (newSpeed <= 0)
            {
                newSpeed = 0;
                break;
            }
        }

        return newSpeed;
    }

    #region slow

    public void GetSlow(float slowPercentage, float slowDuration)
    {
        //do only if there is slow effect
        if (slowPercentage <= 0 || slowDuration <= 0)
            return;

        //add slow effect
        effectsOnEnemy.Add(slowPercentage);

        //set speed
        speed = SetSpeed();

        //remove effect timer
        if (gameObject.activeInHierarchy)
            StartCoroutine(RemoveEffectTimer(slowPercentage, slowDuration));
    }

    IEnumerator RemoveEffectTimer(float slowPercentage, float slowDuration)
    {
        //wait duration
        yield return new WaitForSeconds(slowDuration);

        //remove effect
        effectsOnEnemy.Remove(slowPercentage);

        //set speed
        speed = SetSpeed();
    }

    #endregion

    #region radar

    public void ShowHealth()
    {
        //show health
        onShowHealth?.Invoke();
    }

    public void HideHealth()
    {
        //hide health
        onHideHealth?.Invoke();
    }

    #endregion

    #region change max speed

    void TryChangeMaxSpeed()
    {
        //be sure previous is setted
        if (previousDistanceFromCube <= 0)
            previousDistanceFromCube = Mathf.Infinity;

        float distanceToCheck = Mathf.Infinity;
        int index = 0;

        for(int i = 0; i < percentageMaxSpeed.Length; i++)
        {
            //check if our distance is lower then this one in the array
            if (distanceFromCube <= percentageMaxSpeed[i].distanceFromCube)
            {
                //get only lower one (nearest to our current distance)
                if (percentageMaxSpeed[i].distanceFromCube < distanceToCheck)
                {
                    distanceToCheck = percentageMaxSpeed[i].distanceFromCube;
                    index = i;
                }
            }
        }

        //if got nothing, or again previous distance, do not set speed again
        if (distanceToCheck <= 0 || distanceToCheck >= previousDistanceFromCube)
            return;

        //save previous
        previousDistanceFromCube = distanceToCheck;

        //change max speed
        maxSpeed = initialMaxSpeed / 100 * percentageMaxSpeed[index].maxSpeedPercentage;

        //set speed
        speed = SetSpeed();
    }

    #endregion
}
