using UnityEngine;

public class AlavancaManager : MonoBehaviour
{
    [Header("Alavancas")]
    public Alavanca[] alavancas;

    [Header("Objeto a Ligar")]
    public GameObject objetoParaLigar;

    private bool jaLigou = false;

    void Update()
    {
        if (jaLigou) return;

        bool todasLigadas = true;
        foreach (var alavanca in alavancas)
        {
            if (!alavanca.ligado)
            {
                todasLigadas = false;
                break;
            }
        }

        if (todasLigadas)
        {
            jaLigou = true;
            if (objetoParaLigar != null)
                objetoParaLigar.SetActive(true);
        }
    }
}