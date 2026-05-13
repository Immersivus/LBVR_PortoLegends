using UnityEngine;
using UnityEngine.XR.Hands;
using System.Collections.Generic;

public class HandTrackingManager : MonoBehaviour
{
    public static HandTrackingManager Instance;
    public XRHandSubsystem HandSubsystem { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (HandSubsystem == null || !HandSubsystem.running)
        {
            var subsystems = new List<XRHandSubsystem>();
            SubsystemManager.GetSubsystems(subsystems);
            foreach (var s in subsystems)
            {
                if (s.running)
                {
                    HandSubsystem = s;
                    break;
                }
            }
        }
    }
}