using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameHUD : MonoBehaviour
{
    /// <summary>
    /// Any information that needs to be displayed to the user such as countdown, success, failure, etc...
    /// </summary>
    public Text InformationText;

    /// <summary>
    /// The parent object that holds all of the words to be hacked.
    /// </summary>
    public GameObject WordHolder;

    /// <summary>
    /// The parent object that holds the timer.
    /// </summary>
    public Image TimerBG;

    // TODO: Have this value set somewhere else!
    /// <summary>
    /// Total hacking time player has to hack words.
    /// </summary>
    public float HackTime = 10.0f;

    /// <summary>
    /// Used to display the number of correct hacks.
    /// EX: 1 / 10
    /// </summary>
    public Text NumCorrectText;

    /// <summary>
    /// What the player types.
    /// </summary>
    public InputField MyInput;

    public int TotalNumHacks { get; set; }

    /// <summary>
    /// List of all possible words to be hacked.  Loaded from .txt file in Resources folder.
    /// </summary>
    public List<string> AllWords { get; private set; }

    /// <summary>
    /// Boolean used to determine if the user has succeeded or failed in hacking.
    /// </summary>
    public bool WasHackSuccess { get; private set; }

    /// <summary>
    /// List of words to be hacked.  Children of WordHolder.
    /// </summary>
    private Word[ ] Words;

    /// <summary>
    /// How long the player has left to hack the words.
    /// </summary>
    private float HackTimeRemaining;

    /// <summary>
    /// The image that represents the HackTimeRemaining.
    /// </summary>
    private Image TimerBar;

    /// <summary>
    /// Used to determine if the countdown has finished.  If finished, start hacking!
    /// </summary>
    private bool HasCountdownFinished = false;

    /// <summary>
    /// Used to determine if the hacking has started.
    /// If true, the MyInput InputField should accent input.
    /// </summary>
    private bool IsHackingInProgress = false;

    /// <summary>
    /// How many words were submitted successfully.
    /// </summary>
    private int CurrentNumCorrect = 0;

    /// <summary>
    /// The Instance that can used in other classes.
    /// </summary>
    public static InGameHUD Instance;

    private void Awake( )
    {
        Instance = this;

        TimerBar = TimerBG.transform.GetChild( 0 ).GetComponent<Image>( );
        Words = GetComponentsInChildren<Word>( true );

        ToggleInformation( true );

        HackTimeRemaining = HackTime;

        MyInput.DeactivateInputField( );
        MyInput.text = "";

        WasHackSuccess = IsHackingInProgress = HasCountdownFinished = false;
    }

    private void OnEnable( )
    {
        TotalNumHacks = 10;

        NumCorrectText.text = "Num Correct: " + CurrentNumCorrect + " / " + TotalNumHacks;

        LoadAllWords( );
        SetWords( );
        StartCoroutine( StartCountdown( ) );
    }

    private void Update( )
    {
        if( !HasCountdownFinished )
            return;

        // If hacking in progress, upate timer and allow for input.
        if( IsHackingInProgress )
        {
            if( HackTimeRemaining >= 0.0f )
            {
                if( !MyInput.isFocused )
                    MyInput.ActivateInputField( );

                HackTimeRemaining -= Time.deltaTime;
                TimerBar.fillAmount = HackTimeRemaining / HackTime;

                if( Input.GetKeyDown( KeyCode.Return ) && !string.IsNullOrEmpty( MyInput.text ) )
                    SubmitWord( );
            }
            else
            {
                WasHackSuccess = false;
            }
        }
        else
        {
            if( Input.GetKeyDown( KeyCode.Q ) )
            {
                // TODO: Return to main menu.

                DeactivateHUD( );
            }
        }
    }

    /// <summary>
    /// Load all possible words from the text file located in path.
    /// </summary>
    private void LoadAllWords( )
    {
        AllWords = new List<string>( );

        string path = "Assets/Resources/WordList.txt";
        StreamReader reader = new StreamReader( path );

        while( !reader.EndOfStream )
            AllWords.Add( reader.ReadLine( ) );

        reader.Close( );
    }

    private void SetWords( )
    {
        for( int i = 0; i < Words.Length; i++ )
        {
            string rndWord = AllWords[ Random.Range( 0, AllWords.Count ) ];
            Words[ i ].SetWord( rndWord );
            AllWords.Remove( rndWord );
        }
    }

    private string GetNewWord( )
    {
        string rndWord = AllWords[ Random.Range( 0, AllWords.Count ) ];
        AllWords.Remove( rndWord );
        return rndWord;
    }

    private void ToggleInformation( bool toggle )
    {
        if( toggle )
        {
            InformationText.gameObject.SetActive( true );
            TimerBG.gameObject.SetActive( false );
            WordHolder.gameObject.SetActive( false );
            NumCorrectText.gameObject.SetActive( false );
        }
        else
        {
            InformationText.gameObject.SetActive( false );
            TimerBG.gameObject.SetActive( true );
            WordHolder.gameObject.SetActive( true );
            NumCorrectText.gameObject.SetActive( true );
        }
    }

    private IEnumerator StartCountdown( )
    {
        IsHackingInProgress = HasCountdownFinished = false;

        string text = "Start hacking in\n";
        int countDown = 3;

        while( countDown >= 0 )
        {
            if( countDown == 0 )
                InformationText.text = "Hack!";
            else
                InformationText.text = text + countDown.ToString( ) + "...";

            yield return new WaitForSeconds( 1.0f );
            countDown -= 1;
        }

        ToggleInformation( false );

        IsHackingInProgress = HasCountdownFinished = true;
    }

    private void SubmitWord( )
    {
        bool foundMatch = false;

        foreach( var word in Words )
        {
            if( word.GetWord( ) == MyInput.text )
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

        MyInput.text = "";

        WasHackSuccess = IsHackingInProgress = foundMatch;

        if( foundMatch )
        {
            CurrentNumCorrect++;
            NumCorrectText.text = "Num Correct: " + CurrentNumCorrect + " / " + TotalNumHacks;

            if( CurrentNumCorrect == TotalNumHacks )
            {
                IsHackingInProgress = false;
                WasHackSuccess = true;

                StopHacking( );
            }
        }
        else
        {
            StopHacking( );
        }
    }

    private void StopHacking( )
    {
        MyInput.enabled = false;
        MyInput.DeactivateInputField( );

        ToggleInformation( true );

        string message = "";
        if( WasHackSuccess )
            message = "<color=#00AC33FF>You have succeeded!\nPress Q to return to hacking menu.</color>";
        else
            message = "<color=red>You have failed!\nPress Q to return to hacking menu.</color>";

        InformationText.text = message;

        // TODO: Play failure SFX.
    }

    private void DeactivateHUD( )
    {
        gameObject.SetActive( false );
    }

    public void TypeLetter( )
    {
        // Only proceed if there is text in the input field!
        if( string.IsNullOrEmpty( MyInput.text ) || MyInput.text.Length == 0 )
        {
            foreach( var word in Words )
                if( word != null )
                    word.ResetWord( );

            return;
        }

        // Loop through all words to see if any match.
        for( int i = 0; i < Words.Length; i++ )
        {
            Word word = Words[ i ];

            if( word.DoLettersMatch( MyInput.text ) )
                word.TypeLetter( MyInput.text.Length );
            else
                word.ResetWord( );
        }
    }
}