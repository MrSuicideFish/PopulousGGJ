using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Virus
{
    /// <summary>
    /// How fast the virus kills
    /// </summary>
    public float Speed;

    /// <summary>
    /// When host dies, how much does it spread?
    /// </summary>
    public float Spore;

    /// <summary>
    /// How well the virus spreads through air
    /// </summary>
    public int Lv_Airborne;

    /// <summary>
    /// How well the virus spreads through water
    /// </summary>
    public int Lv_Waterborne;

    /// <summary>
    /// How well the virus spreads via food
    /// </summary>
    public int Lv_Foodborne;

    /// <summary>
    /// How well the virus spreads via insect
    /// </summary>
    public int Lv_Insect;

    /// <summary>
    /// How well the virus spreads via animal
    /// </summary>
    public int Lv_Animal;

    public float GetSpreadRate()
    {
        return (Lv_Airborne + Lv_Animal + Lv_Foodborne + Lv_Insect + Lv_Waterborne) / 5;
    }
}

public enum E_INFRA_TYPE : int
{
    ESCAPE_ROUTE = 0,
    MEDICAL,
    COMMUNICATIONS,
    WASTE_MANAGEMENT
}

[CreateAssetMenu()]
public class Infrastructure : ScriptableObject
{
    public static int InfraResCount;
    private static Infrastructure[ ] all_infrastructure;
    public static Infrastructure[ ] ALL_INFRASTRUCTURE
    {
        get
        {
            if (all_infrastructure == null)
            {
                all_infrastructure = Resources.LoadAll<Infrastructure>( "Infrastructure/" );
                InfraResCount = all_infrastructure.Length - 1;
            }

            return all_infrastructure;
        }
    }

    public bool IsHacked;
    public int HackLevel;
    public E_INFRA_TYPE InfrastructureType;

    public float CureRateModifier;
    public float EscapeRateModifier;
    public float CommunicationsModifier;
    public float WasteManagementModifier;
}