using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum E_COMMAND_STATE
{
    SELECT_DISTRICT,
    DISTRICT_OPTIONS,
    DEPLOY,
    HACK,
    HACK_STATS,
    QUIT
}

public class CommandPanelController : MonoBehaviour
{
    private AudioSource PanelAudioSourceComponent;
    private E_COMMAND_STATE CommandState;
    private bool IsKeyingSelection;
    private int CommandLineCharacterIndex;
    private string CommandLineText;
    private Coroutine KeyCommandRoutine;

    //Selections
    private District SelectedDistrict;
    private Infrastructure SelectedStructure;

    public Text CommandText;
    public AudioClip ClickAudio;
    public AudioClip InputErrorAudio;

    private void OnEnable()
    {
        if(PanelAudioSourceComponent == null)
        {
            PanelAudioSourceComponent = GetComponent<AudioSource>();
        }

        CommandLineText = "";
        CommandLineCharacterIndex = 0;
        CommandState = E_COMMAND_STATE.SELECT_DISTRICT;
        SelectedDistrict = null;
        GoToSelectDistrict();

        //Start routine
        StartCoroutine( Routine_UpdateText() );
    }

    private void Start()
    {
        GoToSelectDistrict();
    }

    public void Update()
    {
        int selection = GetInputSelection();

        if (selection == -1 || GameManager.ALL_DISTRICTS == null
            || GameManager.Instance.GetAllInfrustructure() == null)
            return;

        switch (CommandState)
        {
            case E_COMMAND_STATE.QUIT:
                if (selection == 1)
                    GoToSelectDistrict();
                else if(selection == 2)
                    Application.Quit();
                break;

            case E_COMMAND_STATE.SELECT_DISTRICT:
                selection -= 1;
                Debug.Log( selection );
                if (selection == GameManager.ALL_DISTRICTS.Length)
                {
                    GoToQuit();
                    return;
                }
                else if (selection > GameManager.ALL_DISTRICTS.Length)
                {
                    PanelAudioSourceComponent.PlayOneShot( InputErrorAudio );
                    return;
                }
                else
                {
                    District _d = GameManager.ALL_DISTRICTS[selection];
                    if (_d != null)
                    {
                        SelectedDistrict = _d;
                        SelectedDistrict.Select();
                    }

                    /// Begin district select fade

                    GoToDistrictOptions();
                    return;
                }
                break;

            case E_COMMAND_STATE.DISTRICT_OPTIONS:
                /// Go back
                if (selection == 2)
                    GoToSelectDistrict();
                else if(selection == 1)
                {
                    /// Deploy
                    if(!GameManager.HasVirusDeployed)
                    {
                        SelectedDistrict.Deselect();
                        SelectedDistrict = null;
                        SelectedStructure = null;
                        GameManager.Instance.DeployVirusInDistrict(SelectedDistrict);
                    }
                    else //Go to hack
                    {
                        GoToHackInfrastructure();
                        return;
                    }
                }
                break;

            case E_COMMAND_STATE.HACK:
                //Back
                if (selection == SelectedDistrict.Structures.Count)
                {
                    GoToDistrictOptions();
                    return;
                }
                else if (selection > SelectedDistrict.Structures.Count)
                {
                    PanelAudioSourceComponent.PlayOneShot( InputErrorAudio );
                    return;
                }
                else
                {
                    SelectedStructure = SelectedDistrict.Structures[selection];
                    GoToStructureStats();
                }
                break;

            case E_COMMAND_STATE.HACK_STATS:


                if(selection == 1)
                {

                }
                else if (selection == 2)/// Back
                {
                    GoToHackInfrastructure();
                }
                break;
        }
    }

    private int GetInputSelection()
    {
        /// Select 0
        if (Input.GetKeyDown( KeyCode.Keypad0 )
            || Input.GetKeyDown(KeyCode.Alpha0))
            return 0;

        /// Select 1
        if (Input.GetKeyDown( KeyCode.Keypad1 )
            || Input.GetKeyDown( KeyCode.Alpha1 ))
            return 1;

        /// Select 2
        if (Input.GetKeyDown( KeyCode.Keypad2 )
            || Input.GetKeyDown( KeyCode.Alpha2 ))
            return 2;

        /// Select 3
        if (Input.GetKeyDown( KeyCode.Keypad3 )
            || Input.GetKeyDown( KeyCode.Alpha3 ))
            return 3;

        /// Select 4
        if (Input.GetKeyDown( KeyCode.Keypad4 )
            || Input.GetKeyDown( KeyCode.Alpha4 ))
            return 4;

        /// Select 5
        if (Input.GetKeyDown( KeyCode.Keypad5 )
            || Input.GetKeyDown( KeyCode.Alpha5 ))
            return 5;

        /// Select 6
        if (Input.GetKeyDown( KeyCode.Keypad6 )
            || Input.GetKeyDown( KeyCode.Alpha6 ))
            return 6;

        /// Select 7
        if (Input.GetKeyDown( KeyCode.Keypad7 )
            || Input.GetKeyDown( KeyCode.Alpha7 ))
            return 7;

        /// Select 8
        if (Input.GetKeyDown( KeyCode.Keypad8 )
            || Input.GetKeyDown( KeyCode.Alpha8 ))
            return 8;

        /// Select 9
        if (Input.GetKeyDown( KeyCode.Keypad9 )
            || Input.GetKeyDown( KeyCode.Alpha9 ))
            return 9;

        return -1;
    }

    private void GoToSelectDistrict()
    {
        if (GameManager.ALL_DISTRICTS == null)
            return;

        CommandState = E_COMMAND_STATE.SELECT_DISTRICT;

        int i;
        if (SelectedDistrict != null)
            SelectedDistrict.Deselect();

        SelectedDistrict = null;
        CommandLineText  = "SELECT DISTRICT: \n\n";
        for (i = 0; i < GameManager.ALL_DISTRICTS.Length; i++)
        {
            if(GameManager.HasVirusDeployed
                && (GameManager.ALL_DISTRICTS[i].InfectedPopulation <= 0
                    && GameManager.ALL_DISTRICTS[i] != GameManager.District_GroundZero))
            {
                CommandLineText += CommandLineText += "<color=#5d5d5dff>";
                CommandLineText += (i + 1).ToString() + ". " + GameManager.ALL_DISTRICTS[i].Name.ToUpper();
                CommandLineText += CommandLineText += "</color>";
            }
            else
                CommandLineText += (i + 1).ToString() + ". " + GameManager.ALL_DISTRICTS[i].Name.ToUpper();

            CommandLineText += '\n';
        }
            
        i++;
        CommandLineText += (i).ToString() + ". " + "QUIT";
    }

    private void GoToDistrictOptions()
    {
        CommandState = E_COMMAND_STATE.DISTRICT_OPTIONS;
        CommandLineText = "SELECT AN OPTION: \n\n";

        if(!GameManager.HasVirusDeployed)
        {
            CommandLineText += "1. DEPLOY VIRUS\n";
            CommandLineText += "2. BACK\n";
        }
        else
        {
            CommandLineText += "1. HACK INFRASTRUCTURE\n";
            CommandLineText += "2. BACK\n";
        }
    }

    private void GoToHackInfrastructure()
    {
        CommandState = E_COMMAND_STATE.HACK;
        CommandLineText = "SELECT A STRUCTURE TO HACK: \n\n";
        int i;
        for (i = 0; i < SelectedDistrict.Structures.Count; i++)
        {
            if (SelectedDistrict.Structures[i].IsHacked)
                CommandLineText += "<color=#5d5d5dff>";

            CommandLineText += (i + 1).ToString() + ". " + SelectedDistrict.Structures[i].Name.ToUpper();
            if (SelectedDistrict.Structures[i].IsHacked)
                CommandLineText += "</color>";

            CommandLineText += '\n';
        }
        i++;
        CommandLineText += (i).ToString() + ". " + "QUIT";
    }

    private void GoToStructureStats()
    {
        CommandState = E_COMMAND_STATE.HACK_STATS;
        CommandLineText = "STRUCTURE STATS: \n";
        CommandLineText += "-------------------------------\n\n";
        CommandLineText += "Name: " + SelectedStructure.Name.ToUpper() + " ";
        CommandLineText += "(Difficulty: " + SelectedStructure.Name.ToUpper() + ")\n";
        CommandLineText += "Cure Rating: " + SelectedStructure.CureRateModifier + '\n';
        CommandLineText += "Travel Rating: " + SelectedStructure.EscapeRateModifier + '\n';
    }

    private void GoToQuit()
    {
        CommandState = E_COMMAND_STATE.QUIT;
        CommandLineText = "ARE YOU SURE YOU WANT TO QUIT?: \n\n";
        CommandLineText += "1. Continue wreaking havok\n";
        CommandLineText += "2. Quit\n";
    }

    private IEnumerator Routine_UpdateText()
    {
        while(true)
        {
            /// Update text
            if (CommandText.text != CommandLineText)
            {
                if (CommandLineCharacterIndex > CommandLineText.Length)
                    CommandLineCharacterIndex = CommandLineText.Length - 1;

                CommandText.text = CommandLineText.Substring( 0, CommandLineCharacterIndex );
                CommandLineCharacterIndex++;

                //Play click sound
                if(PanelAudioSourceComponent != null)
                    PanelAudioSourceComponent.PlayOneShot( ClickAudio );

                yield return new WaitForSeconds( 0.003f );
            }
            else
                CommandLineCharacterIndex = 0;

            yield return null;
        }
        yield return null;
    }
}
