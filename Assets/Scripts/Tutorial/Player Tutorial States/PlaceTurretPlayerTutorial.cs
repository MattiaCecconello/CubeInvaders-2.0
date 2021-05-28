using System.Collections.Generic;
using UnityEngine;
using redd096;

public class PlaceTurretPlayerTutorial : BasePlayerTutorialState
{
    List<Turret> turretsToBuild;

    bool pressedSelectCell;

    public PlaceTurretPlayerTutorial(StateMachine stateMachine, List<Turret> turretsToBuild) : base(stateMachine)
    {
        this.turretsToBuild = turretsToBuild;
    }

    public override void Enter()
    {
        base.Enter();

        //set build mode
        GameManager.instance.levelManager.SetBuildMode(true);

        //show preview
        GameManager.instance.world.Cells[player.CurrentCoordinates].ShowPreview();
    }

    public override void Execution()
    {
        //this state doesn't need movement or other things
        //base.Execution();

        //check if confirm turret
        if (InputRedd096.GetButtonDown("Confirm Turret"))
        {
            PlaceTurret();
            return;
        }
        //or deny turret
        else if (InputRedd096.GetButtonDown("Deny Turret"))
        {
            StopPlaceTurret();
            return;
        }

        //else select cell
        SelectCell(InputRedd096.GetValue<Vector2>("Select Cell"));
    }

    public override void Exit()
    {
        base.Exit();

        //set build mode
        GameManager.instance.levelManager.SetBuildMode(false);

        //be sure to remove preview
        GameManager.instance.world.Cells[player.CurrentCoordinates].HidePreview();
    }

    #region private API

    void SelectCell(Vector2 movement)
    {
        //check if pressed input or moved analog
        if (movement.magnitude >= player.deadZoneAnalogs && pressedSelectCell == false)
        {
            pressedSelectCell = true;

            //save previous coordinates
            Coordinates previousCoordinates = player.CurrentCoordinates;

            //select cell
            if (Mathf.Abs(movement.y) > Mathf.Abs(movement.x))
            {
                if (movement.y > 0)
                    player.CurrentCoordinates = WorldUtility.SelectCell(player.CurrentCoordinates.face, player.CurrentCoordinates.x, player.CurrentCoordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.up);
                else if (movement.y < 0)
                    player.CurrentCoordinates = WorldUtility.SelectCell(player.CurrentCoordinates.face, player.CurrentCoordinates.x, player.CurrentCoordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.down);
            }
            else
            {
                if (movement.x > 0)
                    player.CurrentCoordinates = WorldUtility.SelectCell(player.CurrentCoordinates.face, player.CurrentCoordinates.x, player.CurrentCoordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.right);
                else if (movement.x < 0)
                    player.CurrentCoordinates = WorldUtility.SelectCell(player.CurrentCoordinates.face, player.CurrentCoordinates.x, player.CurrentCoordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.left);
            }

            //if differents coordinates
            if (previousCoordinates != player.CurrentCoordinates)
            {
                //hide old preview and show new one
                GameManager.instance.world.Cells[previousCoordinates].HidePreview();
                GameManager.instance.world.Cells[player.CurrentCoordinates].ShowPreview();
            }

            //show selector on new coordinates
            GameManager.instance.uiManager.ShowSelector(player.CurrentCoordinates);
        }
        //reset when release input or analog
        else if (movement.magnitude < player.deadZoneAnalogs)
        {
            pressedSelectCell = false;
        }
    }

    #endregion

    void PlaceTurret()
    {
        Cell cell = GameManager.instance.world.Cells[player.CurrentCoordinates];

        //if this is the turret to build - and is not already builded
        if (cell && cell.TurretToCreate == turretsToBuild[0]
            && (cell.turret == null || cell.turret.IsPreview))
        {
            //place turret
            GameManager.instance.world.Cells[player.CurrentCoordinates].Interact();

            //remove from the list
            turretsToBuild.RemoveAt(0);

            //back to build turret state
            player.SetState(new BuildTurretPlayerTutorial(player, turretsToBuild));
        }
    }

    void StopPlaceTurret()
    {
        //back to build turret state
        player.SetState(new BuildTurretPlayerTutorial(player, turretsToBuild));
    }
}
