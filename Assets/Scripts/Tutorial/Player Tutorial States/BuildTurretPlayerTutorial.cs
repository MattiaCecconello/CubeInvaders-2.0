using System.Collections.Generic;
using redd096;

public class BuildTurretPlayerTutorial : BasePlayerTutorialState
{
    List<Turret> turretsToBuild;

    public BuildTurretPlayerTutorial(StateMachine stateMachine, List<Turret> turretsToBuild) : base(stateMachine)
    {
        //set references
        this.turretsToBuild = new List<Turret>(turretsToBuild);
    }

    public override void Enter()
    {
        base.Enter();

        //if finished turrets to build, finish tutorial
        //(check in enter, because come back here after builded turret from PlaceTurretState)
        if (turretsToBuild == null || turretsToBuild.Count <= 0)
            FinishTutorial();
    }

    public override void Execution()
    {
        base.Execution();

        if (turretsToBuild == null || turretsToBuild.Count <= 0)
            return;

        //if press, enter in build turret state
        if (InputRedd096.GetButtonDown("Build Turret"))
        {
            //enter in "place turret" state
            player.SetState(new PlaceTurretPlayerTutorial(player, turretsToBuild));
        }
    }
}
