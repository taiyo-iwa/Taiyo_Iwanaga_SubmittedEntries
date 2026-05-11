using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEventSound : MonoBehaviour
{
    [SerializeField] AudioSource _tutorialSE;
    [SerializeField] AudioClip _panelSound;
    [SerializeField] AudioClip _correctAnswer;

    public void PanelSound()
    {
        _tutorialSE.PlayOneShot(_panelSound);
    }

    public void CorrectAnswer()
    {
        _tutorialSE.PlayOneShot(_correctAnswer);
    }
}
