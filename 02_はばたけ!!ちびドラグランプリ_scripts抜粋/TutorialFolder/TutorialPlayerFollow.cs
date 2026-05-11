using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayerFollow : MonoBehaviour
{
    [SerializeField] private TutorialFireballManager _fireballManager = default;

    public void FireballShot()
    {
        _fireballManager.MakeFireball(this.transform);
    }
}
