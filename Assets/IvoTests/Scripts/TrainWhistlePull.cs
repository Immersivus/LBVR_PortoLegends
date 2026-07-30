using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class TrainWhistlePull : MonoBehaviour
{
    [Header("References")]
    public Transform restPosition;      // The handle's resting transform (usually parent/mount)
    public AudioSource whistleAudio;
    public ParticleSystem steam;

    [Header("Trigger Settings")]
    public float pullDistanceThreshold = 0.15f; // how far down before it toots
    public float retriggerCooldown = 0.5f;

    private bool isPlaying = false;
    private float cooldownTimer = 0f;

    private Rigidbody rb;

    void Awake()
    {
        if (restPosition == null) restPosition = transform.parent;

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float pulledDistance = restPosition.position.y - transform.position.y;

        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (pulledDistance >= pullDistanceThreshold)
        {
            if (!isPlaying && cooldownTimer <= 0f)
            {
                StartWhistle();
            }
        }
        else
        {
            if (isPlaying)
            {
                StopWhistle();
            }
        }
    }



    void StartWhistle()
    {
        isPlaying = true;
        whistleAudio.loop = true;   // whistle sounds while held down
        whistleAudio.Play();
        steam.Play();
    }

    void StopWhistle()
    {
        isPlaying = false;
        whistleAudio.Stop();
        cooldownTimer = retriggerCooldown;
        steam.Stop();
    }
}
