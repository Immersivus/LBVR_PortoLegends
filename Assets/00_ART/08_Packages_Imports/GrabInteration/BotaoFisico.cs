using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BotaoTest : MonoBehaviour
{
    public GameObject objetoLigarPermanente;
    public GameObject objetoLigarTemporario;

    private XRSimpleInteractable interactable;
    private Renderer botaoRenderer;

    void Start()
    {
        botaoRenderer = GetComponent<Renderer>();
        botaoRenderer.material.color = Color.red;

        interactable = GetComponent<XRSimpleInteractable>();
        interactable.hoverEntered.AddListener(OnBotaoClicado);
        interactable.hoverExited.AddListener(OnBotaoSolto);
    }

    void OnBotaoClicado(HoverEnterEventArgs args)
    {
        botaoRenderer.material.color = Color.green;

        if (objetoLigarPermanente != null)
            objetoLigarPermanente.SetActive(true);

        if (objetoLigarTemporario != null)
            objetoLigarTemporario.SetActive(true);
    }

    void OnBotaoSolto(HoverExitEventArgs args)
    {
        botaoRenderer.material.color = Color.red;

        if (objetoLigarTemporario != null)
            objetoLigarTemporario.SetActive(false);
    }
}