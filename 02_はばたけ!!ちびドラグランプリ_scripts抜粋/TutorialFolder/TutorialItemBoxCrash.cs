using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialItemBoxCrash : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _IdleBox;
    [SerializeField] private GameObject _IdleParticle;
    [SerializeField] private GameObject _BrokenBox;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _animator.Play("Crash");
        }
    }

    public void OnBoxCrash()
    {
        _IdleBox.SetActive(false);
        _IdleParticle.SetActive(false);
        _BrokenBox.SetActive(true);
    }

    public void SetItemBox()
    {
        _IdleBox.SetActive(true);
        _IdleParticle.SetActive(true);
        _BrokenBox.SetActive(false);
    }
}
