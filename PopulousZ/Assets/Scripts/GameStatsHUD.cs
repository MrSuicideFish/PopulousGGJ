using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStatsHUD : MonoBehaviour
{
    public Text StatsText;

    private void Update()
    {
        StatsText.text = string.Format(
            ("----------\n" +
            "<color=#ff8c0cff>INFECTION</color>\n" +
            "----------\n" +
            "TOTAL POPULATION: {0}\n" +
            "TOTAL INFECTED: {1}\n" +
            "PANIC: {2} / 100\n\n" +
            "----------\n" +
            "<color=#ff0fffff>INFRASTRUCTURE</color>\n" +
            "----------\n" +
            "TOTAL INFRASTRUCTURE: {3}\n" +
            "HACKED INFRASTRUCTURE: {4}"),

            GameManager.TotalPopulation,
            GameManager.InfectedPopulation,
            GameManager.Panic,
            GameManager.Instance.GetTotalInfrastructure(),
            GameManager.Instance.GetHackedInfrastructure() );
    }
}
