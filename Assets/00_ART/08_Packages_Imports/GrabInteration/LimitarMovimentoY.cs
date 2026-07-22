using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LimitarMovimento : MonoBehaviour
{
    [Header("Limites Y")]
    public float yMinimo = -0.5f;
    public float yMaximo = 0f;

    [Header("Som")]
    public AudioClip somApito;
    public float yParaAtivarSom = -0.3f;

    [Header("Retorno")]
    public float velocidadeRetorno = 1f;

    [Header("Particulas")]
    public ParticleSystem fumo;

    private Vector3 posicaoInicial;
    public AudioSource audioSource;
    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private bool agarrado = false;

    private bool regressa;

    void Start()
    {
        posicaoInicial = transform.localPosition;

        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener((args) => OnAgarrar());
        grabInteractable.selectExited.AddListener((args) => OnLargar());

        rb = GetComponent<Rigidbody>();
    }

    void OnAgarrar()
    {
        agarrado = true;
        rb.useGravity = false;
        rb.isKinematic = false;
    }

    void OnLargar()
    {
        agarrado = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    void Update()
    {
        if (agarrado)
        {
            regressa = false;
            if (transform.position.y <= posicaoInicial.y + yParaAtivarSom)
            {
                if (!audioSource.isPlaying)
                    audioSource.Play();
                if (fumo != null && !fumo.isPlaying)
                    fumo.Play();
            }
            else
            {
                if (audioSource.isPlaying)
                    audioSource.Stop();
                if (fumo != null && fumo.isPlaying)
                    fumo.Stop();
            }
        }

        if (regressa)
        {
           if(Vector3.Distance(transform.position, posicaoInicial) > 0.001f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    posicaoInicial,
                    velocidadeRetorno * Time.deltaTime
                );
            }
            else
            {
                transform.position = posicaoInicial;
                regressa = false;
            }
        }
    }

    public void Return()
    {
        regressa = true;
    }
}