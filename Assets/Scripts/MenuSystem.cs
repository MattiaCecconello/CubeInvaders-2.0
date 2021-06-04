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
                    SetLockStatus(levelButton.button, true);
                }
            }
            //if boss level
            else
            {
                int currentAchievements = 0;
                foreach (MenuStruct levelForBoss in levelButtons)
                {
                    //check every level is saved and with achievement completed
                    if (Load(levelForBoss.necessaryKey, true))
                    {
                        currentAchievements++;
                    }
                }

                //if has not every achievement (-1 to remove this button), lock it - boss button has a function more to lock it
                SetLockStatus(levelButton.button, currentAchievements < levelButtons.Length -1);
                levelButton.button.GetComponent<LevelButtonGraphics>()?.SetBossLockStatus(currentAchievements < levelButtons.Length -1, currentAchievements);
            }
        }
    }

    #region private API

    public void SetLockStatus(Button button, bool locked)
    {
        //set interactable
        button.interactable = !locked;

        //change color on disable
        if (changeColor)
        {
            //ColorBlock colorBlock = button.colors;
            //colorBlock.disabledColor = colorOnDisable;
            //
            //button.colors = colorBlock;

            button.GetComponent<Image>().color = locked ? colorOnDisable : Color.white;
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
        //do only if there is a key to save
        if (string.IsNullOrWhiteSpace(key))
            return;

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
        //    //to unlock boss level, is necessary to have every achievement
        //    Save(levelButton.necessaryKey, true, true);
        //}
        //
        ////then reload scene
        //SceneLoader.instance.RestartGame();

        //unlock every button
        foreach (MenuStruct levelButton in levelButtons)
        {
            SetLockStatus(levelButton.button, false);
        
            //if boss level, set lock status false also to boss function
            if(levelButton.isBossLevel)
                levelButton.button.GetComponent<LevelButtonGraphics>()?.SetBossLockStatus(false, 0);
        }
    }

    #endregion
}
