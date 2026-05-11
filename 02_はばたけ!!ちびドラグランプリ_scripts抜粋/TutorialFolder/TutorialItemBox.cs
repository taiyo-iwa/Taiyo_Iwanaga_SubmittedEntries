using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialItemBox : MonoBehaviour
{
    [SerializeField] private List<TutorialItemBase> itemPool;
    [SerializeField] private AudioSource _audioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out TutorialPlayerItem playerItem) && !playerItem.IsWaitingForItem)
        {
            playerItem.StartItemRoulette(itemPool);
            if (_audioSource != null)
            {
                _audioSource.Play();
            }
        }
    }
}
