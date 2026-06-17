using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class HandTrackingRecovery : MonoBehaviour
{
    [SerializeField] private GameObject leftHandRoot;
    [SerializeField] private GameObject rightHandRoot;
    [SerializeField] private XRInteractionManager interactionManager;
    [SerializeField] private XRInputModalityManager modalityManager;

    private InputDevice leftHand;
    private InputDevice rightHand;
    private bool leftWasTracked;
    private bool rightWasTracked;

    private void OnEnable()
    {
        InputDevices.deviceConnected += OnDeviceConnected;
        TryInitDevices();
    }

    private void OnDisable()
    {
        InputDevices.deviceConnected -= OnDeviceConnected;
    }

    private void OnDeviceConnected(InputDevice device) => TryInitDevices();

    private void TryInitDevices()
    {
        if (!leftHand.isValid)
            leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        if (!rightHand.isValid)
            rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    private void Update()
    {
        CheckHand(leftHand, leftHandRoot, ref leftWasTracked);
        CheckHand(rightHand, rightHandRoot, ref rightWasTracked);
    }

    private void CheckHand(InputDevice device, GameObject handRoot, ref bool wasTracked)
    {
        if (!device.isValid) return;

        device.TryGetFeatureValue(CommonUsages.isTracked, out bool isTracked);

        if (!isTracked && wasTracked)
        {
            ForceExitAllInteractions(handRoot);
            ForceResetStuckInteractables();
        }

        wasTracked = isTracked;
    }

    // Mimics what the Pico menu return does — full modality manager reset
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
            StartCoroutine(RecoverOnFocus());
    }

    private IEnumerator RecoverOnFocus()
    {
        ForceExitAllInteractions(leftHandRoot);
        ForceExitAllInteractions(rightHandRoot);
        ForceResetStuckInteractables();

        yield return null;

        // Force the modality manager to re-evaluate hand vs controller state
        if (modalityManager != null)
        {
            modalityManager.enabled = false;
            yield return null;
            modalityManager.enabled = true;
        }
    }

    private void ForceExitAllInteractions(GameObject handRoot)
    {
        if (handRoot == null) return;

        var interactors = handRoot.GetComponentsInChildren<XRBaseInteractor>(includeInactive: true);

        foreach (var interactor in interactors)
        {
            var selecting = new List<IXRSelectInteractable>(interactor.interactablesSelected);
            var hovering = new List<IXRHoverInteractable>(interactor.interactablesHovered);

            foreach (var interactable in selecting)
                interactionManager.SelectExit((IXRSelectInteractor)interactor, interactable);

            foreach (var interactable in hovering)
                interactionManager.HoverExit((IXRHoverInteractor)interactor, interactable);
        }
    }

    private void ForceResetStuckInteractables()
    {
        var allInteractables = FindObjectsByType<XRGrabInteractable>(FindObjectsSortMode.None);

        foreach (var interactable in allInteractables)
        {
            if (!interactable.isSelected) continue;

            var selecting = new List<IXRSelectInteractor>(interactable.interactorsSelecting);

            foreach (var interactor in selecting)
            {
                var interactorBehaviour = interactor as MonoBehaviour;
                if (interactorBehaviour == null || !interactorBehaviour.gameObject.activeInHierarchy)
                    interactionManager.SelectExit(interactor, interactable);
            }
        }
    }
}
