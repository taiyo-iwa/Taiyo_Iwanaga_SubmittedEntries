using UnityEngine;
using System.Collections;

public class ResultAudioController : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _resultClip;

    private void Start()
    {
        _audioSource.Play();

        StartCoroutine(CheckSoundEnd());
    }

    IEnumerator CheckSoundEnd()
    {
        yield return new WaitWhile(() => _audioSource.isPlaying);

        // --- Ä¶‚ªI—¹‚µ‚½Œã‚Ìˆ— ---
        _audioSource.Stop();
        _audioSource.clip = _resultClip;
        _audioSource.Play();
        _audioSource.loop = true;
    }
}
