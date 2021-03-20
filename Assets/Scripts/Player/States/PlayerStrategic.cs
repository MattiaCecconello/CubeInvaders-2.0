using UnityEngine;
using redd096;

public class PlayerStrategic : PlayerMove
{
    float timeToEndStrategic;

    public PlayerStrategic(StateMachine stateMachine, Coordinates coordinates) : base(stateMachine, coordinates)
    {
        //be sure to reset slider
        timeToEndStrategic = 0;
    }

    public override void Execution()
    {
        base.Execution();

        //if press, enter in build turret state
        if (InputRedd096.GetButtonDown("Build Turret"))
        {
            EnterBuildTurret();
            return;
        }

        //else update strategic slider
        UpdateStrategicSlider(InputRedd096.GetButton("Finish Strategic Phase"));
    }

    #region private API

    void UpdateStrategicSlider(bool inputPressed)
    {
        float timeToEnd = GameManager.instance.levelManager.generalConfig.TimeToEndStrategic;

        //if keeping pressed, update slider
        if (inputPressed)
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

    void EnterBuildTurret()
    {
        //enter in "place turret" state
        player.SetState(new PlayerPlaceTurret(player, coordinates));
    }

    void EndStrategic()
    {
        //end strategic phase
        GameManager.instance.levelManager.EndStrategicPhase();
    }
}
