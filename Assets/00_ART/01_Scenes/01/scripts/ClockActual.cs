using System;
using UnityEngine;

public class ClockActual : MonoBehaviour
{
    public Transform HourHand;
    public Transform MinHand;

    void Update()
    {
        DateTime now = DateTime.Now;

        float hours = now.Hour % 12 + now.Minute / 60f;
        float minutes = now.Minute + now.Second / 60f;

        HourHand.localRotation = Quaternion.Euler(hours * 30f -90, 90, -90);
        MinHand.localRotation = Quaternion.Euler(minutes * 6f -90, 90, -90);
    }
}
