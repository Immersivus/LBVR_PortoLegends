using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpringJoint))]
public class RopeTensionDetector : MonoBehaviour
{
    [SerializeField] private Transform ropeAnchor;
    [SerializeField] private float maxRopeLength = 1.5f;
    [SerializeField] private float tensionThreshold = 0.95f; // % of max length to count as "taut"
    [SerializeField] private float releaseThreshold = 0.65f; // must fall back below this to re-arm

    [SerializeField] private AudioSource tensionAudio;
    [SerializeField] private ParticleSystem tensionParticles;

    public UnityEvent onRopeReachedTension;
    public UnityEvent onRopeReleaseTension;

    private bool isTaut = false;

    void Update()
    {
        float distance = Vector3.Distance(transform.position, ropeAnchor.position);
        float ratio = distance / maxRopeLength;

        if (!isTaut && ratio >= tensionThreshold)
        {
            isTaut = true;
            TriggerTensionEffects();
            onRopeReachedTension.Invoke();
        }
        else if (isTaut && ratio <= releaseThreshold)
        {
            isTaut = false;
            onRopeReleaseTension.Invoke();
        }
    }

    void TriggerTensionEffects()
    {
        if (tensionAudio != null)
            tensionAudio.Play();

        if (tensionParticles != null)
            tensionParticles.Play();
    }
}
