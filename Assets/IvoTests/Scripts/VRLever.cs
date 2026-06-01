using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRLever : MonoBehaviour
{
    [Header("References")]
    public XRSimpleInteractable interactable;
    public Transform pivot;

    [Header("Lever Settings")]
    public float minAngle = -45f;
    public float maxAngle = 45f;

    private IXRSelectInteractor currentInteractor;

    private void Awake()
    {
        interactable.selectEntered.AddListener(OnSelectEntered);
        interactable.selectExited.AddListener(OnSelectExited);
    }

    private void OnDestroy()
    {
        interactable.selectEntered.RemoveListener(OnSelectEntered);
        interactable.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        currentInteractor = args.interactorObject;
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        currentInteractor = null;
    }

    private void Update()
    {
        if (currentInteractor == null)
            return;

        Transform handTransform = currentInteractor.transform;

        // Convert hand position into pivot-local coordinates
        Vector3 localHandPos =
            pivot.parent.InverseTransformPoint(handTransform.position);

        // Example: lever rotates around local X axis
        float angle =
            Mathf.Atan2(localHandPos.y, localHandPos.z) * Mathf.Rad2Deg;

        angle = Mathf.Clamp(angle, minAngle, maxAngle);

        pivot.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }
}
