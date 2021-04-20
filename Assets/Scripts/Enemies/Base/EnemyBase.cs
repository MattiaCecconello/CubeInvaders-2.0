using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Important")]
    [SerializeField] protected float health = 100;
    [SerializeField] protected float speed = 1;

    [Header("Resources")]
    [SerializeField] protected float resourcesWhenKilledByShot = 1;
    [SerializeField] protected float resourcesWhenKilledByShield = 0;
    [SerializeField] protected float resourcesWhenHitWorld = 0;

    [Header("Debug")]
    [SerializeField] protected Coordinates coordinatesToAttack;

    Rigidbody rb;

    public System.Action onGetDamage;
    public bool StillAlive { get; private set; } = true;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void FixedUpdate()
    {
        //move to the cell
        Vector3 direction = coordinatesToAttack.position - transform.position;

        rb.velocity = direction.normalized * speed;
    }

    #region public API

    public virtual void GetDamage(float damage, TurretShot whoHit)
    {
        //invoke event
        onGetDamage?.Invoke();

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
    }

    #endregion
}
