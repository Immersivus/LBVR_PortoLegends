using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ResetObjeto : MonoBehaviour
{
    [Header("Configuração")]
    public float alturaMinima = -1f;
    public float tempoParaReset = 3f;

    private Vector3 posicaoInicial;
    private Quaternion rotacaoInicial;
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private bool agarrado = false;
    private float tempoNoChao = 0f;

    void Start()
    {
        posicaoInicial = transform.position;
        rotacaoInicial = transform.rotation;

        rb = GetComponent<Rigidbody>();

        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener((args) => agarrado = true);
            grabInteractable.selectExited.AddListener((args) => agarrado = false);
        }
    }

    void Update()
    {
        if (agarrado) return;

        if (transform.position.y < alturaMinima)
        {
            tempoNoChao += Time.deltaTime;
            if (tempoNoChao >= tempoParaReset)
            {
                ResetarPosicao();
            }
        }
        else
        {
            tempoNoChao = 0f;
        }
    }

    void ResetarPosicao()
    {
        tempoNoChao = 0f;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = posicaoInicial;
        transform.rotation = rotacaoInicial;
    }
}