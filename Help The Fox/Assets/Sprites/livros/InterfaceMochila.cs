using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InterfaceMochila : MonoBehaviour
{
    public GameObject painelMochila;
    public TMP_Text textoMochila;
    public MovimentoRaposa movimentoRaposa;

    private bool mochilaAberta = false;

    void Start()
    {
        painelMochila.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (mochilaAberta)
            {
                FecharMochila();
            }
            else
            {
                AbrirMochila();
            }
        }
    }

    void AbrirMochila()
    {
        mochilaAberta = true;

        movimentoRaposa.PararMovimento();
        movimentoRaposa.enabled = false;

        AtualizarTexto();

        painelMochila.SetActive(true);
    }

    void FecharMochila()
    {
        mochilaAberta = false;

        painelMochila.SetActive(false);

        movimentoRaposa.enabled = true;
    }

    void AtualizarTexto()
    {
        List<string> livros = EstadoLivros.PegarLivrosDaMochila();

        string texto =
            "MOCHILA - "
            + EstadoLivros.QuantidadeLivros()
            + "/"
            + EstadoLivros.LimiteMochila()
            + "\n\n";

        if (livros.Count == 0)
        {
            texto += "Nenhum livro";
        }
        else
        {
            foreach (string livro in livros)
            {
                texto += livro + "\n";
            }
        }

        textoMochila.text = texto;
    }
}
