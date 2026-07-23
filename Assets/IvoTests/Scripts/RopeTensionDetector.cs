using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

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

    [SerializeField] private float returnSpeed = 3f;
    [SerializeField] private float returnRotSpeed = 360f; // degrees/sec
    [SerializeField] private float snapDistance = 0.01f; // when to consider "arrived"

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private bool isReturning = false;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private bool isTaut = false;

    private void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnDisable()
    {
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        isReturning = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        if (!isReturning) return;

        Vector3 newPos = Vector3.MoveTowards(transform.position, startPosition, returnSpeed * Time.fixedDeltaTime);
        Quaternion newRot = Quaternion.RotateTowards(transform.rotation, startRotation, returnRotSpeed * Time.fixedDeltaTime);

        rb.MovePosition(newPos);
        rb.MoveRotation(newRot);

        if (Vector3.Distance(transform.position, startPosition) <= snapDistance)
        {
            transform.position = startPosition;
            transform.rotation = startRotation;
            isReturning = false;
        }
    }


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
