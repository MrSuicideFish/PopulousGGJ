﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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
    /// The number of people infected
    /// </summary>
    public int InfectedPopulation;

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
    /// Does this district have any infected population
    /// </summary>
    public bool IsInfected { get { return InfectedPopulation > 0; } }

    private Sprite OldSprite;
    public Sprite SelectedSprite;

    /// <summary>
    /// The infrastructure this district contains
    /// </summary>
    [HideInInspector]
    public List<Infrastructure> Structures = new List<Infrastructure>();

    /// <summary>
    /// The neighboring districts
    /// </summary>
    public District[ ] Neighbors;

    #region Components
    public Image DistrictImage { get; private set; }
    #endregion

    private Color InfectionColor = Color.blue;

    private void Awake()
    {
        DistrictImage = GetComponent<Image>();

        if(DistrictImage != null)
        {
            OldSprite = DistrictImage.sprite;
            DistrictImage.color = InfectionColor;
        }
    }

    private void Start()
    {
        /// Fade out infra
        for(int i = 0; i < Structures.Count; i++)
        {
            Structures[i].StructureSprite.color = new Color( 1, 1, 1, 0.2f );
        }
    }

    public void Update()
    {
        if (DistrictImage != null)
        {
            if (IsInfected)
            {
                DistrictImage.color = Color.white;

                if (GameManager.IsSimulating)
                {
                    for (int i = 0; i < Structures.Count; i++)
                    {
                        Structures[i].StructureSprite.color = Color.white;
                    }

                    if (InfectedPopulation / Population > 0.5f)
                        for (int i = 0; i < Neighbors.Length; i++)
                            Neighbors[i].InfectedPopulation += 5;
                }
            }
        }
    }

    public void Infect()
    {
        if(DistrictImage != null)
        {
            DistrictImage.color = Color.white;
        }
    }

    public void Select()
    {
        DistrictImage.sprite = SelectedSprite;
    }

    public void Deselect()
    {
        DistrictImage.sprite = OldSprite;
    }
}