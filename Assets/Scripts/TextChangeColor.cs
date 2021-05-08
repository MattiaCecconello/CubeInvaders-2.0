using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[AddComponentMenu("Cube Invaders/Text Change Color")]
public class TextChangeColor : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] Text text = default;
    [SerializeField] TextMeshProUGUI textPro = default;
    [SerializeField] Color selectedColor = Color.black;

    Color normalColor = Color.white;

    public void Awake()
    {
        //get reference to Text Component
        if (text == null)
        {
            text = GetComponent<Text>();
        }
        if(textPro == null)
        {
            textPro = GetComponent<TextMeshProUGUI>();
        }

        //get normalColor
        if (text)
        {
            normalColor = text.color;
        }
        if (textPro)
        {
            normalColor = textPro.color;
        }
    }

    public void OnEnable()
    {
        //by default use normalColor
        if (text)
        {
            text.color = normalColor;
        }
        if (textPro)
        {
            textPro.color = normalColor;
        }
    }

    public void OnDisable()
    {
        //reset to normalColor
        if (text)
        {
            text.color = normalColor;
        }
        if (textPro)
        {
            textPro.color = normalColor;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        //use hover color
        if (text)
        {
            text.color = selectedColor;
        }
        if (textPro)
        {
            textPro.color = selectedColor;
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        //reset to normal color
        if (text)
        {
            text.color = normalColor;
        }
        if (textPro)
        {
            textPro.color = normalColor;
        }
    }
}
