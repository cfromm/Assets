using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class allows methos to be called through events.
/// </summary>
public class EventManager : MonoBehaviour {

    private Dictionary<string, UnityEvent> eventDictionary;

    private static EventManager eventManager;

    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }

            return eventManager;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityEvent>();
        }
    }

    /// <summary>
    /// Sets up the events for other class to call.
    /// </summary>
    /// <param name="eventName">The string used to call the inluded methods.</param>
    /// <param name="listener">Methods to be called.</param>
    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    /// <summary>
    /// Stops the events to be invoked.
    /// </summary>
    /// <param name="eventName">The string used to call the inluded methods.</param>
    /// <param name="listener">Methods to be called.</param>
    public static void StopListening(string eventName, UnityAction listener)
        {
            if (eventManager == null) return;
            UnityEvent thisEvent = null;
            if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.RemoveListener(listener);
            }
        }

    /// <summary>
    /// Invoke every methods in <paramref name="eventName"/>.
    /// </summary>
    /// <param name="eventName">The string used to call the inluded methods.</param>
    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }
}

