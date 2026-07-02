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
                RayUIPointerUp(); // release any held UI press, without a click
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
            TrySelectAll();
            RayUIPointerDown();
        }
        else if (!isPinching && wasPinching)
        {
            ForceExitAll();
            RayUIPointerUp();
        }

        wasPinching = isPinching;
    }
    // --- UI (Ray over Canvas) ---
    // Canvas UI Buttons are NOT IXRInteractable, so they never show up in
    // GetValidTargets() and can never be reached via SelectEnter/SelectExit.
    // Instead of relying on XRI's internal UI input plumbing (which differs
    // across package versions — some expose uiPressInput on the interactor,
    // older ones read it off the controller instead), we drive Unity's
    // standard UGUI event system directly. This fires the exact same
    // OnPointerDown/Up/Click callbacks a mouse click would, and works
    // regardless of XRI version.

    private GameObject rayPressedGO;

    private void RayUIPointerDown()
    {
        if (rayInteractor == null) return;
        if (!rayInteractor.TryGetCurrentUIRaycastResult(out var raycastResult)) return;

        var target = UnityEngine.EventSystems.ExecuteEvents.GetEventHandler<UnityEngine.EventSystems.IPointerDownHandler>(raycastResult.gameObject);
        if (target == null) return;

        var eventData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current)
        {
            pointerPress = target,
            pointerEnter = target,
            button = UnityEngine.EventSystems.PointerEventData.InputButton.Left
        };

        UnityEngine.EventSystems.ExecuteEvents.Execute(
            target, eventData, UnityEngine.EventSystems.ExecuteEvents.pointerDownHandler);

        rayPressedGO = target;
    }

    private void RayUIPointerUp()
    {
        if (rayPressedGO == null) return;

        var eventData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current)
        {
            pointerPress = rayPressedGO,
            pointerEnter = rayPressedGO,
            button = UnityEngine.EventSystems.PointerEventData.InputButton.Left
        };

        UnityEngine.EventSystems.ExecuteEvents.Execute(
            rayPressedGO, eventData, UnityEngine.EventSystems.ExecuteEvents.pointerUpHandler);

        // Only fire the click if the ray is still over the same element it
        // was pressed on — mirrors normal button behavior (drag-off cancels).
        bool stillOverSameTarget = false;
        if (rayInteractor != null && rayInteractor.TryGetCurrentUIRaycastResult(out var currentHover))
        {
            var currentTarget = UnityEngine.EventSystems.ExecuteEvents.GetEventHandler<UnityEngine.EventSystems.IPointerClickHandler>(currentHover.gameObject);
            stillOverSameTarget = currentTarget == rayPressedGO;
        }

        if (stillOverSameTarget)
        {
            UnityEngine.EventSystems.ExecuteEvents.Execute(
                rayPressedGO, eventData, UnityEngine.EventSystems.ExecuteEvents.pointerClickHandler);
        }

        rayPressedGO = null;
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