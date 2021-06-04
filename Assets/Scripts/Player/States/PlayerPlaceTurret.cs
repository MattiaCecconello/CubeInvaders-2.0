using UnityEngine;
using redd096;

public class PlayerPlaceTurret : PlayerState
{
    Coordinates coordinates;
    bool pressedSelectCell;

    public PlayerPlaceTurret(StateMachine stateMachine, Coordinates coordinates) : base(stateMachine)
    {
        this.coordinates = coordinates;
    }

    public override void Enter()
    {
        base.Enter();

        //set build mode
        GameManager.instance.levelManager.SetBuildMode(true);

        //show preview
        GameManager.instance.world.Cells[coordinates].ShowPreview();
    }

    public override void Execution()
    {
        base.Execution();

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
        GameManager.instance.world.Cells[coordinates].HidePreview();
    }

    #region private API

    void SelectCell(Vector2 movement)
    {
        //check if pressed input or moved analog
        if (movement.magnitude >= player.deadZoneAnalogs && pressedSelectCell == false)
        {
            pressedSelectCell = true;

            //save previous coordinates
            Coordinates previousCoordinates = coordinates;

            //select cell
            if (Mathf.Abs(movement.y) > Mathf.Abs(movement.x))
            {
                if (movement.y > 0)
                    coordinates = WorldUtility.SelectCell(coordinates.face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.up);
                else if (movement.y < 0)
                    coordinates = WorldUtility.SelectCell(coordinates.face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.down);
            }
            else
            {
                if (movement.x > 0)
                    coordinates = WorldUtility.SelectCell(coordinates.face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.right);
                else if (movement.x < 0)
                    coordinates = WorldUtility.SelectCell(coordinates.face, coordinates.x, coordinates.y, WorldUtility.LateralFace(transform), ERotateDirection.left);
            }

            //if differents coordinates
            if (previousCoordinates != coordinates)
            {
                //hide old preview and show new one
                GameManager.instance.world.Cells[previousCoordinates].HidePreview();
                GameManager.instance.world.Cells[coordinates].ShowPreview();
            }

            //show selector on new coordinates
            GameManager.instance.uiManager.ShowSelector(coordinates);
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
        //place turret
        GameManager.instance.world.Cells[coordinates].Interact();

        //hide old preview and show new one
        GameManager.instance.world.Cells[coordinates].HidePreview();
        GameManager.instance.world.Cells[coordinates].ShowPreview();

        //back to strategic state
        //player.SetState(new PlayerStrategic(player, coordinates));
    }

    void StopPlaceTurret()
    {
        //back to strategic state
        player.SetState(new PlayerStrategic(player, coordinates));
    }
}
