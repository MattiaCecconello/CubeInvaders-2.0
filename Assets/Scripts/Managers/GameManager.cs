using UnityEngine;
using redd096;
using System.Collections.Generic;

[AddComponentMenu("Cube Invaders/Manager/Game Manager")]
[DefaultExecutionOrder(-100)]
public class GameManager : Singleton<GameManager>
{
    [Header("Important")]
    [SerializeField] bool canSaveWorld = true;

    public UIManager uiManager { get; private set; }
    public Player player { get; private set; }
    public World world { get; private set; }
    public LevelManager levelManager { get; private set; }
    public WaveManager waveManager { get; private set; }

    protected override void SetDefaults()
    {
        //get references
        uiManager = FindObjectOfType<UIManager>();
        player = FindObjectOfType<Player>();
        world = FindObjectOfType<World>();
        levelManager = FindObjectOfType<LevelManager>();
        waveManager = FindObjectOfType<WaveManager>();
    }

    #region old, used from buttons UI

    public void UpdateLevel(LevelConfig levelConfig)
    {
        //update level config
        instance.levelManager.UpdateLevel(levelConfig);
    }

    public void SetWave(int wave)
    {
        //set wave
        instance.waveManager.CurrentWave = wave;

        //end wave and start strategic phase after few seconds
        if (instance.levelManager.CurrentPhase == EPhase.assault)
        {
            instance.levelManager.EndAssaultPhase();
        }
        //or start immediatly strategic phase
        else
        {
            instance.levelManager.StartStrategicPhase();
        }

        //resume game
        SceneLoader.instance.ResumeGame();
    }

    public void NextWave()
    {
        //increase wave +1, than set wave
        SetWave(instance.waveManager.CurrentWave + 1);
    }

    #endregion

    #region public API

    public void SaveWorld()
    {
        //only if can save world
        if (canSaveWorld == false)
            return;

        //create a list with every cell
        List<Cell> cellsToSave = new List<Cell>();
        foreach (Cell cell in world.Cells.Values)
            cellsToSave.Add(cell);

        //save
        SaveLoadJSON.Save(world.worldConfig.name, new WorldSave(cellsToSave));
    }

    #endregion
}