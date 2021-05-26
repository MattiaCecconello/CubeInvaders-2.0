using UnityEngine;
using redd096;

public class MoveCameraPlayerTutorial : BasePlayerTutorialState
{
    bool reachObject;

    GameObject objectToReach;
    Vector3 approxMin;
    Vector3 approxMax;

    float howMuchMovesCamera;

    public MoveCameraPlayerTutorial(StateMachine stateMachine, GameObject objectToReach, Vector3 approxMin, Vector3 approxMax) : base(stateMachine)
    {
        reachObject = true;

        //set references
        this.objectToReach = objectToReach;
        this.approxMin = approxMin;
        this.approxMax = approxMax;
    }

    public MoveCameraPlayerTutorial(StateMachine stateMachine, float howMuchMovesCamera) : base(stateMachine)
    {
        reachObject = false;

        //set references
        this.howMuchMovesCamera = howMuchMovesCamera;
    }

    public override void Execution()
    {
        base.Execution();

        if (reachObject)
        {
            //if object is deleted, finish tutorial
            if (objectToReach == null)
            {
                FinishTutorial();
                return;
            }

            //if reach destination, finish tutorial
            if (ReachedDestination())
            {
                FinishTutorial();
            }
        }
        else
        {
            if(ReachedQuantityMovement())
            {
                FinishTutorial();
            }
        }
    }

    protected override void MoveCamera(string activeControlName, Vector2 input)
    {
        base.MoveCamera(activeControlName, input);

        //set max speed
        if (activeControlName == "delta")
        {
            //if delta (so mouse movement) don't use deltaTime
            howMuchMovesCamera -= player.speedX * Mathf.Abs(input.x);
            howMuchMovesCamera -= player.speedY * Mathf.Abs(input.y);
        }
        else
        {
            //normally, use deltaTime
            howMuchMovesCamera -= player.speedX * Mathf.Abs(input.x) * Time.deltaTime;
            howMuchMovesCamera -= player.speedY * Mathf.Abs(input.y) * Time.deltaTime;
        }
    }

    #region private API

    bool ReachedDestination()
    {
        Vector3 currentPosition = stateMachine.transform.position;
        Vector3 positionToReach = objectToReach.transform.position;

        //X
        if (currentPosition.x > positionToReach.x + approxMax.x
            && currentPosition.x < positionToReach.x + approxMin.x)
        {
            //Y
            if (currentPosition.y > positionToReach.y + approxMax.y
                && currentPosition.y < positionToReach.y + approxMin.y)
            {
                //Z
                if (currentPosition.z > positionToReach.z + approxMax.z
                    && currentPosition.z < positionToReach.z + approxMin.z)
                {
                    return true;
                }
            }
        }

        return false;
    }

    bool ReachedQuantityMovement()
    {
        //check if reached necessary movement
        if (howMuchMovesCamera <= 0)
            return true;

        return false;
    }

    #endregion
}
