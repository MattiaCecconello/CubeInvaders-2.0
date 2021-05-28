using UnityEngine;
using redd096;

public enum EPhase
{
    strategic, assault, endStrategic, endAssault
}

[AddComponentMenu("Cube Invaders/Manager/Level Manager")]
public class LevelManager : MonoBehaviour
{
    [Header("Important")]
    public LevelConfig levelConfig;
    public GeneralConfig generalConfig;

    public System.Action onStartGame;
    public System.Action onStartStrategicPhase;
    public System.Action onEndStrategicPhase;
    public System.Action onStartAssaultPhase;
    public System.Action onEndAssaultPhase;
    public System.Action<bool> onEndGame;

    [Header("Debug")]
    [ReadOnly] public EPhase CurrentPhase = EPhase.strategic;

    public bool GameEnded { get; private set; }

    bool noDamage = true;

    void Start()
    {
        //check if randomize world
        if (levelConfig && levelConfig.RandomizeWorldAtStart)
        {
            GameManager.instance.world.RandomRotate();
        }
        else
        {
            //else start game after few seconds
            Invoke("StartGame", 1);
        }
    }

    #region events

    public void StartStrategicPhase()
    {
        onStartStrategicPhase?.Invoke();

        CurrentPhase = EPhase.strategic;
    }

    public void EndStrategicPhase()
    {
        if (CurrentPhase == EPhase.strategic)
        {
            CurrentPhase = EPhase.endStrategic;

            onEndStrategicPhase?.Invoke();

            Invoke("StartAssaultPhase", 1);
        }
    }

    public void StartAssaultPhase()
    {
        onStartAssaultPhase?.Invoke();

        CurrentPhase = EPhase.assault;
    }

    public void EndAssaultPhase()
    {
        if (CurrentPhase == EPhase.assault)
        {
            CurrentPhase = EPhase.endAssault;

            onEndAssaultPhase?.Invoke();

            Invoke("StartStrategicPhase", 1);
        }
    }

    #endregion

    #region public API

    public void StartGame()
    {
        GameEnded = false;

        //call event
        onStartGame?.Invoke();

        //start in strategic
        if (levelConfig.StartInStrategicPhase)
            StartStrategicPhase();
        //or start in assault
        else
            EndStrategicPhase();
    }

    public void EndGame(bool win)
    {
        //do only one time
        if (GameEnded)
            return;

        GameEnded = true;

        //save using scene name
        MenuSystem.Save(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, win, noDamage);

        //if win level, save world
        if(win)
            GameManager.instance.SaveWorld();

        //call event
        onEndGame?.Invoke(win);
    }

    public void UpdateLevel(LevelConfig levelConfig)
    {
        //update level config
        this.levelConfig = levelConfig;
    }

    public void GetDamage()
    {
        //set got damage in this level (used for bonus when finish level with no hit)
        if (noDamage)
        {
            noDamage = false;
        }
    }

    public void SaveEndBossLevel()
    {
        //save using scene name
        MenuSystem.Save(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, true, noDamage);    //this level is ended, because player killed boss
    }

    #endregion
}
