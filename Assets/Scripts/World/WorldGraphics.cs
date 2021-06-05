using UnityEngine;

[AddComponentMenu("Cube Invaders/World/World Graphics")]
public class WorldGraphics : MonoBehaviour
{
    [Header("Explode only when lose or also win?")]
    [SerializeField] bool explodeOnlyOnLose = true;

    [Header("Destroy every enemy in scene")]
    [SerializeField] bool destroyEnemiesInScene = true;

    [Header("Explosion Force")]
    [SerializeField] float minRandomizeDirection = -1;
    [SerializeField] float maxRandomizeDirection = 1;
    [SerializeField] float minForceExplosion = 1;
    [SerializeField] float maxForceExplosion = 10;

    [Header("Explosion Torque")]
    [SerializeField] float minRandomizeTorqueDirection = -1;
    [SerializeField] float maxRandomizeTorqueDirection = 1;
    [SerializeField] float minTorqueExplosion = 1;
    [SerializeField] float maxTorqueExplosion = 10;

    World world;

    void Awake()
    {
        world = GetComponent<World>();
    }

    void OnEnable()
    {
        //add events
        if (GameManager.instance.levelManager)
        {
            GameManager.instance.levelManager.onEndGame += OnEndGame;
        }
    }

    void OnDisable()
    {
        //remove events
        if (GameManager.instance.levelManager)
        {
            GameManager.instance.levelManager.onEndGame -= OnEndGame;
        }
    }

    void OnEndGame(bool win)
    {
        //if explode only on lose, return when player win
        if (explodeOnlyOnLose && win)
            return;

        //destroy enemies in scene if necessary
        if (destroyEnemiesInScene)
            GameManager.instance.waveManager.ClearEnemies();

        //foreach cell
        foreach(Cell cell in world.Cells.Values)
        {
            if (cell == null)
                continue;

            //get rigidbody
            Rigidbody rb = cell.GetComponent<Rigidbody>();
            if (rb == null)
                continue;

            //remove parent (because if cube is rotating, it break everything)
            cell.transform.parent = null;

            ExplosionCell(cell, rb);
        }

        //clear dictionary (or rotation will reset parent)
        world.Cells.Clear();
    }

    void ExplosionCell(Cell cell, Rigidbody rb)
    {
        //remove kinematic
        rb.isKinematic = false;

        //get direction explosion from center of the world to the cell + randomizer
        Vector3 direction = (cell.transform.position - transform.position).normalized;
        direction += new Vector3(Random.Range(minRandomizeDirection, maxRandomizeDirection), Random.Range(minRandomizeDirection, maxRandomizeDirection), Random.Range(minRandomizeDirection, maxRandomizeDirection));

        //add explosion force
        rb.AddForce(direction * Random.Range(minForceExplosion, maxForceExplosion), ForceMode.Impulse);

        //randomize torque direction and add explosion torque
        Vector3 torqueDirection = new Vector3(Random.Range(minRandomizeTorqueDirection, maxRandomizeTorqueDirection), Random.Range(minRandomizeTorqueDirection, maxRandomizeTorqueDirection), Random.Range(minRandomizeTorqueDirection, maxRandomizeTorqueDirection));
        rb.AddTorque(torqueDirection * Random.Range(minTorqueExplosion, maxTorqueExplosion), ForceMode.Impulse);
    }
}
