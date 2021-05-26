using UnityEngine;

public class SelectCellTutorialManager : BaseTutorialManagerState
{
    [Header("Coordinates to Reach")]
    [SerializeField] bool reachCoordinates = false;
    [SerializeField] Coordinates coordinatesToReach = default;

    [Header("How much Moves Cells")]
    [SerializeField] bool reachNumberOfCells = true;
    [SerializeField] int numberOfCells = 5;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //set player state - using coordinates or number of cells
        GameManager.instance.player.SetState(new SelectCellPlayerTutorial(GameManager.instance.player, coordinatesToReach, numberOfCells, reachCoordinates, reachNumberOfCells));
    }
}
