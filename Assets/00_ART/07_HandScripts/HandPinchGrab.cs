using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit;

public class HandPinchGrab : MonoBehaviour
{
    public bool isLeftHand = true;
    public XRDirectInteractor directInteractor;
    public float pinchThreshold = 0.7f;

    [Header("Feedback Visual")]
    public Renderer targetSphereRenderer;

    private XRHandSubsystem handSubsystem;
    private bool wasPinching = false;
    private IXRSelectInteractable currentSelection; // Guarda o que estamos a segurar

    void Update()
    {
        if (handSubsystem == null || !handSubsystem.running)
        {
            var subsystems = new List<XRHandSubsystem>();
            SubsystemManager.GetSubsystems(subsystems);
            if (subsystems.Count > 0) handSubsystem = subsystems[0];
            return;
        }

        var hand = isLeftHand ? handSubsystem.leftHand : handSubsystem.rightHand;

        if (!hand.isTracked)
        {
            if (targetSphereRenderer != null) targetSphereRenderer.material.color = Color.red;
            return;
        }

        var indexJoint = hand.GetJoint(XRHandJointID.IndexTip);
        if (indexJoint.TryGetPose(out var indexPose) && targetSphereRenderer != null)
        {
            targetSphereRenderer.transform.position = indexPose.position;
        }

        float strength = GetPinchStrength(hand);
        bool isPinching = strength >= pinchThreshold;

        if (targetSphereRenderer != null)
            targetSphereRenderer.material.color = isPinching ? Color.green : Color.blue;

        // --- LÓGICA DE GRAB COMPATÍVEL COM 2.5.2 ---
        if (directInteractor != null)
        {
            if (isPinching && !wasPinching)
            {
                // Tenta encontrar objetos próximos
                List<IXRInteractable> targets = new List<IXRInteractable>();
                directInteractor.GetValidTargets(targets);

                if (targets.Count > 0)
                {
                    // Na v2.5.2 usamos StartManualInteraction através do InteractionManager ou do Interactor
                    currentSelection = targets[0] as IXRSelectInteractable;
                    if (currentSelection != null)
                    {
                        directInteractor.interactionManager.SelectEnter((IXRSelectInteractor)directInteractor, currentSelection);
                    }
                }
            }
            else if (!isPinching && wasPinching)
            {
                // Se estávamos a segurar algo, soltamos
                if (directInteractor.hasSelection)
                {
                    directInteractor.interactionManager.SelectExit((IXRSelectInteractor)directInteractor, directInteractor.interactablesSelected[0]);
                }
                currentSelection = null;
            }
        }

        wasPinching = isPinching;
    }

    float GetPinchStrength(XRHand hand)
    {
        var thumbJoint = hand.GetJoint(XRHandJointID.ThumbTip);
        var indexJoint = hand.GetJoint(XRHandJointID.IndexTip);
        if (!thumbJoint.TryGetPose(out var thumbPose) || !indexJoint.TryGetPose(out var indexPose))
            return 0f;

        float dist = Vector3.Distance(thumbPose.position, indexPose.position);
        return 1f - Mathf.Clamp01((dist - 0.02f) / 0.06f);
    }
}