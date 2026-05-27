using UnityEngine;

public class ClockBacwards : MonoBehaviour
{
    public Transform HourHand;
    public Transform MinHand;

    public float speed = 6000f;
    private float totalSec = 0f;
    
    void Update()
    {
        totalSec -= Time.deltaTime * speed;

        float minutes = (totalSec / 60f) % 60f;
        float hours = (totalSec / 3600f) % 12f;

        HourHand.localRotation = Quaternion.Euler(hours * 30f, -90, 90);
        MinHand.localRotation = Quaternion.Euler(minutes * 6f, -90, 90);
    }
}
