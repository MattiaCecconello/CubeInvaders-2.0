﻿using UnityEngine;
using redd096;
using Cinemachine;

[AddComponentMenu("Cube Invaders/Player")]
public class Player : StateMachine
{
    #region lock 60 fps

    [Header("Lock 60 FPS")]
    [SerializeField] bool lock60FPS = false;

    private void OnValidate()
    {
        if (lock60FPS)
            Application.targetFrameRate = 60;
        else
            Application.targetFrameRate = -1;
    }

    #endregion

    [Header("Camera")]
    [SerializeField] float mouseSpeedX = 300;
    [SerializeField] float mouseSpeedY = 2;
    [SerializeField] float gamepadSpeedX = 150;
    [SerializeField] float gamepadSpeedY = 1;
    public bool invertY = false;
    public float speedX => InputRedd096.IsCurrentControlScheme("KeyboardAndMouse") ? mouseSpeedX : gamepadSpeedX;
    public float speedY => InputRedd096.IsCurrentControlScheme("KeyboardAndMouse") ? mouseSpeedY : gamepadSpeedY;

    [Header("Player")]
    [Range(0.1f, 0.9f)] public float deadZoneAnalogs = 0.6f;

    [Header("Debug")]
    [SerializeField] string currentState;

    public CinemachineFreeLook VirtualCam { get; private set; }

    float currentResources;
    public float CurrentResources
    {
        get
        {
            return currentResources;
        }
        set
        {
            currentResources = value;
            GameManager.instance.uiManager.SetResourcesText(currentResources);  //update UI
        }
    }

    //used to come back from pause
    State previousState;

    void Start()
    {
        //get virtual cam
        VirtualCam = FindObjectOfType<CinemachineFreeLook>();

        //by default deactive cinemachine
        VirtualCam.enabled = false;

        //set state and lock mouse
        SetState(new PlayerPause(this));
        Utility.LockMouse(CursorLockMode.Locked);

        AddEvents();

        //set values from options
        SetOptionsValue(SaveLoadJSON.Load<OptionsSave>("Options"));
    }

    void OnDestroy()
    {
        RemoveEvents();
    }

    void Update()
    {
        //if pause game, do not do anything
        if(PauseGame(InputRedd096.GetButtonDown("Pause Button")))
        {
            return;
        }

        //else check if resume game
        ResumeGame(InputRedd096.GetButtonDown("Resume Button"));

        //and update state
        state?.Execution();
    }

    public override void SetState(State stateToSet)
    {
        base.SetState(stateToSet);

        //for debug
        currentState = state?.ToString();
    }

    #region events

    protected virtual void AddEvents()
    {
        GameManager.instance.levelManager.onStartGame += OnStartGame;
        GameManager.instance.levelManager.onStartStrategicPhase += OnStartStrategicPhase;
        GameManager.instance.levelManager.onStartAssaultPhase += OnStartAssaultPhase;
        GameManager.instance.levelManager.onEndGame += OnEndGame;
    }

    protected virtual void RemoveEvents()
    {
        GameManager.instance.levelManager.onStartGame -= OnStartGame;
        GameManager.instance.levelManager.onStartStrategicPhase -= OnStartStrategicPhase;
        GameManager.instance.levelManager.onStartAssaultPhase -= OnStartAssaultPhase;
        GameManager.instance.levelManager.onEndGame -= OnEndGame;
    }

    void OnStartGame()
    {
        //do only if game is not ended
        if (GameManager.instance.levelManager.GameEnded)
            return;

        Vector2Int centerCell = GameManager.instance.world.worldConfig.CenterCell;

        //start in strategic phase
        if (GameManager.instance.levelManager.levelConfig.StartInStrategicPhase)
        {
            SetState(new PlayerStrategic(this, new Coordinates(EFace.front, centerCell)));
        }
        //or in assault phase
        {
            SetState(new PlayerAssault(this, new Coordinates(EFace.front, centerCell)));
        }
    }

    void OnStartStrategicPhase()
    {
        //do only if game is not ended
        if (GameManager.instance.levelManager.GameEnded)
            return;

        Vector2Int centerCell = GameManager.instance.world.worldConfig.CenterCell;

        //if in pause, set as previous state strategic phase (so when remove pause, go to strategic)
        if (state is PlayerPause)
        {
            previousState = new PlayerStrategic(this, new Coordinates(EFace.front, centerCell));
        }
        //else go to strategic, starting from center cell
        else
        {
            SetState(new PlayerStrategic(this, new Coordinates(EFace.front, centerCell)));
        }
    }

    void OnStartAssaultPhase()
    {
        //do only if game is not ended
        if (GameManager.instance.levelManager.GameEnded)
            return;

        Vector2Int centerCell = GameManager.instance.world.worldConfig.CenterCell;

        //if in pause, set as previous state assault phase (so when remove pause, go to assault)
        if (state is PlayerPause)
        {
            previousState = new PlayerAssault(this, new Coordinates(EFace.front, centerCell));
        }
        //else go to assault, starting from center cell
        else
        {
            SetState(new PlayerAssault(this, new Coordinates(EFace.front, centerCell)));
        }
    }

    void OnEndGame(bool win)
    {
        //set pause state and show mouse
        SetState(new PlayerPause(this));
        Utility.LockMouse(CursorLockMode.None);
    }

    #endregion

    #region private API

    bool PauseGame(bool inputPause)
    {
        if(inputPause)
        {
            //if state is place turret && press escape, doesn't pause (we use it to exit from this state)
            if (state.GetType() == typeof(PlayerPlaceTurret) && InputRedd096.IsSameInput("Pause Button", "Deny Turret"))
                return false;

            //if not ended game && time is running && is not end assault phase (showing panel to end level)
            if (GameManager.instance.levelManager.GameEnded == false && Time.timeScale > 0 && GameManager.instance.levelManager.CurrentPhase != EPhase.endAssault)
            {
                SceneLoader.instance.PauseGame();
                return true;
            }
        }

        return false;
    }

    bool ResumeGame(bool input)
    {
        if(input)
        {
            //only if pause state
            if (state.GetType() != typeof(PlayerPause))
                return false;

            //if not ended game && time is paused && is not end assault phase (showing panel to end level)
            if (GameManager.instance.levelManager.GameEnded == false && Time.timeScale <= 0 && GameManager.instance.levelManager.CurrentPhase != EPhase.endAssault)
            {
                //if options menu is active, back to pause menu
                if(GameManager.instance.uiManager.IsActiveOptions())
                {
                    GameManager.instance.uiManager.OptionsMenu(false);
                    return true;
                }    

                //else resume game
                SceneLoader.instance.ResumeGame();
                return true;
            }
        }

        return false;
    }

    #endregion

    #region public API

    public void PausePlayer(bool pause)
    {
        if(pause)
        {
            //pause
            previousState = state;
            SetState(new PlayerPause(this));
        }
        else
        {
            //resume
            SetState(previousState);
        }
    }

    public void SetOptionsValue(OptionsSave optionsSave)
    {
        //set every value from Options class
        if (optionsSave != null)
        {
            mouseSpeedX = optionsSave.mouseX;
            mouseSpeedY = optionsSave.mouseY;
            gamepadSpeedX = optionsSave.gamepadX;
            gamepadSpeedY = optionsSave.gamepadY;
            invertY = optionsSave.invertY;
        }
    }

    #endregion
}