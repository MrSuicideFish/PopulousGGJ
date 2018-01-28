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

    private int CharIndex = 0;
    private string Word;
    private Text Txt;

    private void Awake( )
    {
        Txt = GetComponent<Text>( );
        RectTrans = GetComponent<RectTransform>( );
    }

    private void Update( )
    {
        transform.position += Vector3.down * Time.deltaTime * FallSpeed;
    }

    public string GetWord( )
    {
        return Txt.text;
    }

    public char GetNextLetter( )
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
        Txt.text = Word = word;
    }

    public void ResetWord()
    {
        Txt.color = Color.white;
        Txt.text = Word;
    }
}
