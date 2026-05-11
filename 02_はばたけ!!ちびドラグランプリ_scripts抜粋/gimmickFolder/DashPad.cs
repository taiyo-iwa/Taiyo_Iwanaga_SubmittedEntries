using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashPad : MonoBehaviour
{
    [SerializeField] private float _boostForce = 15f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(transform.forward * _boostForce, ForceMode.Impulse);
            }
        }
    }
}
