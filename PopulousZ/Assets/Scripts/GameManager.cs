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

    //UI Components
    public CommandPanelController CommandLinePanel;

    public BeautifyEffect.Beautify BeautifyComponent;
    public Color Color_WorldSafe;
    public Color Color_WorldInfected;

    public static float TimeForSimulation = 7.0f;
    private float SimulationTimer = 0.0f;

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
            /// tick districts
            for (int i = 0; i < ALL_DISTRICTS.Length; i++)
            {
                //Increase the infected population by factors
                float proximity = ALL_DISTRICTS[i].InfectedPopulation / ALL_DISTRICTS[i].Population;
                proximity *= ALL_DISTRICTS[i].PopulationDensity;

                float waterInfectRate = ((1.1f - ALL_DISTRICTS[i].WasteManagement) * PLAYER_VIRUS.Lv_Waterborne) * ALL_DISTRICTS[i].PopulationDensity;
                float airInfectRate = ((1.1f - ALL_DISTRICTS[i].WasteManagement) * PLAYER_VIRUS.Lv_Airborne) * ALL_DISTRICTS[i].PopulationDensity;

                Debug.LogFormat( "Water infect ({0}): {1}", ALL_DISTRICTS[i].name, waterInfectRate );
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
        CommandLinePanel.gameObject.SetActive( true );
        TurnCount++;
    }

    private void EndPlayerTurn()
    {
        CommandLinePanel.gameObject.SetActive( false );
        IsSimulating = true;
    }

    public void DeployVirusInDistrict(District _d)
    {
        /// Infect a population
        District_GroundZero = _d;
        _d.InfectedPopulation += Random.Range( 1, 10 );

        HasVirusDeployed = true;
        EndPlayerTurn();
    }

    public float GetHealthyToInfectedRatio()
    {
        int totalPop = 0;
        int infectedPop = 0;
        for(int i = 0; i < ALL_DISTRICTS.Length; i++)
        {
            totalPop += ALL_DISTRICTS[i].Population;
            infectedPop += ALL_DISTRICTS[i].InfectedPopulation;
        }

        return infectedPop / totalPop;
    }

    public Infrastructure[] GetAllInfrustructure()
    {
        List<Infrastructure> _structures = new List<Infrastructure>();
        for(int i = 0; i < ALL_DISTRICTS.Length; i++)
            for(int j = 0; j < ALL_DISTRICTS[i].Structures.Count; j++)
                _structures.Add( ALL_DISTRICTS[i].Structures[j] );
        return _structures.ToArray();
    }
}