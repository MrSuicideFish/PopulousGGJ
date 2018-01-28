using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Infrastructure : MonoBehaviour
{
    public static int InfraResCount;
    private static Infrastructure_Data[ ] all_infrastructure;
    public static Infrastructure_Data[ ] ALL_INFRASTRUCTURE {
        get {
            if (all_infrastructure == null)
            {
                all_infrastructure = Resources.LoadAll<Infrastructure_Data>( "Structures/" );
                InfraResCount = all_infrastructure.Length - 1;
            }

            return all_infrastructure;
        }
    }

    private Infrastructure_Data Model;
    public Image StructureSprite { get; private set; }

    [HideInInspector]
    public string Name;
    [HideInInspector]
    public bool IsHacked;
    [HideInInspector]
    public int HackLevel;
    [HideInInspector]
    public E_INFRA_TYPE InfrastructureType;

    [HideInInspector]
    public float CureRateModifier;
    [HideInInspector]
    public float EscapeRateModifier;
    [HideInInspector]
    public float CommunicationsModifier;
    [HideInInspector]
    public float WasteManagementModifier;

    public District ParentDistrict;

    private void Awake()
    {
        StructureSprite = GetComponent<Image>();

        //Get random properties
        if (ParentDistrict != null
            && Infrastructure.ALL_INFRASTRUCTURE != null)
        {
            //Generate random structure
            Model = Infrastructure.ALL_INFRASTRUCTURE[
                Random.Range( 0, Infrastructure.InfraResCount )];

            Name                    = Model.Name;
            HackLevel               = Model.HackLevel;
            InfrastructureType      = Model.InfrastructureType;
            CureRateModifier        = Model.CureRateModifier;
            EscapeRateModifier      = Model.EscapeRateModifier;
            CommunicationsModifier  = Model.CommunicationsModifier;
            WasteManagementModifier = Model.WasteManagementModifier;
            StructureSprite.sprite  = Model.Sprite;

            ParentDistrict.Structures.Add( this );
        }
    }
}