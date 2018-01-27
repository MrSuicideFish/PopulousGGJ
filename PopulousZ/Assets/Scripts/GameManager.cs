using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static Virus PLAYER_VIRUS { get; private set; }
    public static District[] ALL_DISTRICTS { get; private set; }
    public static int GlobalDifficultyLevel = 1;

    private void ResetGame()
    {
        /// Reset virus
        PLAYER_VIRUS = new Virus();

        /// Reset Districts
        ALL_DISTRICTS = District.FindObjectsOfType<District>();
        for (int i = 0; i < ALL_DISTRICTS.Length; i++)
        {
            ALL_DISTRICTS[i].Population        = Random.Range( 500, 3000 );
            ALL_DISTRICTS[i].PopulationDensity = Random.Range( 0.3f, 0.8f );
            ALL_DISTRICTS[i].PanicLevel        = 0;
            ALL_DISTRICTS[i].SpawnRate         = 1.0f;
            ALL_DISTRICTS[i].SpreadRate        = 0.0f;
            ALL_DISTRICTS[i].CureRate          = 0.0f;
            ALL_DISTRICTS[i].Communications    = 1.0f;
            ALL_DISTRICTS[i].WasteManagement   = 1.0f;
            ALL_DISTRICTS[i].EscapeRate        = 1.0f;
            ALL_DISTRICTS[i].HasCure           = false;
            ALL_DISTRICTS[i].IsInfected        = false;

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
        
    }
}
