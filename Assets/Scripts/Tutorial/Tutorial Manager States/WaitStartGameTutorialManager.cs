using UnityEngine;

public class WaitStartGameTutorialManager : BaseTutorialManagerState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //add events
        GameManager.instance.levelManager.onStartGame += OnStartGame;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        //remove events
        GameManager.instance.levelManager.onStartGame -= OnStartGame;
    }

    void OnStartGame()
    {
        //move to next tutorial
        GameManager.instance.tutorialManager.MoveToNextTutorial();
    }
}
