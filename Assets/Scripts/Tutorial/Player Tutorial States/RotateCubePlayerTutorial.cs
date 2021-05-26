using redd096;

public class RotateCubePlayerTutorial : BasePlayerTutorialState
{
    ERotateDirection[] rotationsToDo;
    int numberOfRotations;

    bool doRotations;
    bool reachNumberOfRotations;

    int currentRotation;

    public RotateCubePlayerTutorial(StateMachine stateMachine, ERotateDirection[] rotationsToDo, int numberOfRotations, bool doRotations, bool reachNumberOfRotations) : base(stateMachine)
    {
        currentRotation = 0;

        //set references
        this.rotationsToDo = rotationsToDo;
        this.numberOfRotations = numberOfRotations;
        this.doRotations = doRotations;
        this.reachNumberOfRotations = reachNumberOfRotations;
    }

    protected override void DoRotation(ERotateDirection rotateDirection)
    {
        base.DoRotation(rotateDirection);

        //do these rotations
        if (doRotations)
        {
            if (rotationsToDo[currentRotation] == rotateDirection)
            {
                //move to next in the array
                currentRotation++;

                //finished array
                if (currentRotation >= rotationsToDo.Length)
                    FinishTutorial();
            }
        }

        //reach number of rotations
        if(reachNumberOfRotations)
        {
            numberOfRotations--;

            //reached number of rotations
            if (numberOfRotations <= 0)
                FinishTutorial();
        }
    }
}
