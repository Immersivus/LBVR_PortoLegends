using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Collider))]
public class SismoSimulator : MonoBehaviour
{
    [Header("Configurações do Tempo")]
    public float duracao = 5.0f;

    [Header("Transição (Suavização)")]
    public float tempoSuavizacao = 1.5f;

    [Header("Frequência do Sismo")]
    [Tooltip("Velocidade do tremor")]
    public float velocidadeSismo = 25.0f;

    [Header("Tamanho do Tremor por Eixo (Metros)")]
    [Tooltip("Lados: Movimento para a Esquerda e Direita")]
    public float tremorEixoX_Lados = 0.05f;

    [Tooltip("Cima / Baixo: Movimento Vertical")]
    public float tremorEixoY_CimaBaixo = 0.03f;

    [Tooltip("Frente / Trás: Movimento de Profundidade")]
    public float tremorEixoZ_FrenteTras = 0.02f;

    [Header("Efeito Shutter / Lag Visual")]
    public bool usarEfeitoShutter = false;
    public float fpsShutter = 12f;

    [Header("Efeito de Arrastamento Visual")]
    public Volume volumeDeArrastamento;

    [Header("Efeitos Adicionais (Partículas e Objetos)")]
    [Tooltip("Sistemas de partículas a disparar quando o sismo inicia")]
    public ParticleSystem[] sistemasParticulas;

    [Tooltip("Objetos a ativar durante o sismo e desativar no fim")]
    public GameObject[] objetosParaAtivar;

    [Header("Áudio do Sismo")]
    [Tooltip("Componente AudioSource com o som do terramoto (configura como 3D no Inspector para ser espacial)")]
    public AudioSource somSismo;

    [Tooltip("Volume máximo do som durante o pico do sismo")]
    [Range(0f, 1f)]
    public float volumeMaximoSom = 1.0f;

    private Transform cameraVR;
    private float tempoRestante = 0f;
    private bool efeitoAtivo = false;
    private float tempoUltimoShutter = 0f;

    private Vector3 posOriginalCamera;
    private Vector3 posShutterAtual;
    private float pesoAtualVolume = 0f;

    void Start()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            cameraVR = cam.transform;
            posOriginalCamera = cameraVR.localPosition;
        }

        if (volumeDeArrastamento != null)
        {
            volumeDeArrastamento.weight = 0f;
        }

        if (somSismo != null)
        {
            somSismo.volume = 0f;
        }
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

            // 2. CONTROLO DO ÁUDIO (Volume sobe e desce com a transição do sismo)
            if (somSismo != null)
            {
                somSismo.volume = multiplicadorForca * volumeMaximoSom;
            }

            // 3. POSIÇÃO DA CÂMERA (TREMOR DE SISMO)
            if (cameraVR != null)
            {
                if (usarEfeitoShutter)
                {
                    float intervaloShutter = 1f / fpsShutter;
                    if (Time.time - tempoUltimoShutter >= intervaloShutter)
                    {
                        tempoUltimoShutter = Time.time;
                        posShutterAtual = posOriginalCamera + CalcularPosicaoSismo(multiplicadorForca);
                    }
                    cameraVR.localPosition = Vector3.Lerp(cameraVR.localPosition, posShutterAtual, Time.deltaTime * 15f);
                }
                else
                {
                    Vector3 posAlvo = posOriginalCamera + CalcularPosicaoSismo(multiplicadorForca);
                    cameraVR.localPosition = Vector3.Lerp(cameraVR.localPosition, posAlvo, Time.deltaTime * 15f);
                }
            }

            if (tempoRestante <= 0)
            {
                PararEfeito();
            }
        }
        else
        {
            // Regressa suavemente à posição local original
            if (cameraVR != null && cameraVR.localPosition != posOriginalCamera)
            {
                cameraVR.localPosition = Vector3.Lerp(cameraVR.localPosition, posOriginalCamera, Time.deltaTime * 4f);
            }

            if (volumeDeArrastamento != null && volumeDeArrastamento.weight > 0f)
            {
                volumeDeArrastamento.weight = Mathf.Lerp(volumeDeArrastamento.weight, 0f, Time.deltaTime * 4f);
            }

            // Faz fade out do som se ainda estiver a tocar
            if (somSismo != null && somSismo.volume > 0f)
            {
                somSismo.volume = Mathf.Lerp(somSismo.volume, 0f, Time.deltaTime * 5f);
                if (somSismo.volume <= 0.01f)
                {
                    somSismo.Stop();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!efeitoAtivo)
        {
            if (cameraVR == null && Camera.main != null)
            {
                cameraVR = Camera.main.transform;
                posOriginalCamera = cameraVR.localPosition;
            }

            IniciarEfeito();
        }
    }

    private Vector3 CalcularPosicaoSismo(float multiplicadorForca)
    {
        float xOffset = Mathf.Sin(Time.time * velocidadeSismo) * (tremorEixoX_Lados * multiplicadorForca);
        float yOffset = Mathf.Cos(Time.time * velocidadeSismo * 1.3f) * (tremorEixoY_CimaBaixo * multiplicadorForca);
        float zOffset = Mathf.Sin(Time.time * velocidadeSismo * 0.7f) * (tremorEixoZ_FrenteTras * multiplicadorForca);

        return new Vector3(xOffset, yOffset, zOffset);
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

    public void IniciarEfeito()
    {
        tempoRestante = duracao;
        efeitoAtivo = true;
        tempoUltimoShutter = Time.time;

        if (cameraVR != null)
        {
            posOriginalCamera = cameraVR.localPosition;
            posShutterAtual = posOriginalCamera;
        }

        // Toca o som
        if (somSismo != null)
        {
            somSismo.volume = 0f;
            somSismo.Play();
        }

        // Ativa Partículas
        if (sistemasParticulas != null)
        {
            foreach (ParticleSystem ps in sistemasParticulas)
            {
                if (ps != null) ps.Play();
            }
        }

        // Ativa Objetos extra
        if (objetosParaAtivar != null)
        {
            foreach (GameObject obj in objetosParaAtivar)
            {
                if (obj != null) obj.SetActive(true);
            }
        }
    }

    private void PararEfeito()
    {
        efeitoAtivo = false;
        tempoRestante = 0f;

        // Desliga Partículas
        if (sistemasParticulas != null)
        {
            foreach (ParticleSystem ps in sistemasParticulas)
            {
                if (ps != null) ps.Stop();
            }
        }

        // Desliga Objetos extra
        if (objetosParaAtivar != null)
        {
            foreach (GameObject obj in objetosParaAtivar)
            {
                if (obj != null) obj.SetActive(false);
            }
        }
    }
}