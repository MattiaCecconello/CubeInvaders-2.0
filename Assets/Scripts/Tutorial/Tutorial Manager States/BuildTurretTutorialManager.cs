using System.Collections.Generic;
using UnityEngine;

public class BuildTurretTutorialManager : BaseTutorialManagerState
{
    [Header("Turrets to Build")]
    [SerializeField] List<Turret> turretsToBuild = new List<Turret>();

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //set player state - build these turrets
        GameManager.instance.player.SetState(new BuildTurretPlayerTutorial(GameManager.instance.player, turretsToBuild));
    }
}
