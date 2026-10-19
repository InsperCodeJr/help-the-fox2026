using System.Collections.Generic;
using UnityEngine;

public class EstadoLivros : MonoBehaviour
{
    private static HashSet<string> livrosRetirados = new HashSet<string>();

    public static bool LivroFoiRetirado(string idLivro)
    {
        return livrosRetirados.Contains(idLivro);
    }

    public static void RetirarLivro(string idLivro)
    {
        if (!livrosRetirados.Contains(idLivro))
        {
            livrosRetirados.Add(idLivro);

            Debug.Log("LIVRO RETIRADO: " + idLivro);
        }
    }

    public static void DevolverLivro(string idLivro)
    {
        if (livrosRetirados.Contains(idLivro))
        {
            livrosRetirados.Remove(idLivro);

            Debug.Log("LIVRO DEVOLVIDO: " + idLivro);
        }
    }
}