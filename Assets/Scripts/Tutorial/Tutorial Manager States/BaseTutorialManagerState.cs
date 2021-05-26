using UnityEngine;

public class BaseTutorialManagerState : StateMachineBehaviour
{
    [Header("Player")]
    [SerializeField] bool canMove = false;
    [SerializeField] bool canSelectCell = false;
    [SerializeField] bool canRotate = false;

    [Space()]
    [TextArea] [SerializeField] string textToShow = "";

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //set tutorial manager and player
        GameManager.instance.tutorialManager.SetTextToShow(textToShow);
        ((PlayerTutorial)GameManager.instance.player).SetPlayer(canMove, canSelectCell, canRotate);
    }
}
