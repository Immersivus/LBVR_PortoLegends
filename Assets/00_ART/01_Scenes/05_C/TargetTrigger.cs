using UnityEngine;

public class TargetTrigger : MonoBehaviour
{
    [SerializeField] private ParticleSystem targetParticles;
    [SerializeField] private AudioSource targetAudio;

    private void OnTriggerEnter(Collider other)
    {
        if (targetParticles != null)
        {
            targetParticles.Play();
        }

        if (targetAudio != null)
        {
            targetAudio.Play();
        }
    }
}