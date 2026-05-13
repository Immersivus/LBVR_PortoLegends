using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BotaoCodigo : MonoBehaviour
{
    public int numeroBotao;
    public CodigoManager codigoManager;
    public bool correto = false;

    private XRSimpleInteractable interactable;
    private Renderer botaoRenderer;
    private bool dedoDentro = false;

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
        dedoDentro = true;
        bool foiCorreto = codigoManager.BotaoPressionado(numeroBotao);
        if (foiCorreto)
            correto = true;
        botaoRenderer.material.color = Color.green;
    }

    void OnBotaoSolto(HoverExitEventArgs args)
    {
        dedoDentro = false;
        if (!correto)
            botaoRenderer.material.color = Color.red;
    }

    public void FicarVerde()
    {
        correto = true;
        botaoRenderer.material.color = Color.green;
    }

    public void ResetCor()
    {
        correto = false;
        dedoDentro = false;
        botaoRenderer.material.color = Color.red;
    }
}