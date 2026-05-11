using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEvent : MonoBehaviour
{

    public enum EventType
    {
        DriftEvent,
        DashEvent,
        ItemBoxEvent,
        ItemCrashEvent,
        FrameEvent,
        ShieldEvent,
        FretherEvent,
        SoarEvent,
    }

    [SerializeField] private EventType _type;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TutorialEventManager.Instance.ActivateEvent((TutorialEventManager.EventType)_type);
        }
    }
}
