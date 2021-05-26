using UnityEngine;

public class RotateCubeTutorialManager : BaseTutorialManagerState
{
    [Header("Do these Rotations")]
    [SerializeField] bool doRotations = false;
    [SerializeField] ERotateDirection[] rotationsToDo = default;

    [Header("How much Rotates to Do")]
    [SerializeField] bool reachNumberOfRotations = true;
    [SerializeField] int numberOfRotations = 5;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //set player state - do these rotations or reach number of rotations
        GameManager.instance.player.SetState(new RotateCubePlayerTutorial(GameManager.instance.player, rotationsToDo, numberOfRotations, doRotations, reachNumberOfRotations));
    }
}
