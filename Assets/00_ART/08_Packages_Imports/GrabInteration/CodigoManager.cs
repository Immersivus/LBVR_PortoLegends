using UnityEngine;

public class CodigoManager : MonoBehaviour
{
    [Header("Zona")]
    public string nomeZona = "Zona A";

    [Header("Botoes desta Zona")]
    public BotaoCodigo[] botoesDaZona;

    [Header("Sequencia Correta")]
    public int[] sequenciaCorreta = { 1, 2, 3, 4 };

    [Header("Objetos")]
    public GameObject objetoParaLigar;
    public GameObject objetoParaDesligar;

    private int indiceAtual = 0;

    public bool BotaoPressionado(int numeroBotao)
    {
        if (numeroBotao == sequenciaCorreta[indiceAtual])
        {
            indiceAtual++;
            if (indiceAtual >= sequenciaCorreta.Length)
            {
                if (objetoParaLigar != null)
                    objetoParaLigar.SetActive(true);
                if (objetoParaDesligar != null)
                    objetoParaDesligar.SetActive(false);
                TodosVerdes();
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    void TodosVerdes()
    {
        foreach (var botao in botoesDaZona)
            botao.FicarVerde();
    }

    void ResetTudo()
    {
        indiceAtual = 0;
        foreach (var botao in botoesDaZona)
            botao.ResetCor();
    }
}