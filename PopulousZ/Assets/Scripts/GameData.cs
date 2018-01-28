using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public float GetSpreadRate()
    {
        return (Lv_Airborne + Lv_Foodborne + Lv_Waterborne) / 3;
    }
}

public enum E_INFRA_TYPE : int
{
    TRAVEL = 0,
    MEDICAL,
    COMMUNICATIONS,
    WASTE_MANAGEMENT
}


[CreateAssetMenu()]
public class Infrastructure_Data : ScriptableObject
{
    public int HackLevel;
    public E_INFRA_TYPE InfrastructureType;

    public float CureRateModifier;
    public float EscapeRateModifier;
    public float CommunicationsModifier;
    public float WasteManagementModifier;

    public Sprite Sprite;
}