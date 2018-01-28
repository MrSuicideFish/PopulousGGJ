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

    private List<FallingWord> CurrentWords;
    private List<FallingWord> MatchingWords;
    private string CurrentText;
    private bool HasFoundWord = false;

    public static InGameHUD Instance;

    private void Awake( )
    {
        Instance = this;
    }

    private void Start( )
    {
        MyText.text = "";
        LoadWords( );
        CurrentWords = new List<FallingWord>( );
        MatchingWords = new List<FallingWord>( );
        StartCoroutine( StartFallingWords( ) );
    }

    private void LoadWords( )
    {
        WordPool = new List<string>( );

        string path = "Assets/Resources/WordList.txt";
        StreamReader reader = new StreamReader( path );

        while( !reader.EndOfStream )
            WordPool.Add( reader.ReadLine( ) );

        reader.Close( );
    }

    private IEnumerator StartFallingWords( )
    {
        while( WordPool.Count > 0 )
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
        // Append what has been typed so far.
        CurrentText += letter.ToString( );

        // Find all matching words.
        foreach( var word in CurrentWords )
        {
            // Does current index of word match how many letters we've typed
            // AND next letter match current typed letter?
            if( word.CharIndex == CurrentText.Length - 1
                && word.GetCurrentLetter( ) == letter )
            {
                if( MatchingWords != null && !MatchingWords.Contains( word ) )
                    MatchingWords.Add( word );
            }
            else
            {
                if( MatchingWords != null && MatchingWords.Contains( word ) )
                    MatchingWords.Remove( word );

                word.ResetWord( );
            }
        }

        // If we have found some words.
        if( MatchingWords != null && MatchingWords.Count > 0 )
        {
            // Show what we've typed so far.
            MyText.text = CurrentText;

            // Change the color of the currently typed letter in each word.
            for( int i = 0; i < MatchingWords.Count; i++ )
                MatchingWords[ i ].TypeLetter( );

            // Check to see if all words have been completed.
            bool allWordsTyped = MatchingWords.All( x => x.WordFinished( ) );
            if( allWordsTyped )
            {
                // Remove finished words from the CurrentWords list.
                // Destroy the finished word.
                for( int i = 0; i < MatchingWords.Count; i++ )
                {
                    CurrentWords.Remove( MatchingWords[ i ] );
                    Destroy( MatchingWords[ i ].gameObject );
                }

                // Reset stuff
                CurrentText = null;
                MatchingWords = new List<FallingWord>( );
            }
        }
        else // Didn't find matching or mistyped character.
        {
            // Reset all words.
            foreach( var word in CurrentWords )
                word.ResetWord( );

            // Reset everything.
            MyText.text = CurrentText = null;
            MatchingWords = new List<FallingWord>( );
        }
    }
}