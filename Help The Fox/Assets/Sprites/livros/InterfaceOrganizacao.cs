using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InterfaceOrganizacao : MonoBehaviour
{
    public TMP_Text[] textosLivros;
    public PosicaoOrganizacao[] posicoes;

    [Header("Ordem correta dos livros")]
    public string[] ordemCorreta = new string[5];

    [Header("Conclusao")]
    public GameObject mensagemConclusao;

    private List<string> livrosAtuais = new List<string>();

    private string livroSelecionado = "";

    private string[] livrosNasPosicoes = new string[5];

    private bool puzzleConcluido = false;

    void Start()
    {
        if (mensagemConclusao != null)
        {
            mensagemConclusao.SetActive(false);
        }
    }

    void Update()
    {
        AtualizarLivros();
    }

    void AtualizarLivros()
    {
        List<string> mochila =
            EstadoLivros.PegarLivrosDaMochila();

        livrosAtuais.Clear();

        foreach (string livro in mochila)
        {
            if (!LivroJaEstaOrganizado(livro))
            {
                livrosAtuais.Add(livro);
            }
        }

        for (int i = 0; i < textosLivros.Length; i++)
        {
            if (i < livrosAtuais.Count)
            {
                textosLivros[i].text = livrosAtuais[i];
                textosLivros[i].gameObject.SetActive(true);
            }
            else
            {
                textosLivros[i].text = "";
                textosLivros[i].gameObject.SetActive(false);
            }
        }
    }

    bool LivroJaEstaOrganizado(string idLivro)
    {
        for (int i = 0; i < livrosNasPosicoes.Length; i++)
        {
            if (livrosNasPosicoes[i] == idLivro)
            {
                return true;
            }
        }

        return false;
    }

    public void SelecionarLivro(int indice)
    {
        if (puzzleConcluido)
        {
            return;
        }

        if (indice < 0 || indice >= livrosAtuais.Count)
        {
            return;
        }

        livroSelecionado = livrosAtuais[indice];

        Debug.Log("LIVRO SELECIONADO: " + livroSelecionado);

        AtualizarDestaque();
    }

    void AtualizarDestaque()
    {
        for (int i = 0; i < textosLivros.Length; i++)
        {
            LivroOrganizacao livro =
                textosLivros[i].GetComponent<LivroOrganizacao>();

            if (livro != null)
            {
                bool selecionado =
                    i < livrosAtuais.Count &&
                    livrosAtuais[i] == livroSelecionado;

                livro.MostrarSelecionado(selecionado);
            }
        }
    }

    public void ColocarLivroNaPosicao(int numeroPosicao)
    {
        if (puzzleConcluido)
        {
            return;
        }

        int indicePosicao = numeroPosicao - 1;

        if (
            indicePosicao < 0 ||
            indicePosicao >= livrosNasPosicoes.Length
        )
        {
            return;
        }

        if (!string.IsNullOrEmpty(livrosNasPosicoes[indicePosicao]))
        {
            string livroRetirado =
                livrosNasPosicoes[indicePosicao];

            livrosNasPosicoes[indicePosicao] = "";

            posicoes[indicePosicao].AtualizarTexto("");

            livroSelecionado = "";

            AtualizarLivros();
            AtualizarDestaque();

            Debug.Log(
                livroRetirado
                + " retirado da posicao "
                + numeroPosicao
            );

            return;
        }

        if (string.IsNullOrEmpty(livroSelecionado))
        {
            Debug.Log("Selecione um livro primeiro.");
            return;
        }

        livrosNasPosicoes[indicePosicao] =
            livroSelecionado;

        Debug.Log(
            livroSelecionado
            + " colocado na posicao "
            + numeroPosicao
        );

        posicoes[indicePosicao].AtualizarTexto(
            livroSelecionado
        );

        livroSelecionado = "";

        AtualizarLivros();
        AtualizarDestaque();

        VerificarVitoria();
    }

    void VerificarVitoria()
    {
        if (ordemCorreta.Length != 5)
        {
            return;
        }

        for (int i = 0; i < 5; i++)
        {
            if (livrosNasPosicoes[i] != ordemCorreta[i])
            {
                return;
            }
        }

        puzzleConcluido = true;

        Debug.Log("PUZZLE CONCLUIDO!");

        if (mensagemConclusao != null)
        {
            mensagemConclusao.SetActive(true);
        }

        gameObject.SetActive(false);
    }

    public string PegarLivroSelecionado()
    {
        return livroSelecionado;
    }
}