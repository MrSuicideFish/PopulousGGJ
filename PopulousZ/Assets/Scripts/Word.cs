using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Word : MonoBehaviour
{
    public RectTransform RectTrans { get; private set; }

    private string MyWord;
    private Text Txt;

    private void Awake( )
    {
        Txt = GetComponentInChildren<Text>( );
        RectTrans = GetComponent<RectTransform>( );
    }

    public string GetWord( )
    {
        return MyWord;
    }

    public bool DoLettersMatch( string word )
    {
        if( word.Length > MyWord.Length )
            return false;

        for( int i = 0; i < word.Length; i++ )
            if( word[ i ] != MyWord[ i ] )
                return false;

        return true;
    }

    public void TypeLetter( int lastIdx )
    {
        string wordBegin = MyWord.Substring( 0, lastIdx );
        string wordEnd = MyWord.Substring( lastIdx );

        Txt.text = "<color=red>" + wordBegin + "</color>" + "<color=black>" + wordEnd + "</color>";
    }

    public void SetWord( string word )
    {
        MyWord = word;
        Txt.text = "<color=black>" + word + "</color>";
    }

    public void ResetWord( )
    {
        SetWord( MyWord );
    }
}
