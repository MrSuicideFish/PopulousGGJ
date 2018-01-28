using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameHUD : MonoBehaviour
{
    public Text CountdownText;
    public InputField MyText;
    public float HackTime = 10.0f;
    public Image TimerBG;
    public GameObject WordHolder;

    public List<string> WordPool { get; private set; }

    private Word[ ] Words;
    private float HackTimeRemaining;
    private Image TimerBar;
    private IEnumerator HackCoroutine;

    public static InGameHUD Instance;

    private void Awake( )
    {
        Instance = this;

        TimerBar = TimerBG.transform.GetChild( 0 ).GetComponent<Image>( );
        Words = GetComponentsInChildren<Word>( true );

        CountdownText.gameObject.SetActive( true );
        TimerBG.gameObject.SetActive( false );
        WordHolder.gameObject.SetActive( false );

        HackTimeRemaining = HackTime;
        MyText.text = "";
    }

    private void Start( )
    {
        LoadAllWords( );
        SetWords( );
        StartCoroutine( StartCountdown( ) );
    }

    private IEnumerator StartCountdown( )
    {
        string text = "Start hacking in\n";
        int countDown = 3;

        while( countDown >= 0 )
        {
            if( countDown == 0 )
                CountdownText.text = text + "Hack!";
            else
                CountdownText.text = text + countDown.ToString( ) + "...";

            yield return new WaitForSeconds( 1.0f );
            countDown -= 1;
        }

        CountdownText.gameObject.SetActive( false );
        WordHolder.gameObject.SetActive( true );
        TimerBG.gameObject.SetActive( true );

        HackCoroutine = StartHacking( );
        StartCoroutine( HackCoroutine );
    }

    /// <summary>
    /// Load all possible words from the text file located in path.
    /// </summary>
    private void LoadAllWords( )
    {
        WordPool = new List<string>( );

        string path = "Assets/Resources/WordList.txt";
        StreamReader reader = new StreamReader( path );

        while( !reader.EndOfStream )
            WordPool.Add( reader.ReadLine( ) );

        reader.Close( );
    }

    private void SetWords( )
    {
        for( int i = 0; i < Words.Length; i++ )
        {
            string rndWord = WordPool[ Random.Range( 0, WordPool.Count ) ];
            Words[ i ].SetWord( rndWord );
            WordPool.Remove( rndWord );
        }
    }

    private string GetNewWord( )
    {
        string rndWord = WordPool[ Random.Range( 0, WordPool.Count ) ];
        WordPool.Remove( rndWord );
        return rndWord;
    }

    private IEnumerator StartHacking( )
    {
        while( HackTimeRemaining >= 0.0f )
        {
            // If at any point the input is no longer focused, focus it to receive input!
            if( !MyText.isFocused )
                MyText.ActivateInputField( );

            // Decrease hack time.
            HackTimeRemaining -= Time.deltaTime;
            TimerBar.fillAmount = HackTimeRemaining / HackTime;

            if( Input.GetKeyDown( KeyCode.Return ) )
                SubmitWord( );

            yield return null;
        }
    }

    private void SubmitWord( )
    {
        bool foundMatch = false;

        foreach( var word in Words )
        {
            if( word.GetWord( ) == MyText.text )
            {
                word.SetWord( GetNewWord( ) );
                foundMatch = true;

                // TODO: Play word success SFX.
            }
            else
            {
                word.ResetWord( );
            }
        }

        MyText.text = "";

        if( !foundMatch )
            StopHacking( );
    }

    private void StopHacking()
    {
        StopCoroutine( HackCoroutine );
        MyText.enabled = false;
        MyText.DeactivateInputField( );

        // TODO: Play failure SFX.
    }

    public void TypeLetter( )
    {
        // Only proceed if there is text in the input field!
        if( string.IsNullOrEmpty( MyText.text ) || MyText.text.Length == 0 )
        {
            foreach( var word in Words )
                word.ResetWord( );

            return;
        }

        // Loop through all words to see if any match.
        for( int i = 0; i < Words.Length; i++ )
        {
            Word word = Words[ i ];
            if( word.DoLettersMatch( MyText.text ) )
            {
                word.TypeLetter( MyText.text.Length );
            }
            else
            {
                word.ResetWord( );
            }
        }
    }
}