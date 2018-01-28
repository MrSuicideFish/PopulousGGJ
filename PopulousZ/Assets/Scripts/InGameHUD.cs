using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameHUD : MonoBehaviour
{
    public Text MyText;
    public FallingWord FallingTextPrefab;
    public float TimeBetweenWords = 1.0f;

    public List<string> WordPool { get; private set; }

    private string CurrentText;
    private List<FallingWord> CurrentWords;
    private string MatchingWord;
    private FallingWord[ ] MatchingWords;
    private bool HasFoundWord = false;

    public static InGameHUD Instance;

    private void Awake( )
    {
        Instance = this;
    }

    private void Start( )
    {
        LoadWords( );
        CurrentWords = new List<FallingWord>( );
        StartCoroutine( StartFallingWords( ) );
    }

    private void LoadWords( )
    {
        WordPool = new List<string>( );

        string path = "Assets/Resources/WordList.txt";
        StreamReader reader = new StreamReader( path );

        while( !reader.EndOfStream )
            WordPool.Add( reader.ReadLine( ) );

        Debug.Log( WordPool.Count );
        reader.Close( );
    }

    private IEnumerator StartFallingWords( )
    {
        while( true )
        {
            SpawnWordRandomLocation( );
            yield return new WaitForSeconds( TimeBetweenWords );
        }
    }

    private void SpawnWordRandomLocation( )
    {
        FallingWord obj = Instantiate( FallingTextPrefab, transform ) as FallingWord;
        float minPosX = obj.RectTrans.position.x - ( Screen.width / 2f ) + ( obj.RectTrans.sizeDelta.x / 2f );
        float maxPosX = obj.RectTrans.position.x + ( Screen.width / 2f ) - ( obj.RectTrans.sizeDelta.x / 2f );
        float posY = transform.position.y + ( Screen.height / 2f ) + ( obj.RectTrans.sizeDelta.y / 2f );
        obj.RectTrans.position = new Vector3( Random.Range( minPosX, maxPosX ), posY, 0f );

        AddWordToList( obj );
    }

    private void AddWordToList( FallingWord textObj )
    {
        string randWord = WordPool[ Random.Range( 0, WordPool.Count ) ];
        textObj.SetText( randWord );
        CurrentWords.Add( textObj );
        WordPool.Remove( randWord );
    }

    private void Update( )
    {
        foreach( char letter in Input.inputString )
            TypeLetter( letter );
    }

    public void TypeLetter( char letter )
    {
        if( HasFoundWord )
        {
            if( MatchingWords[ 0 ].GetNextLetter( ) == letter )
            {
                MyText.text += letter.ToString( );

                foreach( var word in MatchingWords )
                    if( word.GetNextLetter( ) == letter )
                        word.TypeLetter( );
            }

            if( MatchingWords[ 0 ].WordFinished( ) )
            {
                for( int i = 0; i < MatchingWords.Length; i++ )
                {
                    CurrentWords.Remove( MatchingWords[ i ] );
                    Destroy( MatchingWords[ i ].gameObject );
                }

                HasFoundWord = false;
                MatchingWords = null;
                MyText.text = "";
            }
        }
        else
        {
            MatchingWords = CurrentWords.FindAll( x => x.GetNextLetter( ) == letter ).ToArray( );
            if( MatchingWords != null && MatchingWords.Length > 0 )
            {
                HasFoundWord = true;
                MyText.text = letter.ToString( );
            }

            foreach( var word in MatchingWords )
                word.TypeLetter( );
        }
    }
}