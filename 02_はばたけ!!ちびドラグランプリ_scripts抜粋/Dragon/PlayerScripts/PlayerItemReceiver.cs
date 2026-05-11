using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemReceiver : MonoBehaviour
{
    [SerializeField] private PlayerFlightGauge _gauge;

    public void RecoverGauge(float amount)
    {
        _gauge.Recover(amount);
    }
}
