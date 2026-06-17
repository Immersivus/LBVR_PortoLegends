using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class HandPinchGrab : MonoBehaviour
{
    public bool isLeftHand = true;
    public XRDirectInteractor directInteractor;
    public XRRayInteractor rayInteractor;
    public XRPokeInteractor pokeInteractor;
    public float pinchThreshold = 0.7f;

    [Header("Feedback Visual")]
    public Renderer targetSphereRenderer;

    private XRHandSubsystem handSubsystem;
    private bool wasPinching = false;

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
            // Tracking lost mid-grab — force exit all interactors cleanly
            if (wasPinching)
            {
                ForceExitAll();
                wasPinching = false;
            }

            if (targetSphereRenderer != null)
                targetSphereRenderer.material.color = Color.red;
            return;
        }

        // Move feedback sphere to index tip
        var indexJoint = hand.GetJoint(XRHandJointID.IndexTip);
        if (indexJoint.TryGetPose(out var indexPose) && targetSphereRenderer != null)
            targetSphereRenderer.transform.position = indexPose.position;

        float strength = GetPinchStrength(hand);
        bool isPinching = strength >= pinchThreshold;

        if (targetSphereRenderer != null)
            targetSphereRenderer.material.color = isPinching ? Color.green : Color.blue;

        if (isPinching && !wasPinching)
        {
            TryPressRayUI(pressed: true);
            TrySelectAll();
        }
            
        else if (!isPinching && wasPinching)
        {
            TryPressRayUI(pressed: false);
            ForceExitAll();
        }
           

        wasPinching = isPinching;
    }
    // --- UI (Ray over Canvas) ---

    private void TryPressRayUI(bool pressed)
    {
        if (rayInteractor == null) return;

        // XRRayInteractor exposes the UI press state through its line visual
        // but the actual UI click goes through the XRUIInputModule via simulateTouch
        // We drive it by toggling the ray interactor's select action simulation
        if (rayInteractor.TryGetCurrentUIRaycastResult(out var raycastResult))
        {
            // There's a UI element under the ray — send the UI press
            var uiInputModule = FindAnyObjectByType<XRUIInputModule>();
            if (uiInputModule != null)
                SimulateUIPress(uiInputModule, raycastResult, pressed);
        }
    }

    private void SimulateUIPress(XRUIInputModule uiInputModule,
                                  UnityEngine.EventSystems.RaycastResult raycastResult,
                                  bool pressed)
    {
        // XRUIInputModule routes UI events through the interactor's select state.
        // The cleanest way to trigger it without a physical button is to
        // temporarily enable selectActionTrigger via the interactor's UI press path.
        // Since XRRayInteractor.uiPressInput isn't directly settable, we use
        // SendUIPress which XRUIInputModule exposes internally via the interactor.

        // Force the ray interactor into select state to trigger UI press
        if (pressed)
            rayInteractor.interactionManager.SelectEnter(
                (IXRSelectInteractor)rayInteractor,
                GetRayUIInteractable());
        else
            ForceExitInteractor(rayInteractor);
    }

    // The XRRayInteractor itself acts as the UI interactable target when over UI
    private IXRSelectInteractable GetRayUIInteractable()
    {
        var targets = new List<IXRInteractable>();
        rayInteractor.GetValidTargets(targets);
        if (targets.Count > 0)
            return targets[0] as IXRSelectInteractable;
        return null;
    }

    private void TrySelectAll()
    {
        TrySelectInteractor(directInteractor);
        TrySelectInteractor(rayInteractor);
        TrySelectInteractor(pokeInteractor);
    }

    private void TrySelectInteractor(XRBaseInteractor interactor)
    {
        if (interactor == null) return;

        var targets = new List<IXRInteractable>();
        interactor.GetValidTargets(targets);

        if (targets.Count == 0) return;

        var selectable = targets[0] as IXRSelectInteractable;
        if (selectable != null)
            interactor.interactionManager.SelectEnter(
                (IXRSelectInteractor)interactor, selectable);
    }

    private void ForceExitAll()
    {
        ForceExitInteractor(directInteractor);
        ForceExitInteractor(rayInteractor);
        ForceExitInteractor(pokeInteractor);
    }

    private void ForceExitInteractor(XRBaseInteractor interactor)
    {
        if (interactor == null || !interactor.hasSelection) return;

        // Copy list — it mutates during iteration
        var selecting = new List<IXRSelectInteractable>(interactor.interactablesSelected);
        foreach (var interactable in selecting)
            interactor.interactionManager.SelectExit(
                (IXRSelectInteractor)interactor, interactable);
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