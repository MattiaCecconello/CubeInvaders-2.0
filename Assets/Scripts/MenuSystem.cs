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
    public GameObject noDamageObject;
    public string necessaryKey;
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

            //if no key, or load is succesfull
            bool isActive = string.IsNullOrWhiteSpace(levelButton.necessaryKey) || Load(levelButton.necessaryKey);

            //try show achievements if there is a save (call also if there is not, so deactive objects)
            ShowAchievements(levelButton);

            //if not active, lock level
            if(isActive == false)
            {
                LockLevel(levelButton);
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
        //try load
        MenuSave load = SaveLoadJSON.Load<MenuSave>(levelButton.button.name);

        //if no damage, active object
        if(load != null && load.noDamage)
        {
            if (levelButton.noDamageObject)
                levelButton.noDamageObject.SetActive(true);
        }
        //else disable object
        else
        {
            if (levelButton.noDamageObject)
                levelButton.noDamageObject.SetActive(false);
        }
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
    public static bool Load(string key)
    {
        MenuSave load = SaveLoadJSON.Load<MenuSave>(key);
        return load != null && load.win;
    }

    /// <summary>
    /// Delete data
    /// </summary>
    public static void Delete(string key)
    {
        SaveLoadJSON.DeleteData(key);
    }

    /// <summary>
    /// Delete every data
    /// </summary>
    public static void DeleteAll()
    {
        SaveLoadJSON.DeleteAll();
    }

    /// <summary>
    /// Unlock every level
    /// </summary>
    public void UnlockEveryLevel()
    {
        //save every necessary key
        foreach (MenuStruct levelButton in levelButtons)
        {
            Save(levelButton.necessaryKey, true, false);
        }
        
        //then reload scene
        SceneLoader.instance.RestartGame();

        //set interactable every button
        //foreach (MenuStruct levelButton in levelButtons)
        //{
        //    levelButton.button.interactable = true;
        //}
    }

    #endregion
}
