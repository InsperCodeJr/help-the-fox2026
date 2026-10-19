using UnityEngine;

public class DevolverLivroTeste : MonoBehaviour
{
    public int numeroLivro = 2;
    public GameObject botaoDevolver;

    void Update()
    {
        if (string.IsNullOrEmpty(PrateleiraInterativa.PrateleiraAtual))
        {
            botaoDevolver.SetActive(false);
            return;
        }

        string idLivro =
            PrateleiraInterativa.PrateleiraAtual
            + "_L"
            + numeroLivro.ToString("00");

        bool livroFoiRetirado =
            EstadoLivros.LivroFoiRetirado(idLivro);

        botaoDevolver.SetActive(livroFoiRetirado);
    }

    public void Devolver()
    {
        Debug.Log("BOTAO DEVOLVER FOI CLICADO!");

        string idLivro =
            PrateleiraInterativa.PrateleiraAtual
            + "_L"
            + numeroLivro.ToString("00");

        Debug.Log("Tentando devolver: " + idLivro);

        if (EstadoLivros.LivroFoiRetirado(idLivro))
        {
            EstadoLivros.DevolverLivro(idLivro);
            Debug.Log("DEVOLVIDO PARA A PRATELEIRA: " + idLivro);
        }
        else
        {
            Debug.Log("ERRO: esse livro nao consta como retirado.");
        }
    }
}