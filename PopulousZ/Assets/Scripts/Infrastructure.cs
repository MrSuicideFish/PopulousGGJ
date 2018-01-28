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
                all_infrastructure = Resources.LoadAll<Infrastructure_Data>( "Infrastructure/" );
                InfraResCount = all_infrastructure.Length - 1;
            }

            return all_infrastructure;
        }
    }

    public string Name;
    public bool IsHacked;
    public int HackLevel;
    public E_INFRA_TYPE InfrastructureType;

    public float CureRateModifier;
    public float EscapeRateModifier;
    public float CommunicationsModifier;
    public float WasteManagementModifier;

    public District ParentDistrict;
    private Infrastructure_Data Model;
    private Image StructureSprite;

    private void Awake()
    {
        StructureSprite = GetComponent<Image>();

        //Get random properties
        if (Infrastructure.InfraResCount > 0 && ParentDistrict != null)
        {
            //Generate random structure
            Model = Infrastructure.ALL_INFRASTRUCTURE[
                Random.Range( 0, Infrastructure.InfraResCount )];

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