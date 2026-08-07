using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class LanguageSystemPlayerFollow : MonoBehaviour
{
    bool tracking;
    Transform playerTransform;

    [SerializeField] List<PlayableDirector> timelines;
    public static LanguageSystemPlayerFollow Instance { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }
        else
        {
            if (!tracking)
            {
                StartCoroutine(MoveToPlayer());
                tracking = true;
            }

            Vector3 newPosition = new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z);
            transform.position = newPosition;
        }
    }

    IEnumerator MoveToPlayer()
    {
        while (true)
        {
            Quaternion stabilizedRotation = new Quaternion(transform.rotation.x, playerTransform.rotation.y, transform.rotation.z, playerTransform.rotation.w);
            transform.rotation = Quaternion.Lerp(transform.rotation, stabilizedRotation, Time.deltaTime);
            yield return null;
        }
    }

    public void MuteWithLanguageChange(int index)
    {
        foreach(PlayableDirector director in timelines)
        {
            TimelineAsset timeline = director.playableAsset as TimelineAsset;
            if (timeline == null) return;

            var track = timeline.GetOutputTracks()
                                 .OfType<AudioTrack>().ToList();                                

            if(track != null)
            {
                for (int i = 0; i < track.Count; i++)
                {
                    if(i != index)
                    {
                        track[i].muted = true;
                    }
                    else
                    {
                        track[i].muted = false;
                    }
                }
            }
        }
    }

}
