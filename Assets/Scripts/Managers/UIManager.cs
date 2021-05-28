using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Cube Invaders/Manager/UI Manager")]
public class UIManager : MonoBehaviour
{
    [Header("General")]
    [SerializeField] GameObject pauseMenu = default;
    [SerializeField] GameObject winMenu = default;
    [SerializeField] GameObject loseMenu = default;

    [Header("Main Pause - Options")]
    [SerializeField] GameObject mainPauseMenu = default;
    [SerializeField] GameObject optionsMenu = default;

    [Header("Resources")]
    [SerializeField] Text resourcesText = default;
    [SerializeField] string stringBeforeResources = "Resources: ";
    [SerializeField] [Min(0)] int decimalsResourcesText = 0;
    [SerializeField] Text costText = default;
    [SerializeField] string stringBeforeCost = "Cost: ";
    [SerializeField] string stringBeforeSell = "Sell: ";
    [SerializeField] [Min(0)] int decimalsCostText = 0;

    [Header("Current Level")]
    [SerializeField] Text currentLevelText = default;
    [SerializeField] string currentLevelString = "Level: ";

    [Header("Strategic")]
    [SerializeField] GameObject strategicCanvas = default;
    [SerializeField] Slider readySlider = default;

    [Header("Warning When no Turrets Builded")]
    [SerializeField] [Range(0, 1)] float percentageWarningApparition = 0.1f;
    [SerializeField] GameObject warningObject = default;
    [SerializeField] BuildableObject[] turretsToCheck = default;

    //selector
    GameObject selector;
    GameObject multipleSelector;

    void Start()
    {
        //instantiate and disable selector
        selector = Instantiate(GameManager.instance.levelManager.generalConfig.Selector);
        multipleSelector = Instantiate(GameManager.instance.levelManager.generalConfig.MultipleSelector);
        HideSelector();

        //hide all
        PauseMenu(false);
        EndMenu(false, false);
        SetCostText(false);
        strategicCanvas.SetActive(false);
        HideWarningObject();

        //show default wave
        UpdateCurrentLevelText(GameManager.instance.waveManager.CurrentWave);

        //add events
        AddEvents();
    }

    private void OnDestroy()
    {
        RemoveEvents();
    }

    #region events

    void AddEvents()
    {
        GameManager.instance.levelManager.onStartStrategicPhase += OnStartStrategicPhase;
        GameManager.instance.levelManager.onEndStrategicPhase += OnEndStrategicPhase;
        GameManager.instance.levelManager.onEndGame += OnEndGame;
    }

    void RemoveEvents()
    {
        GameManager.instance.levelManager.onStartStrategicPhase -= OnStartStrategicPhase;
        GameManager.instance.levelManager.onEndStrategicPhase -= OnEndStrategicPhase;
        GameManager.instance.levelManager.onEndGame -= OnEndGame;
    }

    void OnStartStrategicPhase()
    {
        //show strategic canvas
        strategicCanvas.SetActive(true);
    }

    void OnEndStrategicPhase()
    {
        //hide strategic canvas
        strategicCanvas.SetActive(false);
    }

    void OnEndGame(bool win)
    {
        //show end menu
        EndMenu(true, win);
    }

    #endregion

    #region public API

    #region general

    public void PauseMenu(bool active)
    {
        pauseMenu.SetActive(active);
    }

    public void EndMenu(bool active, bool win)
    {
        //if active, be sure pause menu is deactivated
        if (active)
        {
            PauseMenu(false);

            //then show win or lose menu
            if (win)
                winMenu.SetActive(true);
            else
                loseMenu.SetActive(true);
        }
        //else, hide end menus
        else
        {
            winMenu.SetActive(false);
            loseMenu.SetActive(false);
        }
    }

    #endregion

    #region options - pause

    public bool IsActiveOptions()
    {
        return optionsMenu.activeInHierarchy;
    }

    public void OptionsMenu(bool active)
    {
        //hide main and show options
        if(active)
        {
            mainPauseMenu.SetActive(false);
            optionsMenu.SetActive(true);
        }
        //viceversa
        else
        {
            mainPauseMenu.SetActive(true);
            optionsMenu.SetActive(false);
        }
    }

    #endregion

    #region resources

    public void SetResourcesText(float resources)
    {
        //set resources text
        if (resourcesText)
        {
            resourcesText.text = stringBeforeResources + resources.ToString($"F{decimalsResourcesText}");
        }
    }

    public void SetCostText(bool active, bool isBuying = false, float cost = 0)
    {
        //set cost text + active or deactive
        if(costText)
        {
            string stringBefore = isBuying ? stringBeforeCost : stringBeforeSell;

            costText.text = stringBefore + cost.ToString($"F{decimalsCostText}");
            costText.gameObject.SetActive(active);
        }
    }

    #endregion

    #region current level

    public void UpdateCurrentLevelText(int currentWave)
    {
        if (currentLevelText)
        {
            //set text (current wave +1, so player doesn't see wave 0)
            currentLevelText.text = currentLevelString + (currentWave + 1);
        }
    }

    #endregion

    #region strategic

    public void UpdateReadySlider(float value)
    {
        //update slider
        readySlider.value = value;

        //check if show warning object
        CheckWarningObject();
    }

    void CheckWarningObject()
    {
        if (warningObject == null)
            return;

        //when reach percentage
        if (warningObject.activeInHierarchy == false && readySlider.value >= percentageWarningApparition)
        {
            //check every face if there are gatling or rocket
            foreach (EFace face in GameManager.instance.turretsManager.BuildableObjectsOnFace.Keys)
            {
                foreach (BuildableObject buildableObject in GameManager.instance.turretsManager.TurretsOnFace(face))
                {
                    //if there is at least one, no warning to show
                    foreach(BuildableObject turret in turretsToCheck)
                    {
                        if(buildableObject.CellOwner && buildableObject.CellOwner.TurretToCreate == turret)
                        {
                            return;
                        }
                    }
                }
            }

            //if there are no gatling or rocket builded, show warning
            warningObject.SetActive(true);
        }
        //if warning still active when slider reset, hide it
        else if (warningObject.activeInHierarchy && readySlider.value < percentageWarningApparition)
        {
            warningObject.SetActive(false);
        }
    }

    void HideWarningObject()
    {
        if (warningObject == null)
            return;

        //hide warning object
        warningObject.SetActive(false);
    }

    #endregion

    #region selector

    public void ShowSelector(Coordinates coordinates)
    {
        //set size
        float size = GameManager.instance.world.worldConfig.CellsSize;
        selector.transform.localScale = new Vector3(size, size, size);

        //position and rotation of our cell
        selector.transform.position = coordinates.position;
        selector.transform.rotation = coordinates.rotation;

        //active selector
        selector.SetActive(true);

        //if select more cells, show multiple selector
        if (GameManager.instance.levelManager.levelConfig.SelectorSize > 1)
            ShowMultipleSelector(coordinates);
    }

    void ShowMultipleSelector(Coordinates coordinates)
    {
        //cell size * selector size
        float size = GameManager.instance.world.worldConfig.CellsSize * GameManager.instance.levelManager.levelConfig.SelectorSize;
        multipleSelector.transform.localScale = new Vector3(size, size, size);

        //position of our cell + move to select other cells
        Vector3 position = coordinates.position;
        position += MoveSelector(true, coordinates);
        position += MoveSelector(false, coordinates);

        multipleSelector.transform.position = position;
        multipleSelector.transform.rotation = coordinates.rotation;

        //active selector
        multipleSelector.SetActive(true);
    }

    Vector3 MoveSelector(bool useX, Coordinates coordinates)
    {
        //movement for our selector
        Vector3 movement = Vector3.zero;
        int selectorSize = GameManager.instance.levelManager.levelConfig.SelectorSize;

        int value = useX ? coordinates.x : coordinates.y;

        //check if there are enough cells to the right (useX) or up (!useX)
        bool increase = value + selectorSize - 1 < GameManager.instance.world.worldConfig.NumberCells;

        //min (next after our cell) and max (until selector size)
        //or min (from selector cell) and max (next after our cell)
        int min = increase ? value + 1 : value - (selectorSize -1);
        int max = increase ? value + selectorSize : value;

        //increase or decrease
        for (int i = min; i < max; i++)
        {
            //get coordinates using x or y
            Coordinates coordinatesToRotate = useX ? new Coordinates(coordinates.face, i, coordinates.y) : new Coordinates(coordinates.face, coordinates.x, i);

            //if there is a cell
            if (GameManager.instance.world.Cells.ContainsKey(coordinatesToRotate))
            {
                int b = increase ? +1 : -1;
                movement += useX ? Vector3.right * (GameManager.instance.world.worldConfig.CellsSize /2) * b : Vector3.up * (GameManager.instance.world.worldConfig.CellsSize /2) * b;
            }
        }

        //rotate movement to the face
        movement = WorldUtility.RotateTowardsFace(movement, coordinates.face);

        //return movement
        return movement;
    }

    public void HideSelector()
    {
        //hide selector
        selector.SetActive(false);
        multipleSelector.SetActive(false);
    }

    #endregion

    #endregion
}