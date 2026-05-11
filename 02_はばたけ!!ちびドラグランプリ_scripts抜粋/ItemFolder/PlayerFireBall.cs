using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFireBall : MonoBehaviour
{
    [SerializeField] FireballManager _fireballManager;
    public void ActivateFireBall(Transform playerTransForm)
    {
        _fireballManager.MakeFireball(playerTransForm);
    }
}
