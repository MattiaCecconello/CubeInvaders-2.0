using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("Cube Invaders/Text Change Color")]
public class TextChangeColor : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] Text text;
    [SerializeField] Color selectedColor = Color.black;

    Color normalColor = Color.white;

    public void Awake()
    {
        //get reference to Text Component
        if (text == null)
        {
            text = GetComponent<Text>();
        }

        //get normalColor
        if (text)
        {
            normalColor = text.color;
        }
    }

    public void OnEnable()
    {
        //by default use normalColor
        if (text)
        {
            text.color = normalColor;
        }
    }

    public void OnDisable()
    {
        //reset to normalColor
        if (text)
        {
            text.color = normalColor;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        //use hover color
        if (text)
        {
            text.color = selectedColor;
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        //reset to normal color
        if (text)
        {
            text.color = normalColor;
        }
    }
}
