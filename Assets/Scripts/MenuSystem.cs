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

            //if not active, lock level
            if(isActive == false)
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

                //remove event
                levelButton.button.onClick = new Button.ButtonClickedEvent();
            }
        }
    }

    #region public API

    /// <summary>
    /// Save data
    /// </summary>
    public static void Save(string key, bool win, bool noDamage)
    {
        //PlayerPrefs.SetInt(key, win ? 1 : 0);

        SaveLoadJSON.Save(key, new MenuSave(win, noDamage));
    }

    /// <summary>
    /// Load data
    /// </summary>
    public static bool Load(string key)
    {
        //return PlayerPrefs.GetInt(key, 0) > 0 ? true : false;

        MenuSave load = SaveLoadJSON.Load<MenuSave>(key);
        return load != null && load.win;
    }

    /// <summary>
    /// Delete data
    /// </summary>
    public static void Delete(string key)
    {
        //PlayerPrefs.DeleteKey(key);

        SaveLoadJSON.DeleteData(key);
    }

    /// <summary>
    /// Delete every data
    /// </summary>
    public static void DeleteAll()
    {
        //PlayerPrefs.DeleteAll();

        SaveLoadJSON.DeleteAll();
    }

    /// <summary>
    /// Unlock every level
    /// </summary>
    public void UnlockEveryLevel()
    {
        ////save every necessary key
        //foreach (MenuStruct levelButton in levelButtons)
        //{
        //    Save(levelButton.necessaryKey, true, false);
        //}
        //
        ////then reload scene
        //SceneLoader.instance.RestartGame();

        //set interactable every button
        foreach (MenuStruct levelButton in levelButtons)
        {
            levelButton.button.interactable = true;
        }
    }

    #endregion
}
