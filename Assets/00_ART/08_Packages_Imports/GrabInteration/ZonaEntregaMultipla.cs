using UnityEngine;

public class ZonaEntregaMultipla : MonoBehaviour
{
    [Header("Configuração")]
    public float distanciaEntrega = 0.3f;

    [Header("Objetos a Entregar e Recompensas")]
    public EntregaItem[] itens;

    [Header("Recompensa Final")]
    public GameObject objetoFinal;

    private bool jaLigou = false;

    void Update()
    {
        if (jaLigou) return;

        bool todosEntregues = true;

        for (int i = 0; i < itens.Length; i++)
        {
            if (itens[i].entregue) continue;
            if (itens[i].objetoEsperado == null) continue;

            float distancia = Vector3.Distance(
                itens[i].objetoEsperado.transform.position,
                transform.position
            );

            if (distancia <= distanciaEntrega)
            {
                itens[i].entregue = true;
                itens[i].objetoEsperado.SetActive(false);

                if (itens[i].recompensa != null)
                    itens[i].recompensa.SetActive(true);
            }

            if (!itens[i].entregue)
                todosEntregues = false;
        }

        if (todosEntregues)
        {
            jaLigou = true;
            if (objetoFinal != null)
                objetoFinal.SetActive(true);
        }
    }
}

[System.Serializable]
public class EntregaItem
{
    public GameObject objetoEsperado;
    public GameObject recompensa;
    [HideInInspector] public bool entregue = false;
}