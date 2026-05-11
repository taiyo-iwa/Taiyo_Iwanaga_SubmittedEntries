using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventProgressObject : MonoBehaviour
{
    public enum ProgressType
    {
        StartProgress,
        DriftProgress,
        DashProgress,
        ItemBoxProgress,
        ItemCrashProgress,
        FrameProgress,
        ShieldProgress,
        FretherProgress,
        SoarProgress,
    }

    [SerializeField] private ProgressType _type;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventProgressManager.Instance.ActivateProgress((EventProgressManager.ProgressType)_type);
        }
    }
}
