using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Word : MonoBehaviour
{
    /// <summary>
    /// The string word that class has.
    /// </summary>
    private string MyWord;

    /// <summary>
    /// The text component to display MyWord.
    /// </summary>
    private Text Txt;

    private void Awake( )
    {
        Txt = GetComponentInChildren<Text>( );
    }

    public string GetWord( )
    {
        return MyWord;
    }

    public bool DoLettersMatch( string word )
    {
        if( word.Length > MyWord.Length )
            return false;

        // Compare each character in the word passed in with MyWord.
        for( int i = 0; i < word.Length; i++ )
            if( word[ i ] != MyWord[ i ] )
                return false;

        return true;
    }

    /// <summary>
    /// This is where the magic happens.  Create 2 strings of differenct colors based on the index passed in.
    /// The first string will be red (matches the typed word in InGameHUD).
    /// The second string is the remainder that has not been typed yet.
    /// </summary>
    /// <param name="lastIdx"></param>
    public void TypeLetter( int lastIdx )
    {
        string wordBegin = MyWord.Substring( 0, lastIdx );
        string wordEnd = MyWord.Substring( lastIdx );

        Txt.text = "<color=#00AC33FF>" + wordBegin + "</color>" + "<color=black>" + wordEnd + "</color>";
    }

    public void SetWord( string word )
    {
        if(Txt == null)
        {
            Txt = GetComponentInChildren<Text>();
        }
        MyWord = word;
        Txt.text = "<color=black>" + word + "</color>";
    }

    public void ResetWord( )
    {
        SetWord( MyWord );
    }
}