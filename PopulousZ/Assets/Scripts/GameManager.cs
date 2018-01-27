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

    private void ResetGame()
    {
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

            /// Setup random district structures
            ALL_DISTRICTS[i].SpawnStructures();
        }
    }

    private void Awake()
    {
        ResetGame();
    }

    private void Update()
    {
        /// Debug Info
        //for (int i = 0; i < ALL_DISTRICTS.Length; i++)
        //{
        //    Debug.Log( "District: " + ALL_DISTRICTS[i].name );
        //    Debug.Log( "    --Is Infected: " + ALL_DISTRICTS[i].IsInfected );
        //    Debug.Log( "    --Has Cure: " + ALL_DISTRICTS[i].HasCure );
        //    Debug.Log( "    --Population: " + ALL_DISTRICTS[i].Population );
        //    Debug.Log( "    --Density: " + ALL_DISTRICTS[i].PopulationDensity );
        //    Debug.Log( "    --Panic Level: " + ALL_DISTRICTS[i].PanicLevel );
        //    Debug.Log( "    --Spawn Rate: " + ALL_DISTRICTS[i].SpawnRate );
        //    Debug.Log( "    --Spread Rate: " + ALL_DISTRICTS[i].SpreadRate );
        //    Debug.Log( "    --Cure Rate: " + ALL_DISTRICTS[i].CureRate );
        //    Debug.Log( "    --Communications: " + ALL_DISTRICTS[i].Communications );
        //    Debug.Log( "    --Waste Management: " + ALL_DISTRICTS[i].WasteManagement );
        //    Debug.Log( "    --Escape Rate: " + ALL_DISTRICTS[i].EscapeRate );
        //}

        /// tick districts
        for (int i = 0; i < ALL_DISTRICTS.Length; i++)
        {
            //Increase the infected population by factors
            float waterInfectRate = ((1.1f - ALL_DISTRICTS[i].WasteManagement) * PLAYER_VIRUS.Lv_Waterborne) * ALL_DISTRICTS[i].PopulationDensity;
            Debug.LogFormat( "Water infect ({0}): {1}", ALL_DISTRICTS[i].name, waterInfectRate );
        }
    }
}
