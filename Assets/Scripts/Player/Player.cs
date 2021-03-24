using UnityEngine;
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

    void AddEvents()
    {
        GameManager.instance.levelManager.onStartGame += OnStartGame;
        GameManager.instance.levelManager.onStartStrategicPhase += OnStartStrategicPhase;
        GameManager.instance.levelManager.onStartAssaultPhase += OnStartAssaultPhase;
        GameManager.instance.levelManager.onEndGame += OnEndGame;
    }

    void RemoveEvents()
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

        //start in strategic phase
        Vector2Int centerCell = GameManager.instance.world.worldConfig.CenterCell;
        SetState(new PlayerStrategic(this, new Coordinates(EFace.front, centerCell)));
    }

    void OnStartStrategicPhase()
    {
        //do only if game is not ended
        if (GameManager.instance.levelManager.GameEnded)
            return;

        //if in pause, set as previous state strategic phase
        if (state is PlayerPause)
        {
            Vector2Int centerCell = GameManager.instance.world.worldConfig.CenterCell;
            previousState = new PlayerStrategic(this, new Coordinates(EFace.front, centerCell));
        }
        //else go to player move, starting from center cell
        else
        {
            Vector2Int centerCell = GameManager.instance.world.worldConfig.CenterCell;
            SetState(new PlayerStrategic(this, new Coordinates(EFace.front, centerCell)));
        }
    }

    void OnStartAssaultPhase()
    {
        //do only if game is not ended
        if (GameManager.instance.levelManager.GameEnded)
            return;

        //if in pause, set as previous state assault phase
        if (state is PlayerPause)
        {
            Vector2Int centerCell = GameManager.instance.world.worldConfig.CenterCell;
            previousState = new PlayerAssault(this, new Coordinates(EFace.front, centerCell));
        }
        //else go to player move, starting from center cell
        else
        {
            Vector2Int centerCell = GameManager.instance.world.worldConfig.CenterCell;
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

    #endregion
}