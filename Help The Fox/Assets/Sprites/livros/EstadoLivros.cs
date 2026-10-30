using System.Collections.Generic;
using UnityEngine;

public class EstadoLivros : MonoBehaviour
{
    // A ordem da lista registra a sequencia de coleta dos livros ainda na mochila.
    private static readonly List<string> livrosRetirados = new List<string>();

    private const int LIMITE_MOCHILA = 7;

    public static bool LivroFoiRetirado(string idLivro)
    {
        return livrosRetirados.Contains(idLivro);
    }

    public static bool MochilaEstaCheia()
    {
        return livrosRetirados.Count >= LIMITE_MOCHILA;
    }

    public static bool RetirarLivro(string idLivro)
    {
        if (string.IsNullOrEmpty(idLivro) || livrosRetirados.Contains(idLivro))
        {
            return false;
        }

        if (MochilaEstaCheia())
        {
            Debug.Log("MOCHILA CHEIA! Limite de 7 livros.");
            return false;
        }

        livrosRetirados.Add(idLivro);

        Debug.Log("LIVRO RETIRADO: " + idLivro);
        Debug.Log(
            "MOCHILA: "
            + livrosRetirados.Count
            + "/"
            + LIMITE_MOCHILA
        );

        return true;
    }

    public static void DevolverLivro(string idLivro)
    {
        if (livrosRetirados.Contains(idLivro))
        {
            livrosRetirados.Remove(idLivro);

            Debug.Log("LIVRO DEVOLVIDO: " + idLivro);
            Debug.Log(
                "MOCHILA: "
                + livrosRetirados.Count
                + "/"
                + LIMITE_MOCHILA
            );
        }
    }

    public static List<string> PegarLivrosDaMochila()
    {
        return new List<string>(livrosRetirados);
    }

    public static string PegarUltimoLivroPego()
    {
        return livrosRetirados.Count > 0 ? livrosRetirados[livrosRetirados.Count - 1] : string.Empty;
    }

    public static int QuantidadeLivros()
    {
        return livrosRetirados.Count;
    }

    public static int LimiteMochila()
    {
        return LIMITE_MOCHILA;
    }
}
