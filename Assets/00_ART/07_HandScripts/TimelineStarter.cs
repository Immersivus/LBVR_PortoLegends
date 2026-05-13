using UnityEngine;
using UnityEngine.Playables;

public class TimelineStarter : MonoBehaviour
{
    public float delay = 1f;
    private PlayableDirector director;

    void Start()
    {
        director = GetComponent<PlayableDirector>();
        director.playOnAwake = false;
        director.Stop();
        Invoke("IniciarTimeline", delay);
    }

    void IniciarTimeline()
    {
        director.Play();
    }
}