using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using redd096;
using TMPro;

[AddComponentMenu("Cube Invaders/Tutorial/Tutorial Manager")]
[RequireComponent(typeof(ParseInputsTutorial))]
public class TutorialManager : MonoBehaviour
{
    [Header("Text UI")]
    [SerializeField] Text textTutorial = default;
    [SerializeField] TextMeshProUGUI textProTutorial = default;

    //TODO
    //creare lista di tutorial
    //ogni tutorial avrà string (se deve premere un bottone), float (se deve attendere tempo), int (se deve ruotare cubo), trigger (se deve andare a un trigger)
    //+ una stringa che sarà il text da mostrare (al posto della string debug qua sotto, si controllerà ogni volta che si attiva un tutorial, la string di quel tutorial)

    [Header("Debug")]
    [SerializeField] string debug = "";
    [ReadOnly] [SerializeField] bool isUsingKeyboard = false;

    ParseInputsTutorial parseInputsTutorial;

    void Awake()
    {
        //get references
        parseInputsTutorial = GetComponent<ParseInputsTutorial>();
    }

    void Update()
    {
        //update text
        if (ChangedInputDevice())
        {
            UpdateText();
        }
    }

    #region private API

    bool ChangedInputDevice()
    {
        //if device didn't change, return
        bool currentlyUsingKeyboard = InputRedd096.IsCurrentControlScheme("KeyboardAndMouse");
        if (isUsingKeyboard == currentlyUsingKeyboard)
            return false;

        //else change inputs
        isUsingKeyboard = currentlyUsingKeyboard;

        return true;
    }

    void UpdateText()
    {
        //parse
        string text = parseInputsTutorial.ParseString(debug);

        //update UI
        if (textTutorial)
            textTutorial.text = text;

        if (textProTutorial)
            textProTutorial.text = text;
    }

    #endregion
}
