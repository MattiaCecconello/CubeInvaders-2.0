using UnityEngine;

[SelectionBase]
[AddComponentMenu("Cube Invaders/Enemy/Enemy Poison")]
public class EnemyPoison : Enemy
{
    [Header("Poison")]
    [SerializeField] float timerPoison = 5;
    [SerializeField] int limitSpread = 1;
    [SerializeField] bool canDestroyTurrets = false;

    public override void Die<T>(T hittedBy)
    {
        if (StillAlive)
        {
            //poison cell instead of kill it
            if (hittedBy.GetType() == typeof(Cell))
            {
                hittedBy.gameObject.AddComponent<PoisonCell>().Init(timerPoison, limitSpread, canDestroyTurrets, true);
            }

            base.Die(hittedBy);
        }
    }
}
