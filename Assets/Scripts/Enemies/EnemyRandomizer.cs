using UnityEngine;

[SelectionBase]
[AddComponentMenu("Cube Invaders/Enemy/Enemy Randomizer")]
public class EnemyRandomizer : Enemy
{
    [Header("Randomizer")]
    [Tooltip("Number of rotations when hit cube")] [SerializeField] int numberRotations = 5;
    [Tooltip("Duration for every rotation")] [SerializeField] float rotationTime = 0.1f;

    public override void Die<T>(T hittedBy)
    {
        if (StillAlive)
        {
            //if hitted world, randomize it few times
            if (hittedBy.GetType() == typeof(Cell))
            {
                GameManager.instance.world.RotateByEnemy(numberRotations, rotationTime);
            }

            base.Die(hittedBy);
        }
    }
}
