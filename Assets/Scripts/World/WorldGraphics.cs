using UnityEngine;

[AddComponentMenu("Cube Invaders/World/World Graphics")]
public class WorldGraphics : MonoBehaviour
{
    [Header("Explode only when lose or also win?")]
    [SerializeField] bool explodeOnlyOnLose = true;

    [Header("Destroy every enemy in scene")]
    [SerializeField] bool destroyEnemiesInScene = true;

    [Header("Explosion")]
    [SerializeField] float forceExplosion = 10;

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

            //get direction explosion from center of the world to the cell
            Vector3 direction = (cell.transform.position - transform.position).normalized;

            //remove kinematic and add explosion force
            rb.isKinematic = false;
            rb.AddForce(direction * forceExplosion, ForceMode.Impulse);
        }

        //clear dictionary (or rotation will reset parent)
        world.Cells.Clear();
    }
}
