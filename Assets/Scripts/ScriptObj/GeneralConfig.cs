using UnityEngine;
using redd096;

[CreateAssetMenu(menuName = "Cube Invaders/General Config", fileName = "General Config")]
public class GeneralConfig : ScriptableObject
{
    [Header("General")]
    [Tooltip("Keep pressed for this time to end strategic phase")] public float TimeToEndStrategic = 1.5f;
    [Tooltip("Cell selector")] public GameObject Selector;
    [Tooltip("Selector when there are more tiles selected at same time")] public GameObject MultipleSelector;
    [Tooltip("Portal to instantiate on enemy spawn")] public GameObject PortalPrefab;
    [Tooltip("When start wave, the delay before spawn first enemy")] public float DelaySpawnFirstEnemyWave = 1;

    [Header("Delay Commands")]
    public float delayReleaseFinishStrategicPhase = 0.1f;
    public float delayRotateOrSelectCell = 0.1f;

    [Header("Radar")]
    public bool showEnemiesHealth = true;
    public bool showEnemiesDestination = true;
    [CanShow("showEnemiesDestination")] public float minDistanceToShowDestination = 10;

    [Header("Sell Turrets Builded on this Wave")]
    public bool CanSellTurretsBuildedInThisWave = true;
    public bool TurretsOnAirFirstStrategicPhase = true;

    [Header("Helpers")]
    public HelperBase[] Helpers = default;
}
