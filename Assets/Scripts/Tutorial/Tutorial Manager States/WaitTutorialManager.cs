using UnityEngine;
using UnityEngine.InputSystem;

public class WaitTutorialManager : BaseTutorialManagerState
{
    [Header("Time to Wait")]
    [SerializeField] bool useTime = false;
    [SerializeField] float timeToWait = 1;

    [Header("Input to Press")]
    [SerializeField] bool useInput = true;
    [SerializeField] InputActionReference inputToPress = default;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //set player state
        GameManager.instance.player.SetState(new WaitPlayerTutorial(GameManager.instance.player, timeToWait, inputToPress, useTime, useInput));
    }
}
