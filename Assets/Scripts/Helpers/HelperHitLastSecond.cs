using UnityEngine;

[CreateAssetMenu(menuName = "Cube Invaders/Helpers/Helper Hit Last Second", fileName = "Helper Hit Last Second")]
public class HelperHitLastSecond : HelperBase
{
    [Header("When enemy reach X health")]
    public float EnemyHealth = 30;

    [Header("If is near to the cube")]
    public float DistanceFromCube = 5;

    [Header("Add an 'helper' damage")]
    public float HelperDamage = 30;
}
