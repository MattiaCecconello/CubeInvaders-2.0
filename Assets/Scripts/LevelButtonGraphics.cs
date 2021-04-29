using UnityEngine;

[AddComponentMenu("Cube Invaders/Level Button Graphics")]
public class LevelButtonGraphics : MonoBehaviour
{
    [Header("Level Button")]
    [SerializeField] GameObject noDamageObject = default;

    public void SetNoDamage(bool active)
    {
        //active or deactive No Damage object
        if(noDamageObject)
        {
            noDamageObject.SetActive(active);
        }
    }
}
