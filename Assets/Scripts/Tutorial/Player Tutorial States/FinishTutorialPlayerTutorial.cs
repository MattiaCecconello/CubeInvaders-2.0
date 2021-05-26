using UnityEngine;
using redd096;

public class FinishTutorialPlayerTutorial : BasePlayerTutorialState
{
    bool finishStrategicPhase;

    float timeToEndStrategic;
    float timerDelayReleaseStrategic;

    public FinishTutorialPlayerTutorial(StateMachine stateMachine, bool finishStrategicPhase) : base(stateMachine)
    {
        //set references
        this.finishStrategicPhase = finishStrategicPhase;

        //be sure to reset slider
        timeToEndStrategic = 0;
    }

    public override void Enter()
    {
        base.Enter();

        //if no need to wait player to finish strategic phase, finish immediatly the tutorial
        if (finishStrategicPhase == false)
        {
            GameManager.instance.tutorialManager.FinishTutorials();
            player.FinishTutorialStates();
            player.SetState(new PlayerStrategic(player, player.CurrentCoordinates));
        }
    }

    public override void Execution()
    {
        base.Execution();

        //update strategic slider
        UpdateStrategicSlider(InputRedd096.GetButton("Finish Strategic Phase"));
    }

    #region private API

    void UpdateStrategicSlider(bool inputPressed)
    {
        float timeToEnd = GameManager.instance.levelManager.generalConfig.TimeToEndStrategic;

        if (inputPressed)
            timerDelayReleaseStrategic = Time.time + GameManager.instance.levelManager.generalConfig.delayReleaseFinishStrategicPhase;      //use a delay, to not stop immediatly when unity see a release button

        //if keeping pressed, update slider
        if (timerDelayReleaseStrategic > Time.time)     //check delay
        {
            timeToEndStrategic += Time.deltaTime;

            //check if end
            if (timeToEndStrategic >= timeToEnd)
            {
                EndStrategic();
            }
        }
        //else, reset slider
        else
        {
            timeToEndStrategic = 0;
        }

        //update UI
        GameManager.instance.uiManager.UpdateReadySlider(timeToEndStrategic / timeToEnd);
    }

    #endregion

    void EndStrategic()
    {
        //finish the tutorial
        GameManager.instance.tutorialManager.FinishTutorials();
        player.FinishTutorialStates();
        player.SetState(new PlayerStrategic(player, player.CurrentCoordinates));

        //end strategic phase
        GameManager.instance.levelManager.EndStrategicPhase();
    }
}
