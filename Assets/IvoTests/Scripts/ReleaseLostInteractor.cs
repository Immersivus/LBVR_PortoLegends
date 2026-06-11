using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ReleaseLostInteractor : MonoBehaviour
{
    XRGrabInteractable grab;
    XRInteractionManager interactionManager;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        interactionManager = FindObjectOfType<XRInteractionManager>();
    }

    void Update()
    {
        if (!grab.isSelected)
            return;

        var interactor = grab.firstInteractorSelecting;

        if (interactor == null || !interactor.transform.gameObject.activeInHierarchy)
        {
            Debug.Log("Interactor disappeared");
            for (int i = interactor.interactablesSelected.Count - 1; i >= 0; i--)
            {
                var interactable = interactor.interactablesSelected[i];

                interactionManager.SelectCancel(interactor, interactable);
            }
        }
    }
}
