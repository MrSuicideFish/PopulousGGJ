using UnityEngine;
using System.Collections;

public class District : MonoBehaviour
{
    /// <summary>
    /// The name of the disctrict.
    /// </summary>
    public string Name;

    /// <summary>
    /// The number of people remaining in district
    /// </summary>
    public int Population;

    /// <summary>
    /// The defcon level
    /// </summary>
    public int PanicLevel;

    /// <summary>
    /// How crowded the district is
    /// </summary>
    public float PopulationDensity;

    /// <summary>
    /// The rate at which the district spawns new populous
    /// </summary>
    public float SpawnRate;

    /// <summary>
    /// How well the virus spreads to other districts
    /// </summary>
    public float SpreadRate;

    /// <summary>
    /// The rate at which the district cures it's population
    /// </summary>
    public float CureRate;

    /// <summary>
    /// The integrity of the district's communication
    /// </summary>
    public float Communications;

    /// <summary>
    /// The integrity of the district's waste management
    /// </summary>
    public float WasteManagement;

    /// <summary>
    /// The rate at which the district's populous evacuates
    /// </summary>
    public float EscapeRate;

    /// <summary>
    /// The rate at which the district cures it's population
    /// </summary>
    public bool HasCure;

    /// <summary>
    /// The infrastructure this district contains
    /// </summary>
    public Infrastructure[ ] Structures;

    /// <summary>
    /// The neighboring districts
    /// </summary>
    public District[ ] Neighbors;
}