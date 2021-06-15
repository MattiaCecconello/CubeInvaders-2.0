using UnityEngine;
using redd096;

[CreateAssetMenu(menuName = "Cube Invaders/Level/Level Config", fileName = "Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("Important")]
    [Tooltip("The player can recreate a destroyed cell")] public bool CanRecreateCell = false;
    [Tooltip("Randomize world at start")] public bool RandomizeWorldAtStart = true;
    [Tooltip("Start in strategic phase or assault phase?")] public bool StartInStrategicPhase = true;

    [Header("Modifier")]
    [Tooltip("How many rotations at time")] [Min(1)] public int NumberRotations = 1;
    [Tooltip("Size of the selector, to select one cell or more")] [Min(1)] public int SelectorSize = 1;

    [Header("Modifier - Destroy turrets when no move")]
    [Tooltip("Destroy turret after few seconds that player doesn't move it")] public bool DestroyTurretWhenNoMove = false;
    [CanShow("DestroyTurretWhenNoMove")] public bool DisableInsteadOfDestroy = true;

    [Header("Modifier - Generator")]
    [Tooltip("Turret need generator to activate")] public bool TurretsNeedGenerator = false;
    [Tooltip("Activate every turret on this face, or only turrets around")] [CanShow("TurretsNeedGenerator")] public bool GeneratorActiveAllFace = true;

    [Header("Modifier - Limit of turrets on same face")]
    [Tooltip("Limit of turrets on same face, if exceed explode turrets (0 = no limits)")] [Min(0)] public int LimitOfTurretsOnSameFace = 0;
    [Tooltip("Timer to destroy if there are more turrets on same face")] [CanShow("LimitOfTurretsOnSameFace")] [Min(0)] public float TimeBeforeDestroyTurretsOnSameFace = 2;
    [Tooltip("Limit of turrets on same face, only if is the same type of turret")] [CanShow("LimitOfTurretsOnSameFace")] public bool OnlyIfSameType = true;
    [CanShow("LimitOfTurretsOnSameFace")] public LineRenderer line = default;
    [CanShow("LimitOfTurretsOnSameFace")] public Color lineColorWhenExplode = Color.red;
}
