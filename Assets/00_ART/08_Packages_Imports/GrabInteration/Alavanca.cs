using UnityEngine;
using UnityEngine.XR.Hands;
using System.Collections.Generic;

public class Alavanca : MonoBehaviour
{
    public enum OrientacaoAlavanca { Horizontal, Vertical }
    public enum TipoMovimento { Lateral, Frontal }

    [Header("Configuração")]
    public Transform pivot;
    public Transform pontoAgarre;
    public string nomeXRRig = "XRoamVRRig";
    public OrientacaoAlavanca orientacao = OrientacaoAlavanca.Horizontal;
    public TipoMovimento tipoMovimento = TipoMovimento.Lateral;
    public float anguloDesligado = -45f;
    public float anguloLigado = 45f;
    public float distanciaAgarrar = 0.3f;
    public float escalaMovimento = 0.2f;

    [Header("Objeto a Ligar")]
    public GameObject objetoParaLigar;

    private Transform xrRig;
    private bool agarrado = false;
    public bool ligado = false;
    private bool pinchEsquerdaAnterior = false;
    private bool pinchDireitaAnterior = false;
    private int maoQueAgarra = 0;

    void Start()
    {
        AplicarRotacaoInicial(anguloDesligado);

        var rig = GameObject.Find(nomeXRRig);
        if (rig != null)
            xrRig = rig.transform;
    }

    void Update()
    {
        if (xrRig == null)
        {
            var rig = GameObject.Find(nomeXRRig);
            if (rig != null)
                xrRig = rig.transform;
            else
                return;
        }

        if (HandTrackingManager.Instance == null) return;
        var handSubsystem = HandTrackingManager.Instance.HandSubsystem;
        if (handSubsystem == null || !handSubsystem.running) return;

        bool pinchEsquerda = GetPinch(handSubsystem.leftHand, out Vector3 posEsquerda);
        bool pinchDireita = GetPinch(handSubsystem.rightHand, out Vector3 posDireita);

        if (!agarrado)
        {
            if (pinchEsquerda && !pinchEsquerdaAnterior &&
                Vector3.Distance(posEsquerda, pontoAgarre.position) < distanciaAgarrar)
            {
                agarrado = true;
                maoQueAgarra = 1;
            }
            else if (pinchDireita && !pinchDireitaAnterior &&
                Vector3.Distance(posDireita, pontoAgarre.position) < distanciaAgarrar)
            {
                agarrado = true;
                maoQueAgarra = 2;
            }
        }

        if (agarrado)
        {
            bool pinchAtual = maoQueAgarra == 1 ? pinchEsquerda : pinchDireita;
            Vector3 posicaoMao = maoQueAgarra == 1 ? posEsquerda : posDireita;

            if (pinchAtual)
            {
                Vector3 direcao = posicaoMao - pivot.position;
                float input = 0f;

                if (orientacao == OrientacaoAlavanca.Horizontal)
                {
                    input = tipoMovimento == TipoMovimento.Lateral
                        ? -direcao.x
                        : direcao.z;
                }
                else
                {
                    input = tipoMovimento == TipoMovimento.Lateral
                        ? -direcao.x
                        : direcao.y;
                }

                float angulo = Mathf.Lerp(anguloDesligado, anguloLigado,
                    Mathf.InverseLerp(-escalaMovimento, escalaMovimento, input));
                angulo = Mathf.Clamp(angulo, anguloDesligado, anguloLigado);

                AplicarRotacao(angulo);

                float anguloAtual = orientacao == OrientacaoAlavanca.Horizontal
                    ? pivot.localEulerAngles.y
                    : pivot.localEulerAngles.z;
                if (anguloAtual > 180f) anguloAtual -= 360f;

                float limite = Mathf.Lerp(anguloDesligado, anguloLigado, 0.8f);
                bool novoEstado = anguloAtual >= limite;
                if (novoEstado != ligado)
                {
                    ligado = novoEstado;
                    if (objetoParaLigar != null)
                        objetoParaLigar.SetActive(ligado);
                }
            }
            else
            {
                agarrado = false;
                maoQueAgarra = 0;
                SnapParaPosicao();
            }
        }

        pinchEsquerdaAnterior = pinchEsquerda;
        pinchDireitaAnterior = pinchDireita;
    }

    void AplicarRotacaoInicial(float angulo)
    {
        if (orientacao == OrientacaoAlavanca.Horizontal)
            pivot.localRotation = Quaternion.Euler(0f, angulo, 0f);
        else
            pivot.localRotation = Quaternion.Euler(0f, 0f, angulo);
    }

    void AplicarRotacao(float angulo)
    {
        if (orientacao == OrientacaoAlavanca.Horizontal)
            pivot.localRotation = Quaternion.Euler(0f, angulo, 0f);
        else
            pivot.localRotation = Quaternion.Euler(0f, 0f, angulo);
    }

    bool GetPinch(XRHand mao, out Vector3 posicaoMao)
    {
        posicaoMao = Vector3.zero;
        if (!mao.isTracked) return false;

        var thumbJoint = mao.GetJoint(XRHandJointID.ThumbTip);
        var indexJoint = mao.GetJoint(XRHandJointID.IndexTip);

        if (!thumbJoint.TryGetPose(out var thumbPose)) return false;
        if (!indexJoint.TryGetPose(out var indexPose)) return false;

        Vector3 thumbWorld = xrRig != null
            ? xrRig.TransformPoint(thumbPose.position)
            : thumbPose.position;
        Vector3 indexWorld = xrRig != null
            ? xrRig.TransformPoint(indexPose.position)
            : indexPose.position;

        posicaoMao = (thumbWorld + indexWorld) / 2f;
        float distancia = Vector3.Distance(thumbWorld, indexWorld);
        return distancia < 0.03f;
    }

    void SnapParaPosicao()
    {
        float anguloAtual = orientacao == OrientacaoAlavanca.Horizontal
            ? pivot.localEulerAngles.y
            : pivot.localEulerAngles.z;

        if (anguloAtual > 180f) anguloAtual -= 360f;

        float limite = Mathf.Lerp(anguloDesligado, anguloLigado, 0.8f);
        bool novoEstado = anguloAtual >= limite;
        if (novoEstado != ligado)
        {
            ligado = novoEstado;
            if (objetoParaLigar != null)
                objetoParaLigar.SetActive(ligado);
        }
    }
}