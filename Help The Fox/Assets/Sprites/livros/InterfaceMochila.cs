using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InterfaceMochila : MonoBehaviour
{
    public GameObject painelMochila;
    public TMP_Text textoMochila;
    public MovimentoRaposa movimentoRaposa;

    [Header("Visual dos livros")]
    [SerializeField] private InterfaceOrganizacao organizacaoLivros;

    private bool mochilaAberta;
    private RectTransform[] itensLivros;
    private VisualLivroOrganizacao[] visuaisLivros;
    private TMP_Text textoVazio;
    private readonly List<string> livrosExibidos = new List<string>();

    void Start()
    {
        InicializarVisualizacao();
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

        if (mochilaAberta)
        {
            AtualizarTexto();
        }
    }

    void InicializarVisualizacao()
    {
        if (itensLivros != null) return;

        if (organizacaoLivros == null)
        {
            organizacaoLivros = GetComponentInChildren<InterfaceOrganizacao>(true);
        }

        PosicionarNoTopo(textoMochila.rectTransform, -45f, new Vector2(320f, 50f));
        textoMochila.alignment = TextAlignmentOptions.Center;
        textoMochila.textWrappingMode = TextWrappingModes.NoWrap;
        textoMochila.enableAutoSizing = true;
        textoMochila.fontSizeMin = 18f;
        textoMochila.fontSizeMax = 36f;

        textoVazio = CriarTexto("MochilaVazia");
        PosicionarNoTopo(textoVazio.rectTransform, -110f, new Vector2(320f, 50f));
        textoVazio.text = "Nenhum livro";
        textoVazio.fontSize = 24f;

        int limite = EstadoLivros.LimiteMochila();
        itensLivros = new RectTransform[limite];
        visuaisLivros = new VisualLivroOrganizacao[limite];

        for (int i = 0; i < limite; i++)
        {
            TMP_Text textoItem = CriarTexto("LivroMochilaVisual_" + (i + 1).ToString("00"));
            itensLivros[i] = textoItem.rectTransform;
            PosicionarNoTopo(itensLivros[i], -110f - i * 50f, new Vector2(200f, 50f));
            // Cada livro tem seu proprio texto; o cabecalho continua visivel.
            visuaisLivros[i] = new VisualLivroOrganizacao(itensLivros[i], textoItem);
            itensLivros[i].gameObject.SetActive(false);
        }
    }

    TMP_Text CriarTexto(string nome)
    {
        GameObject objeto = new GameObject(nome, typeof(RectTransform), typeof(CanvasRenderer),
            typeof(TextMeshProUGUI));
        objeto.layer = painelMochila.layer;
        objeto.transform.SetParent(painelMochila.transform, false);

        TMP_Text texto = objeto.GetComponent<TMP_Text>();
        texto.font = textoMochila.font;
        texto.fontSharedMaterial = textoMochila.fontSharedMaterial;
        texto.color = Color.white;
        texto.alignment = TextAlignmentOptions.Center;
        texto.raycastTarget = false;
        return texto;
    }

    static void PosicionarNoTopo(RectTransform retangulo, float y, Vector2 tamanho)
    {
        retangulo.anchorMin = new Vector2(0.5f, 1f);
        retangulo.anchorMax = new Vector2(0.5f, 1f);
        retangulo.pivot = new Vector2(0.5f, 0.5f);
        retangulo.anchoredPosition = new Vector2(0f, y);
        retangulo.sizeDelta = tamanho;
    }

    void AbrirMochila()
    {
        InicializarVisualizacao();
        mochilaAberta = true;
        movimentoRaposa.PararMovimento();
        movimentoRaposa.enabled = false;
        AtualizarTexto(true);
        painelMochila.SetActive(true);
    }

    void FecharMochila()
    {
        mochilaAberta = false;
        painelMochila.SetActive(false);
        movimentoRaposa.enabled = true;
    }

    void AtualizarTexto(bool forcar = false)
    {
        List<string> livros = EstadoLivros.PegarLivrosDaMochila();
        livros.Sort(StringComparer.Ordinal);

        bool mudou = forcar || livros.Count != livrosExibidos.Count;
        for (int i = 0; !mudou && i < livros.Count; i++)
        {
            mudou = livros[i] != livrosExibidos[i];
        }
        if (!mudou) return;

        livrosExibidos.Clear();
        livrosExibidos.AddRange(livros);
        textoMochila.text = "MOCHILA - " + livros.Count + "/" + EstadoLivros.LimiteMochila();
        textoVazio.gameObject.SetActive(livros.Count == 0);

        for (int i = 0; i < itensLivros.Length; i++)
        {
            bool temLivro = i < livros.Count;
            itensLivros[i].gameObject.SetActive(temLivro);
            if (temLivro)
            {
                Sprite sprite = organizacaoLivros != null ? organizacaoLivros.PegarSprite(livros[i]) : null;
                visuaisLivros[i].Mostrar(livros[i], sprite, new Vector2(192f, 44f));
            }
        }
    }
}
