using System.Collections;
using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/Enemy Component/Poison Cell")]
public class PoisonCell : MonoBehaviour
{
    [SerializeField] float poisonTimer = 10;
    [SerializeField] int poisonSpread = 1;
    [SerializeField] bool poisonDestroyTurrets = true;
    [ReadOnly] [SerializeField] bool isFirstPoison = false;

    Coroutine poison_Coroutine;

    public void Init(float poisonTimer, int poisonSpread, bool poisonDestroyTurrets, bool isFirstPoison)
    {
        this.poisonTimer = poisonTimer;
        this.poisonSpread = poisonSpread;
        this.poisonDestroyTurrets = poisonDestroyTurrets;
        this.isFirstPoison = isFirstPoison;
    }

    void Start()
    {
        //start poison timer
        if (poison_Coroutine == null && gameObject.activeInHierarchy)
            poison_Coroutine = StartCoroutine(Poison_Coroutine());
    }

    IEnumerator Poison_Coroutine()
    {
        //wait
        yield return new WaitForSeconds(poisonTimer);

        //poison every cell around and kill this one
        Poison();
    }

    void Poison()
    {
        Cell currentCell = GetComponent<Cell>();

        if(poisonSpread > 0)
        {
            //remove limit spread
            poisonSpread--;

            //foreach cell around
            foreach (Cell cell in GameManager.instance.world.GetCellsAround(currentCell.coordinates))
            {
                //if is alive, poison it
                if (cell.IsAlive)
                {
                    cell.gameObject.AddComponent<PoisonCell>().Init(poisonTimer, poisonSpread, poisonDestroyTurrets, false);
                }
            }
        }

        //and kill this one (or lose game) - only if there is no turret, or is only a preview (impossible i think), or is active poisonDestroyTurrets, or is the first poison (enemy hitted this cell)
        if (currentCell.turret == null || currentCell.turret.IsPreview || poisonDestroyTurrets || isFirstPoison)
            currentCell.KillCell();
    }
}
