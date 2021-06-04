using UnityEngine;
using UnityEngine.UI;
using redd096;

#region save class

[System.Serializable]
public class MenuSave
{
    public bool win;
    public bool noDamage;

    public MenuSave(bool win, bool noDamage)
    {
        this.win = win;
        this.noDamage = noDamage;
    }
}

#endregion

[System.Serializable]
public struct MenuStruct
{
    public Button button;
    public string necessaryKey;
    public bool isBossLevel;
    public string[] levelsForNoDamage;
}

[AddComponentMenu("Cube Invaders/Menu System")]
public class MenuSystem : MonoBehaviour
{
    [Header("Disabled Buttons")]
    [SerializeField] bool setNotInteractable = false;
    [SerializeField] bool changeColor = true;
    [CanShow("changeColor")] [SerializeField] Color colorOnDisable = Color.red;

    [Header("Menu")]
    [SerializeField] MenuStruct[] levelButtons = default;

    void Start()
    {
        foreach(MenuStruct levelButton in levelButtons)
        {
            //skip not setted button
            if (levelButton.button == null)
                continue;

            //try show achievements if there is a save (call also if there is not, so deactive objects)
            ShowAchievements(levelButton);

            //if normal level
            if(levelButton.isBossLevel == false)
            {
                //check string is empty or level is won, else lock button
                if ((string.IsNullOrWhiteSpace(levelButton.necessaryKey) || Load(levelButton.necessaryKey)) == false)
                {
                    LockLevel(levelButton);
                }
            }
            //if boss level
            else
            {
                foreach (MenuStruct levelForBoss in levelButtons)
                {
                    //check every level is saved and with achievement completed, else lock button
                    if (Load(levelForBoss.necessaryKey, true) == false)
                    {
                        LockLevel(levelButton);
                        break;
                    }
                }
            }
        }
    }

    #region private API

    void LockLevel(MenuStruct levelButton)
    {
        //set interactable
        if (setNotInteractable)
            levelButton.button.interactable = false;

        //change color on disable
        if (changeColor)
        {
            //ColorBlock colorBlock = levelButton.button.GetComponent<Button>().colors;
            //colorBlock.disabledColor = colorOnDisable;
            //
            //levelButton.button.GetComponent<Button>().colors = colorBlock;

            levelButton.button.GetComponent<Image>().color = colorOnDisable;
        }
    }

    void ShowAchievements(MenuStruct levelButton)
    {
        //check every level in the list, or check button name as level if list is empty
        string[] levelsToCheck = levelButton.levelsForNoDamage == null || levelButton.levelsForNoDamage.Length <= 0 ? new string[1] { levelButton.button.name } : levelButton.levelsForNoDamage;

        //check no damage achievement
        bool noDamage = true;

        foreach (string levelToCheck in levelsToCheck)
        {
            //check every level is saved and with achievement completed, else set it to false
            if (Load(levelToCheck, true) == false)
            {
                noDamage = false;
                break;
            }
        }

        //if no damage, active object, else disable it
        levelButton.button.GetComponent<LevelButtonGraphics>()?.SetNoDamage(noDamage);
    }

    #endregion

    #region public API

    /// <summary>
    /// Save data
    /// </summary>
    public static void Save(string key, bool win, bool noDamage)
    {
        //try load
        MenuSave load = SaveLoadJSON.Load<MenuSave>(key);

        //if nothing saved, create new save
        if (load == null)
        {
            SaveLoadJSON.Save(key, new MenuSave(win, noDamage));
        }
        //else - try save without overwrite already won levels
        else
        {
            //if saved as lost, try save a win
            if(load.win == false)
            {
                if(win)
                    SaveLoadJSON.Save(key, new MenuSave(win, noDamage));
            }
            //if saved a win but with damage, try save with no damage
            else if(load.noDamage == false)
            {
                if(noDamage)
                    SaveLoadJSON.Save(key, new MenuSave(win, noDamage));
            }
        }
    }

    /// <summary>
    /// Load data
    /// </summary>
    public static bool Load(string key, bool checkAchievement = false)
    {
        MenuSave load = SaveLoadJSON.Load<MenuSave>(key);

        //check if won level
        if (checkAchievement == false)
            return load != null && load.win;
        //or check if level has achievement
        else
            return load != null && load.noDamage;
    }

    /// <summary>
    /// Delete data
    /// </summary>
    public static void Delete(string key)
    {
        //delete key
        SaveLoadJSON.DeleteData(key);

        //then reload scene
        SceneLoader.instance.RestartGame();
    }

    /// <summary>
    /// Delete every data
    /// </summary>
    public static void DeleteAll()
    {
        //delete all
        SaveLoadJSON.DeleteAll();

        //then reload scene
        SceneLoader.instance.RestartGame();
    }

    /// <summary>
    /// Unlock every level
    /// </summary>
    public void UnlockEveryLevel()
    {
        ////save every necessary key
        //foreach (MenuStruct levelButton in levelButtons)
        //{
        //    //per sbloccare il livello del boss, è necessario avere tutti gli achievement
        //    Save(levelButton.necessaryKey, true, true);
        //}
        //
        ////then reload scene
        //SceneLoader.instance.RestartGame();

        //set interactable every button
        foreach (MenuStruct levelButton in levelButtons)
        {
            levelButton.button.interactable = true;

            if(changeColor)
                levelButton.button.GetComponent<Image>().color = Color.white;
        }
    }

    #endregion
}
