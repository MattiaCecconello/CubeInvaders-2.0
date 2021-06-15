using UnityEngine;
using redd096;

public class PlayerPlaceTurret : PlayerState
{
    Coordinates coordinates;
    bool pressedSelectCell;

    bool needToReleaseConfirmInput;
    float timeToConfirm;
    float timerDelayReleaseConfirm;

    public PlayerPlaceTurret(StateMachine stateMachine, Coordinates coordinates) : base(stateMachine)
    {
        this.coordinates = coordinates;

        //be sure to reset confirm button
        timeToConfirm = 0;
        needToReleaseConfirmInput = true;
    }

    public override void Enter()
    {
        base.Enter();

        //set build mode
        GameManager.instance.levelManager.SetBuildMode(true);

        //show slider hold button
        GameManager.instance.uiManager.ShowHoldToConfirmTurret(true);

        //show preview
        GameManager.instance.world.Cells[coordinates].ShowPreview();
    }

    public override void Execution()
    {
        base.Execution();

        //check if deny turret
        if (InputRedd096.GetButtonDown("Deny Turret"))
        {
            StopPlaceTurret();
            return;
        }

        //else select cell
        bool changedCoordinates;
        SelectCell(InputRedd096.GetValue<Vector2>("Select Cell"), out changedCoordinates);

        //if changed coordinates, be sure to reset timer confirm turret
        if (changedCoordinates)
        {
            timeToConfirm = 0;

            //player need to release and repress button to confirm turret
            timerDelayReleaseConfirm = 0;
            needToReleaseConfirmInput = true;
            return;
        }

        //check if confirm turret (keep pressed, when finished is confirmed)
        if (CheckConfirmTurret(InputRedd096.GetButton("Confirm Turret")))
        {
            PlaceTurret();
        }
    }

    public override void Exit()
    {
        base.Exit();

        //set build mode
        GameManager.instance.levelManager.SetBuildMode(false);

        //hide slider hold button
        GameManager.instance.uiManager.ShowHoldToConfirmTurret(false);

        //be sure to remove preview
        GameManager.instance.world.Cells[coordinates].HidePreview();
    }

    #region private API

    bool CheckConfirmTurret(bool inputPressed)
    {
        float timeToEnd = GameManager.instance.levelManager.generalConfig.TimeToConfirmTurret;

        //be sure player doesn't need to release input
        if (needToReleaseConfirmInput == false)
        {
            if (inputPressed)
                timerDelayReleaseConfirm = Time.time + GameManager.instance.levelManager.generalConfig.delayReleaseConfirmTurret;   //use a delay, to not stop immediatly when unity see a release button

            //if keeping pressed, update slider
            if (timerDelayReleaseConfirm > Time.time)     //check delay
            {
                timeToConfirm += Time.deltaTime;

                //check if end
                if (timeToConfirm >= timeToEnd)
                {
                    //update UI
                    GameManager.instance.uiManager.UpdateHoldToConfirmTurret(timeToConfirm / timeToEnd);
                    return true;
                }
            }
            //else, reset slider
            else
            {
                timeToConfirm = 0;
            }
        }
        //else check if released input
        else if (inputPressed == false)
            needToReleaseConfirmInput = false;

        //update UI
        GameManager.instance.uiManager.UpdateHoldToConfirmTurret(timeToConfirm / timeToEnd);
        return false;
    }

    void SelectCell(Vector2 movement, out bool changedCoordinates)
    {
        changedCoordinates = false;     //by default, not changed coordinates

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
                changedCoordinates = true;      //set changed coordinates
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

        //reset slider and be sure player need to repress button
        timeToConfirm = 0;
        timerDelayReleaseConfirm = 0;
        needToReleaseConfirmInput = true;
    }

    void StopPlaceTurret()
    {
        //back to strategic state
        player.SetState(new PlayerStrategic(player, coordinates));
    }
}
