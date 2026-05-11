using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ƒhƒ‰ƒSƒ“‚Ìî•ñ‚ğ“ü‚ê‚é
/// </summary>
public class DragonManager : MonoBehaviour
{
    [SerializeField] private GameObject[] dragons = default;
    //î•ñ‚ğæ“¾‚·‚é
    public GameObject[] GetDragons()
    {
        return dragons;
    }
}
