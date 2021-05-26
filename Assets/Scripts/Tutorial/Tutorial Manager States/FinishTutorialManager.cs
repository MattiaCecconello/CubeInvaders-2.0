using UnityEngine;

public class FinishTutorialManager : BaseTutorialManagerState
{
    [Header("Wait Player to Finish Strategic Phase?")]
    [SerializeField] bool finishStrategicPhase = true;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //set player state - finish tutorial or finish strategic phase
        GameManager.instance.player.SetState(new FinishTutorialPlayerTutorial(GameManager.instance.player, finishStrategicPhase));
    }
}
