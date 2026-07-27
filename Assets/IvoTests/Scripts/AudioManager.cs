using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource track01, track02;

    public static AudioManager instance;

    bool isPlayingTrack1;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        StartCoroutine(FadeTrack(track02));
    }

    public void SwapTrack(AudioSource newSource)
    {
        StopAllCoroutines();
        StartCoroutine(FadeTrack(newSource));
    }

    private IEnumerator FadeTrack(AudioSource newSource)
    {
        float timeToFade = 4f;
        float timeElapsed = 0;

        track01 = track02;
        track02 = newSource;

        track02.Play();

        while (timeElapsed < timeToFade)
        {
            track02.volume = Mathf.Lerp(0, 0.25f, timeElapsed / timeToFade);
            track01.volume = Mathf.Lerp(0.25f, 0, timeElapsed / timeToFade);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        track01.volume = 0;
        track02.volume = 0.25f;
        track01.Stop();
    }
}
