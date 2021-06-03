using System.Collections.Generic;
using UnityEngine;
using redd096;

#region structs

public struct ParseStruct
{
    public string textToReplace;
    public string inputName;
    public int inputIndex;

    public ParseStruct(string textToReplace, string inputName, int inputIndex)
    {
        this.textToReplace = textToReplace;
        this.inputName = inputName;
        this.inputIndex = inputIndex;
    }
}

[System.Serializable]
public struct ReplaceNameStruct
{
    public string nameToReplace;
    public string nameToShow;
}

#endregion

[AddComponentMenu("Cube Invaders/Tutorial/Parse Inputs Tutorial")]
public class ParseInputsTutorial : MonoBehaviour
{
    [Header("Use Display Name or Normal Name")]
    [SerializeField] bool useDisplayName = true;

    [Header("Replace some names with others")]
    [SerializeField] ReplaceNameStruct[] replaceNames = default;

    string textToShow;

    List<ParseStruct> stringsToReplace = new List<ParseStruct>();
    int startIndex;
    int slashIndex;
    int endIndex;

    public string ParseString(string textToShow)
    {
        //set text to show
        this.textToShow = textToShow;

        //find every string to replace
        FindStringsToReplace();

        //replace every string
        ReplaceStrings();

        //return new text with inputs
        return this.textToShow;
    }

    #region private API

    void FindStringsToReplace()
    {
        //clear vars
        stringsToReplace.Clear();
        startIndex = -1;
        slashIndex = -1;
        endIndex = -1;

        for (int i = 0; i < textToShow.Length; i++)
        {
            //set start index
            if (textToShow[i] == '{')
            {
                startIndex = i;
            }
            //set slash index
            else if(textToShow[i] == '/')
            {
                slashIndex = i;
            }
            //end index
            else if (textToShow[i] == '}')
            {
                endIndex = i;

                //add to the list
                AddToList();
            }
        }
    }

    void AddToList()
    {
        //=========================================================================================================================================================
        //with slash

        //14 chars example
        //3 parole inutili  (0 - 2)
        //1 graffa inizio   (3)             start index
        //3 name            (4 - 6)
        //1 slash           (7)             slash index
        //2 numeri          (8 - 9)
        //1 graffa fine     (10)            end index
        //3 parole inutili  (11 - 13)

        //to parse
        //subString(3, 8)                   from char 3, long 8 -> text to replace
        //subString(4, 3)                   from char 4, long 3 -> input name
        //int.Parse( subString(8, 2) )      from char 8, long 2 -> input index

        //so
        //subString(start, end - start + 1)                         from start to end (+1, because is length and not char position)
        //subString(start + 1, slash - start - 1)                   after brace (start +1), length from slash to start (-1 to remove start brace)
        //int.Parse( subString(slash + 1, end - slash - 1) )        after slash (slash +1), length from last brace to slash (-1 to remove slash)

        //=========================================================================================================================================================
        //without slash

        //11 chars example
        //3 parole inutili  (0 - 2)
        //1 graffa inizio   (3)             start index
        //3 name            (4 - 6)
        //1 graffa fine     (7)             end index
        //3 parole inutili  (8 - 10)

        //to parse
        //subString(3, 5)                   from char 3, long 5 -> text to replace
        //subString(4, 3)                   from char 4, long 3 -> input name

        //so
        //subString(start, end - start + 1)                         from start to end (+1, because is length and not char position)
        //subString(start + 1, end - start - 1)                     after brace (start +1), length from end to start (-1 to remove start brace)

        if (slashIndex >= 0)
        {
            stringsToReplace.Add(new ParseStruct(
                textToShow.Substring(startIndex, endIndex - startIndex + 1),                        //text to replace
                textToShow.Substring(startIndex + 1, slashIndex - startIndex - 1),                  //input name
                int.Parse(textToShow.Substring(slashIndex + 1, endIndex - slashIndex - 1))));       //input index
        }
        else
        {
            stringsToReplace.Add(new ParseStruct(
                textToShow.Substring(startIndex, endIndex - startIndex + 1),                        //text to replace
                textToShow.Substring(startIndex + 1, endIndex - startIndex - 1),                    //input name
                0));                                                                                //input index (0)
        }
    }

    void ReplaceStrings()
    {        
        //replace every string
        foreach (ParseStruct s in stringsToReplace)
        {
            //use display name or control name
            string nameToShow = useDisplayName ? InputRedd096.GetControlDisplayName(s.inputName, s.inputIndex) : InputRedd096.GetControlName(s.inputName, s.inputIndex);

            foreach(ReplaceNameStruct replaceName in replaceNames)
            {
                //if this name is to replace
                if(InputRedd096.GetControlName(s.inputName, s.inputIndex).Equals(replaceName.nameToReplace))
                {
                    //replace with new one
                    nameToShow = replaceName.nameToShow;
                    break;
                }
            }

            //replace string with display name or control name
            textToShow = textToShow.Replace(s.textToReplace, nameToShow);
        }
    }

    #endregion
}
