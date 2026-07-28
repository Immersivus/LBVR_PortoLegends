using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Collider))]
public class DrunkEffect2 : MonoBehaviour
{
    [Header("Configurações do Tempo")]
    public float duracao = 5.0f;

    [Header("Transição (Suavização)")]
    public float tempoSuavizacao = 1.5f;

    [Header("Intensidade do Balanço")]
    public float velocidadeBalanco = 2.5f;
    public float anguloBalanco = 6.0f;

    [Header("Efeito Shutter / Lag Visual")]
    public bool usarEfeitoShutter = false;
    public float fpsShutter = 12f;

    [Header("Efeito de Arrastamento Visual")]
    public Volume volumeDeArrastamento;

    private Transform cameraVR;
    private float tempoRestante = 0f;
    private bool efeitoAtivo = false;
    private float tempoUltimoShutter = 0f;
    private Quaternion rotacaoShutterAtual;
    private float pesoAtualVolume = 0f;

    // Posições de origem
    private Vector3 startingWorldPosition;
    private Quaternion startingWorldRotation;
    private Transform initialParent;
    private Rigidbody rb;

    void Start()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            cameraVR = cam.transform;
        }

        if (volumeDeArrastamento != null)
        {
            volumeDeArrastamento.weight = 0f;
        }

        // Guarda o estado inicial exato do objeto
        startingWorldPosition = transform.position;
        startingWorldRotation = transform.rotation;
        initialParent = transform.parent;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (efeitoAtivo)
        {
            tempoRestante -= Time.deltaTime;
            float multiplicadorForca = CalcularMultiplicadorSuave();

            // 1. ATUALIZAÇÃO DO POST PROCESSING
            if (volumeDeArrastamento != null)
            {
                pesoAtualVolume = Mathf.Lerp(pesoAtualVolume, multiplicadorForca, Time.deltaTime * 5f);
                volumeDeArrastamento.weight = pesoAtualVolume;
            }

            // 2. ROTAÇÃO DA CÂMERA
            if (cameraVR != null)
            {
                if (usarEfeitoShutter)
                {
                    float intervaloShutter = 1f / fpsShutter;
                    if (Time.time - tempoUltimoShutter >= intervaloShutter)
                    {
                        tempoUltimoShutter = Time.time;
                        rotacaoShutterAtual = CalcularRotacaoTontura(multiplicadorForca);
                    }
                    cameraVR.localRotation = Quaternion.Lerp(cameraVR.localRotation, rotacaoShutterAtual, Time.deltaTime * 10f);
                }
                else
                {
                    Quaternion rotacaoAlvo = CalcularRotacaoTontura(multiplicadorForca);
                    cameraVR.localRotation = Quaternion.Lerp(cameraVR.localRotation, rotacaoAlvo, Time.deltaTime * 8f);
                }
            }

            if (tempoRestante <= 0)
            {
                PararEfeito();
            }
        }
        else
        {
            if (cameraVR != null && cameraVR.localRotation != Quaternion.identity)
            {
                cameraVR.localRotation = Quaternion.Lerp(cameraVR.localRotation, Quaternion.identity, Time.deltaTime * 4f);
            }

            if (volumeDeArrastamento != null && volumeDeArrastamento.weight > 0f)
            {
                volumeDeArrastamento.weight = Mathf.Lerp(volumeDeArrastamento.weight, 0f, Time.deltaTime * 4f);
            }
        }
    }

    private Quaternion CalcularRotacaoTontura(float multiplicadorForca)
    {
        float zTilt = Mathf.Sin(Time.time * velocidadeBalanco) * (anguloBalanco * multiplicadorForca);
        float xTilt = Mathf.Cos(Time.time * velocidadeBalanco * 0.5f) * (anguloBalanco * 0.5f * multiplicadorForca);
        return Quaternion.Euler(xTilt, 0, zTilt);
    }

    private float CalcularMultiplicadorSuave()
    {
        float tempoDecorrido = duracao - tempoRestante;
        float tempoTransicaoAjustado = Mathf.Min(tempoSuavizacao, duracao / 2f);

        if (tempoDecorrido < tempoTransicaoAjustado)
        {
            return Mathf.SmoothStep(0f, 1f, tempoDecorrido / tempoTransicaoAjustado);
        }
        else if (tempoRestante < tempoTransicaoAjustado)
        {
            return Mathf.SmoothStep(0f, 1f, tempoRestante / tempoTransicaoAjustado);
        }

        return 1.0f;
    }

    public void OnTriggerEnterReal()
    {
        if (!efeitoAtivo)
        {
            if (cameraVR == null && Camera.main != null)
            {
                cameraVR = Camera.main.transform;
            }

            IniciarEfeito();
        }
    }

    public void IniciarEfeito()
    {
        tempoRestante = duracao;
        efeitoAtivo = true;
        tempoUltimoShutter = Time.time;
        if (cameraVR != null) rotacaoShutterAtual = cameraVR.localRotation;
    }

    private void PararEfeito()
    {
        efeitoAtivo = false;
        tempoRestante = 0f;
    }

    // Chamado pelo evento do XR Grab Interactable ao largar (Select Exited)
    public void Largar()
    {
        OnTriggerEnterReal();
        StartCoroutine(ResetPosicaoRotina());
    }

    private IEnumerator ResetPosicaoRotina()
    {
        // Espera 1 frame simples para o XR Toolkit soltar completamente o objeto
        yield return null;

        // Repõe o pai original caso tenha mudado
        transform.SetParent(initialParent);

        // Repõe a posição e rotação originais no mundo
        transform.position = startingWorldPosition;
        transform.rotation = startingWorldRotation;

        // Anula forças físicas sem mexer no isKinematic (elimina o offset na mão ao agarrar)
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep();
        }
    }
}