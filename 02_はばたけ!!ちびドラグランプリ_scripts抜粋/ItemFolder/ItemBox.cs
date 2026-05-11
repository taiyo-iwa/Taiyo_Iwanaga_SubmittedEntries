using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager = default;
    [SerializeField] private List<ItemBase> itemPool;
    [SerializeField] private AudioSource _audioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerItem playerItem))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (!playerController._isFinish && 
                (_gameManager._twoPlayerSelectDragon < 0 || playerController.IsPlayer))
            {
                playerItem.StartItemRoulette(itemPool);
                _audioSource.Play();
            }
        }
    }
}
