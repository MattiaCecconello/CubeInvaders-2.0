using System.Collections.Generic;
using UnityEngine;
using redd096;

public class BasePlayerTutorialState : State
{
    bool alreadyFinished;

    protected PlayerTutorial player;
    protected Transform transform;

    float timerDelayRotateOrSelect;

    public BasePlayerTutorialState(StateMachine stateMachine) : base(stateMachine)
    {
        player = stateMachine as PlayerTutorial;
        transform = player.transform;
    }

    public override void Execution()
    {
        base.Execution();

        if(player.CanMove)
        {
            //move camera
            MoveCamera(InputRedd096.GetActiveControlName("Move Camera"), InputRedd096.GetValue<Vector2>("Move Camera"));

            //when move camera, check if changed face
            CheckChangedFace();
        }

        if (player.CanRotate || player.CanSelectCell)
        {
            //rotate cube or select cell (check if keeping pressed to rotate)
            if (Time.time > timerDelayRotateOrSelect)                               //check delay
            {
                if (InputRedd096.GetButton("Keep Pressed To Rotate"))
                {
                    if(player.CanRotate)
                        RotateCube(InputRedd096.GetValue<Vector2>("Rotate Cube"));
                }
                else
                {
                    if(player.CanSelectCell)
                        SelectCell(InputRedd096.GetValue<Vector2>("Select Cell"));
                }
            }
        }
    }

    protected void FinishTutorial()
    {
        //do only one time
        if (alreadyFinished)
            return;

        alreadyFinished = true;

        //move to next tutorial
        GameManager.instance.tutorialManager.MoveToNextTutorial();
    }

    #region move

    protected virtual void MoveCamera(string activeControlName, Vector2 input)
    {
        //set invert Y
        player.VirtualCam.m_YAxis.m_InvertInput = player.invertY;

        //set max speed
        if (activeControlName == "delta")
        {
            //if delta (so mouse movement) don't use deltaTime
            player.VirtualCam.m_XAxis.m_MaxSpeed = player.speedX;
            player.VirtualCam.m_YAxis.m_MaxSpeed = player.speedY;
        }
        else
        {
            //normally, use deltaTime
            player.VirtualCam.m_XAxis.m_MaxSpeed = player.speedX * Time.deltaTime;
            player.VirtualCam.m_YAxis.m_MaxSpeed = player.speedY * Time.deltaTime;
        }

        //move camera
        player.VirtualCam.m_XAxis.m_InputAxisValue = input.x;
        player.VirtualCam.m_YAxis.m_InputAxisValue = input.y;
    }

    void CheckChangedFace()
    {
        //if change face, reselect center cell and move selector
        EFace face = WorldUtility.SelectFace(transform);

        if (face != player.CurrentCoordinates.face)
        {
            player.CurrentCoordinates = new Coordinates(face, GameManager.instance.world.worldConfig.CenterCell);
            GameManager.instance.uiManager.ShowSelector(player.CurrentCoordinates);
        }
    }

    #endregion

    #region select and rotate

    bool pressedRotateOrSelect = true;

    void RotateCube(Vector2 movement)
    {
        //check if pressed input or moved analog
        if (movement.magnitude >= player.deadZoneAnalogs && pressedRotateOrSelect == false)
        {
            pressedRotateOrSelect = true;

            //check if y or x axis
            if (Mathf.Abs(movement.y) > Mathf.Abs(movement.x))
            {
                if (movement.y > 0)
                    DoRotation(ERotateDirection.up);
                else if (movement.y < 0)
                    DoRotation(ERotateDirection.down);
            }
            else
            {
                if (movement.x > 0)
                    DoRotation(ERotateDirection.right);
                else if (movement.x < 0)
                    DoRotation(ERotateDirection.left);
            }
        }
        //reset when release input or analog
        else if (movement.magnitude < player.deadZoneAnalogs)
        {
            pressedRotateOrSelect = false;
        }
    }

    void SelectCell(Vector2 movement)
    {
        //check if pressed input or moved analog
        if (movement.magnitude >= player.deadZoneAnalogs && pressedRotateOrSelect == false)
        {
            pressedRotateOrSelect = true;

            //check if y or x axis
            if (Mathf.Abs(movement.y) > Mathf.Abs(movement.x))
            {
                if (movement.y > 0)
                    DoSelectionCell(ERotateDirection.up);
                else if (movement.y < 0)
                    DoSelectionCell(ERotateDirection.down);
            }
            else
            {
                if (movement.x > 0)
                    DoSelectionCell(ERotateDirection.right);
                else if (movement.x < 0)
                    DoSelectionCell(ERotateDirection.left);
            }

            //save coordinates and show selector
            GameManager.instance.uiManager.ShowSelector(player.CurrentCoordinates);
        }
        //reset when release input or analog
        else if (movement.magnitude < player.deadZoneAnalogs)
        {
            pressedRotateOrSelect = false;
        }
    }

    protected virtual void DoSelectionCell(ERotateDirection direction)
    {
        timerDelayRotateOrSelect = Time.time + GameManager.instance.levelManager.generalConfig.delayRotateOrSelectCell;     //use a delay, to not call again for error

        //update coordinates
        player.CurrentCoordinates = WorldUtility.SelectCell(WorldUtility.SelectFace(transform), player.CurrentCoordinates.x, player.CurrentCoordinates.y, WorldUtility.LateralFace(transform), direction);
    }

    protected virtual void DoRotation(ERotateDirection rotateDirection)
    {
        timerDelayRotateOrSelect = Time.time + GameManager.instance.levelManager.generalConfig.delayRotateOrSelectCell;     //use a delay, to not call again for error

        //do for number of rotations
        for (int i = 0; i < GameManager.instance.levelManager.levelConfig.NumberRotations; i++)
        {
            //if selector is greater, rotate more cells
            if (GameManager.instance.levelManager.levelConfig.SelectorSize > 1)
            {
                List<Coordinates> coordinatesToRotate = RotateMoreCells(rotateDirection);   //get list of coordinates to rotate
                coordinatesToRotate.Add(player.CurrentCoordinates);                         //add our coordinates
                GameManager.instance.world.PlayerRotate(coordinatesToRotate.ToArray(), WorldUtility.LateralFace(transform), rotateDirection, GameManager.instance.world.worldConfig.RotationTime);
            }
            //else rotate only this cell
            else
            {
                GameManager.instance.world.PlayerRotate(player.CurrentCoordinates, WorldUtility.LateralFace(transform), rotateDirection, GameManager.instance.world.worldConfig.RotationTime);
            }
        }
    }

    List<Coordinates> RotateMoreCells(ERotateDirection rotateDirection)
    {
        int selectorSize = GameManager.instance.levelManager.levelConfig.SelectorSize;
        EFace lookingFace = WorldUtility.LateralFace(transform);

        //check if get cells on x or y
        bool useX = rotateDirection == ERotateDirection.up || rotateDirection == ERotateDirection.down;

        //if rotating up or down face, when looking from right or left, inverse useX
        if (player.CurrentCoordinates.face == EFace.up || player.CurrentCoordinates.face == EFace.down)
            if (lookingFace == EFace.right || lookingFace == EFace.left)
                useX = !useX;

        int value = useX ? player.CurrentCoordinates.x : player.CurrentCoordinates.y;

        //check if there are enough cells to the right (useX) or up (!useX)
        bool increase = value + selectorSize - 1 < GameManager.instance.world.worldConfig.NumberCells;

        //min (next after our cell) and max (until selector size)
        //or min (from selector cell) and max (next after our cell)
        int min = increase ? value + 1 : value - (selectorSize - 1);
        int max = increase ? value + selectorSize : value;

        //increase or decrease
        List<Coordinates> coordinatesToRotate = new List<Coordinates>();
        for (int i = min; i < max; i++)
        {
            //get coordinates using x or y
            Coordinates coords = useX ? new Coordinates(player.CurrentCoordinates.face, i, player.CurrentCoordinates.y) : new Coordinates(player.CurrentCoordinates.face, player.CurrentCoordinates.x, i);

            //if there is a cell, add it
            if (GameManager.instance.world.Cells.ContainsKey(coords))
                coordinatesToRotate.Add(coords);
        }

        return coordinatesToRotate;
    }

    #endregion
}
