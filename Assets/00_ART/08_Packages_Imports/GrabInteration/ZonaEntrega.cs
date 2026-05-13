using UnityEngine;

public class ZonaEntrega : MonoBehaviour
{
    [Header("Configuração")]
    public GameObject objetoEsperado;
    public GameObject objetoParaLigar;
    public float distanciaEntrega = 0.3f;

    private bool entregue = false;

    void Update()
    {
        if (entregue) return;
        if (objetoEsperado == null) return;

        float distancia = Vector3.Distance(
            objetoEsperado.transform.position,
            transform.position
        );

        if (distancia <= distanciaEntrega)
        {
            entregue = true;
            objetoEsperado.SetActive(false);

            if (objetoParaLigar != null)
                objetoParaLigar.SetActive(true);
        }
    }
}