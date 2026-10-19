using System.Collections.Generic;
using UnityEngine;

public class EstadoLivros : MonoBehaviour
{
    private static HashSet<string> livrosRetirados = new HashSet<string>();

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
        if (livrosRetirados.Contains(idLivro))
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

    public static int QuantidadeLivros()
    {
        return livrosRetirados.Count;
    }

    public static int LimiteMochila()
    {
        return LIMITE_MOCHILA;
    }
}