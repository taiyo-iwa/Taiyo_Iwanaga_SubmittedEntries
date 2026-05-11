using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickObject : MonoBehaviour
{
    public enum GimmickType
    {
        DashPad,
        JumpPad,
        DamageWall
    }

    [SerializeField] private GimmickType _type;
    [SerializeField] private float _power = 10f;
    [SerializeField] private float _maxPower = 20f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GimmickManager.Instance.ActivateGimmick((GimmickManager.GimmickType)_type, other.gameObject, _power, _maxPower, transform);
        }
    }
}