using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ResetTimelines : MonoBehaviour
{

    [SerializeField] List<PlayableDirector> timelines;
    [SerializeField] GameObject model;

    public void ResetAllTimelines()
    {
        foreach (PlayableDirector director in timelines)
        {
            director.Stop();
        }

        model.SetActive(false);
    }

}
