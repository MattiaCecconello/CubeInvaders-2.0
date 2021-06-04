using UnityEngine;
using UnityEngine.UI;
using TMPro;

[AddComponentMenu("Cube Invaders/Level Button Graphics")]
public class LevelButtonGraphics : MonoBehaviour
{
    [Header("Level Button")]
    [SerializeField] GameObject noDamageObject = default;

    [Header("Boss Button Locked")]
    [SerializeField] GameObject objectToShowWhenLocked = default;
    [SerializeField] Text textToSet = default;
    [SerializeField] TextMeshProUGUI textProToSet = default;
    [SerializeField] string afterText = " / 19";

    #region public API

    public void SetNoDamage(bool active)
    {
        //active or deactive No Damage object
        if(noDamageObject)
        {
            noDamageObject.SetActive(active);
        }
    }

    public void SetBossLockStatus(bool locked, int numberAchievements)
    {
        //active object when locked
        if (objectToShowWhenLocked)
            objectToShowWhenLocked.SetActive(locked);

        //if text, active when locked and show number of achievements
        if(textToSet)
        {
            textToSet.text = numberAchievements.ToString() + afterText;
            textToSet.gameObject.SetActive(locked);
        }

        //if text mesh pro, active when locked and show number of achievements
        if (textProToSet)
        {
            textProToSet.text = numberAchievements.ToString() + afterText;
            textProToSet.gameObject.SetActive(locked);
        }
    }

    #endregion
}
