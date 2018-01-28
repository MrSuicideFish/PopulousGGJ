using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent( typeof( Text ) )]
public class FallingWord : MonoBehaviour
{
    /// <summary>
    /// How fast will this object fall from top of the screen to bottm of the screen.
    /// </summary>
    [Range( 5.0f, 30.0f )]
    public float FallSpeed = 5.0f;

    public RectTransform RectTrans { get; private set; }
    public int CharIndex { get; private set; }

    private string Word;
    private Text Txt;

    private void Awake( )
    {
        Txt = GetComponent<Text>( );
        RectTrans = GetComponent<RectTransform>( );
        CharIndex = 0;
    }

    private void Update( )
    {
        transform.position += Vector3.down * Time.deltaTime * FallSpeed;
    }

    public bool CompareString( string other )
    {
        if( other.Length > Word.Length )
            return false;

        for( int i = 0; i < other.Length; i++ )
            if( other[ i ] != Word[ i ] )
                return false;

        return true;
    }

    public string GetWord( )
    {
        return Txt.text;
    }

    public char GetCurrentLetter( )
    {
        return Word[ CharIndex ];
    }

    public bool WordFinished( )
    {
        return CharIndex >= Word.Length;
    }

    public void TypeLetter( )
    {
        CharIndex++;

        string wordBegin = Word.Substring( 0, CharIndex );
        string wordEnd = Word.Substring( CharIndex );

        Txt.text = "<color=red>" + wordBegin + "</color>" + "<color=white>" + wordEnd + "</color>";
    }

    public void SetText( string word )
    {
        CharIndex = 0;
        Word = word;
        Txt.text = "<color=white>" + word + "</color>";
    }

    public void ResetWord( )
    {
        CharIndex = 0;
        Txt.text = "<color=white>" + Word + "</color>";
    }
}
