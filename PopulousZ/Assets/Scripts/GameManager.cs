using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance {
        get {
            if (instance == null)
                instance = GameManager.FindObjectOfType<GameManager>();

            return instance;
        }
    }

    public static Virus PLAYER_VIRUS { get; private set; }
    public static District[ ] ALL_DISTRICTS { get; private set; }
    public static int GlobalDifficultyLevel = 1;

    public static int TurnCount { get; private set; }
    public static bool IsSimulating { get; private set; }
    public static bool HasVirusDeployed { get; private set; }

    public static District District_GroundZero;

    public static Infrastructure StructureToHack;

    public static int VirusPoints = 0;

    //UI Components
    public CommandPanelController CommandLinePanel;

    public InGameHUD HackHud;

    public BeautifyEffect.Beautify BeautifyComponent;
    public Color Color_WorldSafe;
    public Color Color_WorldInfected;

    public static float TimeForSimulation = 7.0f;
    public static float SimulationTimer { get; private set; }

    public static int TotalPopulation;
    public static int InfectedPopulation;
    public static int Panic;

    private float MedicalRatingInInfected;
    private float EscapeRatingInInfected;
    private float CommunicationRatingInInfected;
    private float WasteRatingInInfected;
    private float HealthInfectedRatioInInfected;

    private readonly float PopGrowthRate = 10.0f / 60;
    private float GhostTimer = 0.0f;

    private void ResetGame()
    {
        /// Reset variables
        TurnCount = -1;
        District_GroundZero = null;

        /// Reset virus
        PLAYER_VIRUS = new Virus();

        /// Reset Districts
        ALL_DISTRICTS = District.FindObjectsOfType<District>();
        for (int i = 0; i < ALL_DISTRICTS.Length; i++)
        {
            ALL_DISTRICTS[i].Population         = Random.Range( 500, 3000 );
            ALL_DISTRICTS[i].PopulationDensity  = Random.Range( 0.3f, 0.8f );
            ALL_DISTRICTS[i].InfectedPopulation = 0;
            ALL_DISTRICTS[i].PanicLevel         = 0;
            ALL_DISTRICTS[i].SpawnRate          = 1.0f;
            ALL_DISTRICTS[i].SpreadRate         = 0.0f;
            ALL_DISTRICTS[i].CureRate           = 0.0f;
            ALL_DISTRICTS[i].Communications     = 1.0f;
            ALL_DISTRICTS[i].WasteManagement    = 1.0f;
            ALL_DISTRICTS[i].EscapeRate         = 1.0f;
            ALL_DISTRICTS[i].HasCure            = false;
        }

        /// Set start population
        for(int i = 0; i < ALL_DISTRICTS.Length; i++)
            TotalPopulation += ALL_DISTRICTS[i].Population;
    }

    private void Awake()
    {
        ResetGame();

        /// Initialize event service
    }

    private void Update()
    {
        if (IsSimulating)
        {
            GhostTimer += Time.deltaTime;
            if(GhostTimer >= 0.2f)
            {
                UpdateInInfectedRatings();

                float growthThisFrame = 0.9f * (MedicalRatingInInfected / (TotalPopulation / 2));
                float deathThisFrame = 0.2f * HealthInfectedRatioInInfected;
                float escapeThisFrame = 0.3f * (EscapeRatingInInfected);
                float commsThisFrame = 0.6f * CommunicationRatingInInfected;
                float sickThisFrame = 2 + ((int)deathThisFrame + PLAYER_VIRUS.Lv_Airborne) - (int)escapeThisFrame - commsThisFrame;

                TotalPopulation += (int)(PopGrowthRate * growthThisFrame);
                for(int i = 0; i < ALL_DISTRICTS.Length; i++)
                {
                    if(ALL_DISTRICTS[i].IsInfected)
                    {
                        int _x = (int)(sickThisFrame * ALL_DISTRICTS[i].PopulationDensity);
                        Debug.Log( sickThisFrame + ", " + ALL_DISTRICTS[i].PopulationDensity );
                        InfectedPopulation += _x
                            + (PLAYER_VIRUS.Lv_Airborne + PLAYER_VIRUS.Lv_Foodborne + PLAYER_VIRUS.Lv_Waterborne);
                    }
                }
                Panic = (int)(HealthInfectedRatioInInfected / GetHealthyToInfectedRatio()) * 100 / 100;
                GhostTimer = 0.0f;
            }

            if(BeautifyComponent != null)
            {
                BeautifyComponent.nightVisionColor = Color.Lerp( Color_WorldSafe, Color_WorldInfected, GetHealthyToInfectedRatio() );
            }

            SimulationTimer += Time.deltaTime;
            if(SimulationTimer >= TimeForSimulation)
            {
                BeginPlayerTurn();
            }
        }
    }

    public void BeginPlayerTurn()
    {
        IsSimulating = false;
        SimulationTimer = 0.0f;
        GhostTimer = 0.0f;
        //CommandLinePanel.gameObject.SetActive( true );
        CommandLinePanel.GoToSelectMenu();
        TurnCount++;
    }

    public void EndPlayerTurn()
    {
        //CommandLinePanel.gameObject.SetActive( false );
        IsSimulating = true;
    }

    public void DeployVirusInDistrict(District _d)
    {
        /// Infect a population
        District_GroundZero = _d;
        District_GroundZero.InfectedPopulation = 50;

        HasVirusDeployed = true;
        EndPlayerTurn();
    }

    public float GetHealthyToInfectedRatio()
    {
        return InfectedPopulation / TotalPopulation;
    }

    public void BeginHackStructure(Infrastructure _s)
    {
        StructureToHack = _s;

        //Show hack screen
        HackHud.gameObject.SetActive( true );
        CommandLinePanel.gameObject.SetActive( false );
        HackHud.BeginHack();
    }

    public void EndHackStructure()
    {
        //Hide hack screen
        bool success = HackHud.WasHackSuccess;
        HackHud.gameObject.SetActive( true );
        CommandLinePanel.gameObject.SetActive( true);

        if (success)
        {
            StructureToHack.IsHacked = true;
            StructureToHack.StructureSprite.color = Color.white;
        }
        else
        {
            StructureToHack.IsHacked = false;
            StructureToHack.HackLevel = Mathf.Clamp( StructureToHack.HackLevel + 1,
                1, 3 );
            StructureToHack.StructureSprite.color = new Color( 1, 1, 1, 0.2f );
        }

        StructureToHack = null;
        EndPlayerTurn();
    }

    public Infrastructure[] GetAllInfrustructure()
    {
        List<Infrastructure> _structures = new List<Infrastructure>();
        for(int i = 0; i < ALL_DISTRICTS.Length; i++)
            for(int j = 0; j < ALL_DISTRICTS[i].Structures.Count; j++)
                _structures.Add( ALL_DISTRICTS[i].Structures[j] );
        return _structures.ToArray();
    }

    public void LevelUpWaterBorne()
    {

    }

    public void LevelUpAirBorne()
    {

    }

    public void LevelUpFoodBorne()
    {

    }

    private int totalInfra = -1;
    public int GetTotalInfrastructure()
    {
        if (totalInfra == -1)
            totalInfra = GameObject.FindObjectsOfType<Infrastructure>().Length;
        return totalInfra;
    }

    public int GetHackedInfrastructure()
    {
        int hackedCount = 0;
        for(int i = 0; i < ALL_DISTRICTS.Length; i++)
        {
            for(int j = 0; j < ALL_DISTRICTS[i].Structures.Count; j++)
            {
                if (ALL_DISTRICTS[i].Structures[j].IsHacked)
                    hackedCount++;
            }
        }
        return hackedCount;
    }

    private void UpdateInInfectedRatings()
    {
        float newMed = 0.0f;
        float newEscape = 0.0f;
        float newComms = 0.0f;
        float newWaste = 0.0f;

        float totalPop = 0;
        float infectedPop = 0;

        for (int i = 0; i < ALL_DISTRICTS.Length; i++)
        {
            if (!ALL_DISTRICTS[i].IsInfected)
                continue;

            totalPop += ALL_DISTRICTS[i].Population;
            infectedPop += ALL_DISTRICTS[i].InfectedPopulation;
            for (int j = 0; j < ALL_DISTRICTS[i].Structures.Count; j++)
            {
                if (!ALL_DISTRICTS[i].Structures[j].IsHacked)
                {
                    newMed += ALL_DISTRICTS[i].Structures[j].CureRateModifier;
                    newEscape += ALL_DISTRICTS[i].Structures[j].EscapeRateModifier;
                    newComms += ALL_DISTRICTS[i].Structures[j].CommunicationsModifier;
                    newWaste += ALL_DISTRICTS[i].Structures[j].WasteManagementModifier;
                }
            }
        }

        //TotalPopulation = (int)totalPop;
        //InfectedPopulation = (int)infectedPop;
        HealthInfectedRatioInInfected = infectedPop / totalPop;
    }
}