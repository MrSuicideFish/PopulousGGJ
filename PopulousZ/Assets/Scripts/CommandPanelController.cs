using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum E_COMMAND_STATE
{
    NONE,
    SELECT_MENU,
    SELECT_DISTRICT,
    DISTRICT_OPTIONS,
    DEPLOY,
    HACK,
    HACK_STATS,
    VIRUS_STATS,
    LV_AIRBORNE,
    LV_FOODBORNE,
    LV_WATERBORNE,
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
            PanelAudioSourceComponent = GetComponent<AudioSource>();

        CommandLineText           = "";
        CommandLineCharacterIndex = 0;
        SelectedDistrict          = null;
        SelectedStructure         = null;
        GoToSelectMenu();

        //Start routine
        StartCoroutine( Routine_UpdateText() );
    }

    private void OnDisable()
    {
        CommandState = E_COMMAND_STATE.NONE;
    }

    private void Awake()
    {
        CommandState = E_COMMAND_STATE.NONE;
    }

    private void Start()
    {
        GoToSelectMenu();
    }

    public void Update()
    {
        int selection = GetInputSelection();

        if (selection <= 0 || GameManager.ALL_DISTRICTS == null
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

            case E_COMMAND_STATE.SELECT_MENU:
                switch (selection)
                {
                    case 1:
                        GoToSelectDistrict();
                        break;
                    case 2:
                        GoToVirusStats();
                        break;
                    case 3:
                        GameManager.Instance.EndPlayerTurn();
                        break;
                    case 4:
                        GoToQuit();
                        break;
                }
                break;

            case E_COMMAND_STATE.SELECT_DISTRICT:
                selection -= 1;
                if (selection == GameManager.ALL_DISTRICTS.Length)
                {
                    GoToSelectMenu();
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
                        District _d = SelectedDistrict;
                        SelectedDistrict.Deselect();
                        SelectedDistrict = null;
                        SelectedStructure = null;

                        GameManager.Instance.DeployVirusInDistrict( _d );
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
                selection -= 1;
                SelectedStructure = SelectedDistrict.Structures[selection];
                GoToStructureStats();
                break;

            case E_COMMAND_STATE.HACK_STATS:
                if(selection == 1)
                {
                    GameManager.Instance.BeginHackStructure( SelectedStructure );
                }
                else if (selection == 2)/// Back
                {
                    GoToHackInfrastructure();
                }
                break;

            case E_COMMAND_STATE.VIRUS_STATS:
                switch (selection)
                {
                    case 1:
                        GoToLevelUpAirborne();
                        break;
                    case 2:
                        GoToLevelUpWaterborne();
                        break;
                    case 3:
                        GoToLevelUpFoodborne();
                        break;
                    case 4:
                        GoToSelectMenu();
                        break;
                }
                break;

            case E_COMMAND_STATE.LV_AIRBORNE:
                if(selection == 1 && GameManager.VirusPoints >= 27 * GameManager.PLAYER_VIRUS.Lv_Airborne)
                {
                    GameManager.Instance.LevelUpAirBorne();
                    GoToVirusStats();
                }
                else if(selection == 2)
                {
                    GoToVirusStats();
                }
                break;

            case E_COMMAND_STATE.LV_FOODBORNE:
                if (selection == 1 && GameManager.VirusPoints >= 22 * GameManager.PLAYER_VIRUS.Lv_Foodborne)
                {
                    GameManager.Instance.LevelUpFoodBorne();
                    GoToVirusStats();
                }
                else if (selection == 2)
                {
                    GoToVirusStats();
                }
                break;
            case E_COMMAND_STATE.LV_WATERBORNE:
                if (selection == 1 && GameManager.VirusPoints >= 25 * GameManager.PLAYER_VIRUS.Lv_Waterborne)
                {
                    GameManager.Instance.LevelUpWaterBorne();
                    GoToVirusStats();
                }
                else if (selection == 2)
                {
                    GoToVirusStats();
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

    public void GoToSelectMenu()
    {
        if (GameManager.ALL_DISTRICTS == null || CommandState == E_COMMAND_STATE.SELECT_MENU)
            return;

        CommandState = E_COMMAND_STATE.SELECT_MENU;
        CommandLineText = "YOUR TURN: \n\n";
        CommandLineText += "1. DEPLOY VIRUS/HACK\n";
        CommandLineText += "2. UPGRADE VIRUS\n";
        CommandLineText += "3. END TURN\n";
        CommandLineText += "4. QUIT GAME\n";
    }

    private void GoToSelectDistrict()
    {
        if (GameManager.ALL_DISTRICTS == null || CommandState == E_COMMAND_STATE.SELECT_DISTRICT)
            return;

        if(GameManager.HasVirusDeployed)
        {
            SelectedDistrict = GameManager.District_GroundZero;
            SelectedStructure = null;
            GoToHackInfrastructure();
            return;
        }

        CommandState = E_COMMAND_STATE.SELECT_DISTRICT;

        int i;
        if (SelectedDistrict != null)
            SelectedDistrict.Deselect();

        SelectedDistrict = null;

        if(!GameManager.HasVirusDeployed)
            CommandLineText  = "SELECT A DISTRICT TO BEGIN INFECTION: \n\n";
        else
            CommandLineText = "SELECT A DISTRICT: \n\n";
        for (i = 0; i < GameManager.ALL_DISTRICTS.Length; i++)
        {
            //if(GameManager.HasVirusDeployed
            //    && (GameManager.ALL_DISTRICTS[i].InfectedPopulation <= 0
            //        && GameManager.ALL_DISTRICTS[i] != GameManager.District_GroundZero))
            //{
            //    CommandLineText += CommandLineText += "<color=#5d5d5dff>";
            //    CommandLineText += (i + 1).ToString() + ". " + GameManager.ALL_DISTRICTS[i].Name.ToUpper();
            //    CommandLineText += CommandLineText += "</color>";
            //}
            //else
                CommandLineText += (i + 1).ToString() + ". " + GameManager.ALL_DISTRICTS[i].Name.ToUpper();

            CommandLineText += '\n';
        }
            
        i++;
        CommandLineText += (i).ToString() + ". " + "BACK";
    }

    private void GoToDistrictOptions()
    {
        if (CommandState == E_COMMAND_STATE.DISTRICT_OPTIONS)
            return;
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
        if (CommandState == E_COMMAND_STATE.HACK)
            return;
        CommandState = E_COMMAND_STATE.HACK;
        CommandLineText = "SELECT A STRUCTURE TO HACK: \n\n";
        int i;
        for (i = 0; i < SelectedDistrict.Structures.Count; i++)
        {
            if (SelectedDistrict.Structures[i].IsHacked)
                CommandLineText += "<color=#35353562>";

            CommandLineText += (i + 1).ToString() + ". " + SelectedDistrict.Structures[i].Name.ToUpper();
            if (SelectedDistrict.Structures[i].IsHacked)
                CommandLineText += "</color>";

            CommandLineText += '\n';
        }
        i++;
        CommandLineText += (i).ToString() + ". " + "BACK";
    }

    private void GoToStructureStats()
    {
        if (CommandState == E_COMMAND_STATE.HACK_STATS)
            return;
        CommandState = E_COMMAND_STATE.HACK_STATS;
        CommandLineText = "STRUCTURE STATS: \n";
        CommandLineText += "------------\n\n";
        CommandLineText += "NAME: " + SelectedStructure.Name.ToUpper() + "\n";
        CommandLineText += "(DIFFICULTY: " + SelectedStructure.HackLevel + ")\n";
        CommandLineText += "CURE RATING: " + SelectedStructure.CureRateModifier + '\n';
        CommandLineText += "TRAVEL RATING: " + SelectedStructure.EscapeRateModifier + '\n';
    }

    private void GoToVirusStats()
    {
        if (CommandState == E_COMMAND_STATE.VIRUS_STATS)
            return;

        CommandState = E_COMMAND_STATE.VIRUS_STATS;
        CommandLineText = "VIRUS STATS: \n";
        CommandLineText += "------------\n\n";
        CommandLineText += "SPREAD RATING(" + GameManager.PLAYER_VIRUS.GetSpreadRate() + ")\n";
        CommandLineText += "1. AIRBORNE LEVEL: " + GameManager.PLAYER_VIRUS.Lv_Airborne + '\n';
        CommandLineText += "2. WATERBORNE LEVEL: " + GameManager.PLAYER_VIRUS.Lv_Waterborne + '\n';
        CommandLineText += "3. FOODBORNE LEVEL: " + GameManager.PLAYER_VIRUS.Lv_Foodborne + '\n';
        CommandLineText += "4. BACK\n\n";

        CommandLineText += "ENTER A NUMBER TO UPGRADE...";
    }

    private void GoToLevelUpAirborne()
    {
        if (CommandState == E_COMMAND_STATE.LV_AIRBORNE)
            return;

        int p = (27 * GameManager.PLAYER_VIRUS.Lv_Airborne);
        CommandState = E_COMMAND_STATE.LV_AIRBORNE;
        CommandLineText = "INCREASING THE AIRBORNE LEVEL WILL INCREASE THE VIRUS' ABILITY TO SPREAD THROUGH AIR VIA MOUTH-TO-MOUTH CONTACT AND TRASH. ARE YOU SURE YOU WANT TO LEVEL THIS UP?\n\n";
        CommandLineText += "COST: " + p + "\n";

        if (p >= GameManager.VirusPoints)
            CommandLineText += "1. YES\n";
        else
            CommandLineText += "<color=#3h3h3hff>1. YES (INSUFFICIENT VIRUS POINTS)</color>";

        CommandLineText += "2. NO\n";
    }

    private void GoToLevelUpWaterborne()
    {
        if (CommandState == E_COMMAND_STATE.LV_WATERBORNE)
            return;

        int p = (25 * GameManager.PLAYER_VIRUS.Lv_Waterborne);
        CommandState = E_COMMAND_STATE.LV_WATERBORNE;
        CommandLineText = "INCREASING THE WATERBORNE LEVEL WILL INCREASE THE VIRUS' ABILITY TO SPREAD THROUGH WATER VIA RESIVOURS, SEWERS, AND WATER TREATMENT FACILITIES. ARE YOU SURE YOU WANT TO LEVEL THIS UP?\n\n";
        CommandLineText += "COST: " + p + "\n";

        if (p >= GameManager.VirusPoints)
            CommandLineText += "1. YES\n";
        else
            CommandLineText += "<color=#3h3h3hff>1. YES (INSUFFICIENT VIRUS POINTS)</color>";

        CommandLineText += "2. NO\n";
    }

    private void GoToLevelUpFoodborne()
    {
        if (CommandState == E_COMMAND_STATE.LV_FOODBORNE)
            return;
        int p = (22 * GameManager.PLAYER_VIRUS.Lv_Foodborne);
        CommandState = E_COMMAND_STATE.LV_FOODBORNE;
        CommandLineText = "INCREASING THE FOODBORNE LEVEL WILL INCREASE THE VIRUS' ABILITY TO SPREAD THROUGH FOOD VIA UNDERCOOKED MEAT, TRASH, ETC. ARE YOU SURE YOU WANT TO LEVEL THIS UP?\n\n";
        CommandLineText += "COST: " + p + "\n";
        if (p >= GameManager.VirusPoints)
            CommandLineText += "1. YES\n";
        else
            CommandLineText += "<color=#3h3h3hff>1. YES (INSUFFICIENT VIRUS POINTS)</color>";

        CommandLineText += "2. NO\n";
    }

    private void GoToQuit()
    {
        if (CommandState == E_COMMAND_STATE.QUIT)
            return;
        CommandState = E_COMMAND_STATE.QUIT;
        CommandLineText = "ARE YOU SURE YOU WANT TO QUIT?: \n\n";
        CommandLineText += "1. Continue wreaking havok\n";
        CommandLineText += "2. Quit\n";
    }

    private void SkipSelectDistrict()
    {
        SelectedDistrict = GameManager.District_GroundZero;
        GoToSelectDistrict();
    }

    private IEnumerator Routine_UpdateText()
    {
        float t = 0.0f;
        while(true)
        {
            if (!GameManager.IsSimulating)
            {
                /// Update text
                if (CommandText.text != CommandLineText)
                {
                    if (CommandLineCharacterIndex > CommandLineText.Length)
                        CommandLineCharacterIndex = CommandLineText.Length - 1;

                    CommandText.text = CommandLineText.Substring( 0, CommandLineCharacterIndex );
                    CommandLineCharacterIndex++;

                    //Play click sound
                    if (PanelAudioSourceComponent != null)
                        PanelAudioSourceComponent.PlayOneShot( ClickAudio );

                    yield return new WaitForSeconds( 0.003f );
                }
                else
                    CommandLineCharacterIndex = 0;
            }
            else
            {
                t += Time.deltaTime;

                int simTime = Mathf.FloorToInt( GameManager.TimeForSimulation - GameManager.SimulationTimer );
                CommandText.text = "TURN ENDED.\n";
                if (t < 0.5f)
                {
                    CommandText.text += "SIMULATING..." + simTime.ToString() + "|";
                }
                else if (t <= 1.0f)
                {
                    CommandText.text += "SIMULATING..." + simTime.ToString();
                }
                else
                    t = 0.0f;
            }
            yield return null;
        }
        yield return null;
    }

    public RectTransform CommandLineRect;
    public IEnumerator Routine_HideCommandLine()
    {
        float t = 0;


        yield return null;
    }

    public IEnumerator Routine_ShowCommandLine()
    {
        float t = 0;

        yield return null;
    }
}
