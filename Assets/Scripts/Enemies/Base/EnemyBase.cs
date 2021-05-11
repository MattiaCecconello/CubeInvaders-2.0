using UnityEngine;
using redd096;

public class EnemyBase : MonoBehaviour
{
    [Header("Important")]
    [SerializeField] protected float health = 100;
    [SerializeField] protected float speed = 1;

    [Header("Resources")]
    [SerializeField] protected float resourcesWhenKilledByShot = 1;
    [SerializeField] protected float resourcesWhenKilledByShield = 0;
    [SerializeField] protected float resourcesWhenHitWorld = 0;

    [Header("Destroy Cross")]
    [SerializeField] bool destroyCross = false;
    [CanShow("destroyCross")] [Min(0)] [SerializeField] float poisonTimer = 0;
    [CanShow("destroyCross")] [Min(1)] [SerializeField] int poisonSpread = 1;

    [Header("Debug")]
    [SerializeField] protected Coordinates coordinatesToAttack;
    [ReadOnly] [SerializeField] protected float maxHealth;
    [ReadOnly] [SerializeField] protected float distanceFromCube;
    [ReadOnly] [SerializeField] protected float maxSpeed;

    Rigidbody rb;

    public System.Action<float, float> onGetDamage;
    public bool StillAlive { get; private set; } = true;
    public float DistanceFromCube => distanceFromCube;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();

        //set max health
        maxHealth = health;

        //set max speed
        maxSpeed = speed;
    }

    protected virtual void FixedUpdate()
    {
        //move to the cell
        Vector3 direction = coordinatesToAttack.position - transform.position;

        rb.velocity = direction.normalized * speed;

        //update distance from cube
        distanceFromCube = Vector3.Distance(transform.position, coordinatesToAttack.position);
    }

    #region public API

    public virtual void GetDamage(float damage, TurretShot whoHit)
    {
        //invoke event
        onGetDamage?.Invoke(health, maxHealth);

        //get damage
        health -= damage;

        //check death
        if (health <= 0)
        {
            Die(whoHit);
        }
    }

    public virtual void Die<T>(T hittedBy) where T : Component
    {
        if (StillAlive)
        {
            StillAlive = false;

            //add resources to player
            if (hittedBy.GetType() == typeof(TurretShot))
            {
                GameManager.instance.player.CurrentResources += resourcesWhenKilledByShot;
            }
            else if (hittedBy.GetType() == typeof(Shield))
            {
                GameManager.instance.player.CurrentResources += resourcesWhenKilledByShield;
            }
            else if (hittedBy.GetType() == typeof(Cell))
            {
                GameManager.instance.player.CurrentResources += resourcesWhenHitWorld;

                //when hit cell, can poison cell
                if(destroyCross)
                    hittedBy.gameObject.AddComponent<PoisonCell>().Init(poisonTimer, poisonSpread);
            }

            //destroy this enemy
            Destroy(gameObject);
        }
    }

    public virtual void Init(Coordinates coordinatesToAttack)
    {
        //set coordinates to attack and enable
        this.coordinatesToAttack = coordinatesToAttack;
        gameObject.SetActive(true);

        //update distance from cube
        distanceFromCube = Vector3.Distance(transform.position, coordinatesToAttack.position);
    }

    #endregion
}
