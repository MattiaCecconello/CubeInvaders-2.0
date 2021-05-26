using UnityEngine;

[AddComponentMenu("Cube Invaders/Tutorial/Player Tutorial")]
public class PlayerTutorial : Player
{
    public Coordinates CurrentCoordinates { get; set; }

    public bool CanMove { get; private set; }
    public bool CanSelectCell { get; private set; }
    public bool CanRotate { get; private set; }

    bool isTutorialFinished;

    void Awake()
    {
        //set default coordinates
        CurrentCoordinates = new Coordinates(EFace.front, GameManager.instance.world.worldConfig.CenterCell);
    }

    public void SetPlayer(bool canMove, bool canSelectCell, bool canRotate)
    {
        //enable/disable camera movement
        if (CanMove != canMove)
            VirtualCam.enabled = canMove;

        //enable/disable selector
        if (canMove || canSelectCell || canRotate)
            GameManager.instance.uiManager.ShowSelector(CurrentCoordinates);
        else
            GameManager.instance.uiManager.HideSelector();

        //set vars
        CanMove = canMove;
        CanSelectCell = canSelectCell;
        CanRotate = canRotate;
    }

    public void FinishTutorialStates()
    {
        //set tutorial finished
        isTutorialFinished = true;
    }

    protected override void OnStartGame()
    {
        //do only if tutorial finished
        if(isTutorialFinished)
            base.OnStartGame();
    }

    protected override void OnStartStrategicPhase()
    {
        //do only if tutorial finished
        if (isTutorialFinished)
            base.OnStartStrategicPhase();
    }

    protected override void OnStartAssaultPhase()
    {
        //do only if tutorial finished
        if (isTutorialFinished)
            base.OnStartAssaultPhase();
    }
}
