using UnityEngine;
using UnityEngine.InputSystem;
using redd096;

public class WaitPlayerTutorial : BasePlayerTutorialState
{
    float timeToEnd;
    InputActionReference inputToPress;
    bool useTime;
    bool useInput;

    public WaitPlayerTutorial(StateMachine stateMachine, float timeToWait, InputActionReference inputToPress, bool useTime, bool useInput) : base(stateMachine)
    {
        timeToEnd = Time.time + timeToWait;
        this.inputToPress = inputToPress;
        this.useTime = useTime;
        this.useInput = useInput;
    }

    public override void Execution()
    {
        base.Execution();

        if (useTime)
        {
            //when time is finished, finish tutorial
            if (Time.time > timeToEnd)
            {
                FinishTutorial();
            }
        }

        if (useInput)
        {
            //if press input, finish tutorial
            if (InputRedd096.GetButtonDown(inputToPress.name))
            {
                FinishTutorial();
            }
        }
    }
}
