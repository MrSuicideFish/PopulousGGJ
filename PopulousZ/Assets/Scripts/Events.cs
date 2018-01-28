using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventService
{
    private static List<Event> randomEvents;
    private static List<Event> panicEvents;

    public EventService()
    {
        Event[ ] events = Resources.LoadAll<Event>( "" );
        for (int i = 0; i < events.Length; i++)
        {
            if (events[i].PanicEvent)
            {
                panicEvents.Add( events[i] );
            }
            else
            {
                randomEvents.Add( events[i] );
            }
        }
    }

    public List<Event> getRandomEvents()
    {
        return randomEvents;
    }

    public List<Event> getPanicEvents()
    {
        return panicEvents;
    }
}

public class Event : ScriptableObject
{
    public string Name;
    public string Description;
    public bool PanicEvent;
    public float CureRateModifier;
    public float EscapeRateModifier;
    public float CommunicationsModifier;
    public float WasteManagementModifier;
    public float AdjacentDistrictSpreadRateModifier;
    public float SpreadRateModifier;
}