using UnityEngine;

public class PuzzleFinal : MonoBehaviour
{
    [Header("Configuração")]
    public float distanciaEntrega = 0.3f;
    public GameObject objetoConcluidoFinal;

    [Header("Itens do Puzzle")]
    public PuzzleItem[] itens;

    private bool concluido = false;

    void Update()
    {
        if (concluido) return;

        bool todosConcluidos = true;

        foreach (var item in itens)
        {
            if (item.entregue) continue;
            if (item.objeto == null || item.zonaEntrega == null) continue;

            float distancia = Vector3.Distance(
                item.objeto.transform.position,
                item.zonaEntrega.position
            );

            if (distancia <= distanciaEntrega)
            {
                item.entregue = true;
                item.objeto.SetActive(false);

                if (item.objetoParaLigar1 != null)
                    item.objetoParaLigar1.SetActive(true);
                if (item.objetoParaLigar2 != null)
                    item.objetoParaLigar2.SetActive(true);
            }

            if (!item.entregue)
                todosConcluidos = false;
        }

        if (todosConcluidos && itens.Length > 0)
        {
            concluido = true;
            if (objetoConcluidoFinal != null)
                objetoConcluidoFinal.SetActive(true);
        }
    }
}

[System.Serializable]
public class PuzzleItem
{
    public string nome;
    public GameObject objeto;
    public Transform zonaEntrega;
    public GameObject objetoParaLigar1;
    public GameObject objetoParaLigar2;
    [HideInInspector] public bool entregue = false;
}