using UnityEngine;
using UnityEngine.UI;
using TMPro;
using redd096;

[AddComponentMenu("Cube Invaders/Tutorial/Tutorial Manager")]
[RequireComponent(typeof(ParseInputsTutorial))]
public class TutorialManager : MonoBehaviour
{
    [Header("Text UI")]
    [SerializeField] Text textTutorial = default;
    [SerializeField] TextMeshProUGUI textProTutorial = default;

    [Header("Debug")]
    [ReadOnly] [SerializeField] bool isUsingKeyboard = false;
    [ReadOnly] [SerializeField] string textToShow = "";

    Animator anim;
    ParseInputsTutorial parseInputsTutorial;

    void Awake()
    {
        //get references
        anim = GetComponentInChildren<Animator>();
        parseInputsTutorial = GetComponent<ParseInputsTutorial>();
    }

    void Update()
    {
        //if changed input, update text
        if (IsChangedInputDevice())
        {
            UpdateText();
        }
    }

    #region private API

    bool IsChangedInputDevice()
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
        //parse (current tutorial Text To Show)
        string text = parseInputsTutorial.ParseString(textToShow);

        //update UI
        if (textTutorial)
            textTutorial.text = text;

        if (textProTutorial)
            textProTutorial.text = text;
    }

    #endregion

    #region public API

    public void MoveToNextTutorial()
    {
        //set trigger to next state
        anim.SetTrigger("Next State");
    }

    public void SetTextToShow(string textToShow)
    {
        //set text to show
        this.textToShow = textToShow;

        //show new text
        UpdateText();
    }

    public void FinishTutorials()
    {
        //hide text
        textToShow = "";
        UpdateText();
    }

    #endregion
}
