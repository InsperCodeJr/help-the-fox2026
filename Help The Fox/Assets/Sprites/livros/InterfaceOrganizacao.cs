using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InterfaceOrganizacao : MonoBehaviour
{
    public TMP_Text[] textosLivros;
    public PosicaoOrganizacao[] posicoes;

    [Header("Sprites das prateleiras, na ordem dos livros")]
    public Sprite[] spritesEstante06;
    public Sprite[] spritesEstante11;

    [Header("Ordem correta dos livros")]
    public string[] ordemCorreta = new string[5];

    [Header("Conclusao")]
    public GameObject mensagemConclusao;

    private readonly List<string> livrosAtuais = new List<string>();
    private readonly Dictionary<string, bool> tiposPrateleiras = new Dictionary<string, bool>();
    private bool tiposPrateleirasInicializados;
    private string[] livrosNasPosicoes;
    private string livroSelecionado = "";
    private bool puzzleConcluido;
    private bool atualizarInterface = true;
    private AreaOrganizacao areaOrganizacao;
    private InterfaceMochila interfaceMochila;

    private bool InteracaoBloqueada => interfaceMochila != null
        && (interfaceMochila.EstaAberta || interfaceMochila.EstaLendo);

    void Awake()
    {
        interfaceMochila = GetComponentInParent<InterfaceMochila>();
        livrosNasPosicoes = new string[posicoes.Length];

        PrepararTiposPrateleiras();

        foreach (AreaOrganizacao area in FindObjectsByType<AreaOrganizacao>(
            FindObjectsInactive.Include))
        {
            if (area.painelOrganizacao == gameObject)
            {
                areaOrganizacao = area;
                break;
            }
        }

        foreach (TMP_Text texto in textosLivros)
        {
            texto.GetComponent<LivroOrganizacao>().Inicializar(this);
        }

        foreach (PosicaoOrganizacao posicao in posicoes)
        {
            posicao.Inicializar(this);
        }

        if (mensagemConclusao != null)
        {
            mensagemConclusao.SetActive(false);
        }
    }

    void OnEnable()
    {
        atualizarInterface = true;
        AtualizarLivros();
        AtualizarPosicoes();
    }

    void Update()
    {
        AtualizarLivros();
    }

    void AtualizarLivros()
    {
        List<string> mochila = EstadoLivros.PegarLivrosDaMochila();
        mochila.Sort(StringComparer.Ordinal);
        mochila.RemoveAll(LivroJaEstaOrganizado);

        bool mudou = atualizarInterface || mochila.Count != livrosAtuais.Count;
        for (int i = 0; !mudou && i < mochila.Count; i++)
        {
            mudou = mochila[i] != livrosAtuais[i];
        }

        if (!mudou)
        {
            return;
        }

        atualizarInterface = false;
        livrosAtuais.Clear();
        livrosAtuais.AddRange(mochila);
        if (!livrosAtuais.Contains(livroSelecionado))
        {
            livroSelecionado = "";
        }

        for (int i = 0; i < textosLivros.Length; i++)
        {
            if (i < livrosAtuais.Count)
            {
                textosLivros[i].gameObject.SetActive(true);
                textosLivros[i].GetComponent<LivroOrganizacao>().ExibirLivro(
                    livrosAtuais[i], PegarSprite(livrosAtuais[i]));
            }
            else
            {
                textosLivros[i].gameObject.SetActive(false);
            }
        }

        AtualizarDestaque();
    }

    bool LivroJaEstaOrganizado(string idLivro)
    {
        return Array.IndexOf(livrosNasPosicoes, idLivro) >= 0;
    }

    void PrepararTiposPrateleiras()
    {
        if (tiposPrateleirasInicializados) return;

        foreach (PrateleiraInterativa prateleira in FindObjectsByType<PrateleiraInterativa>(
            FindObjectsInactive.Include))
        {
            if (!string.IsNullOrEmpty(prateleira.idPrateleira))
            {
                tiposPrateleiras[prateleira.idPrateleira] = prateleira.usarEstante10;
            }
        }

        tiposPrateleirasInicializados = true;
    }

    public Sprite PegarSprite(string idLivro)
    {
        if (string.IsNullOrEmpty(idLivro)) return null;

        PrepararTiposPrateleiras();
        int separador = idLivro.LastIndexOf("_L", StringComparison.Ordinal);
        if (separador < 0 || !int.TryParse(idLivro.Substring(separador + 2), out int numero))
        {
            return null;
        }

        string prateleira = idLivro.Substring(0, separador);
        bool estanteGrande = tiposPrateleiras.TryGetValue(prateleira, out bool grande) && grande;
        Sprite[] sprites = estanteGrande ? spritesEstante11 : spritesEstante06;
        return sprites != null && numero > 0 && numero <= sprites.Length ? sprites[numero - 1] : null;
    }

    void AtualizarPosicoes()
    {
        for (int i = 0; i < posicoes.Length; i++)
        {
            string idLivro = livrosNasPosicoes[i];
            posicoes[i].ExibirLivro(idLivro, string.IsNullOrEmpty(idLivro) ? null : PegarSprite(idLivro));
        }
    }

    public void SelecionarLivro(int indice)
    {
        if (InteracaoBloqueada || puzzleConcluido || indice < 0 || indice >= livrosAtuais.Count)
        {
            return;
        }

        livroSelecionado = livrosAtuais[indice];
        AtualizarDestaque();
    }

    void AtualizarDestaque()
    {
        for (int i = 0; i < textosLivros.Length; i++)
        {
            textosLivros[i].GetComponent<LivroOrganizacao>().MostrarSelecionado(
                i < livrosAtuais.Count && livrosAtuais[i] == livroSelecionado);
        }
    }

    public void ColocarLivroNaPosicao(int numeroPosicao)
    {
        int indice = numeroPosicao - 1;
        if (InteracaoBloqueada || puzzleConcluido || indice < 0 || indice >= livrosNasPosicoes.Length)
        {
            return;
        }

        // Clicar em uma posicao ocupada devolve o livro a lista da mochila.
        if (!string.IsNullOrEmpty(livrosNasPosicoes[indice]))
        {
            livrosNasPosicoes[indice] = null;
            livroSelecionado = "";
            AtualizarOrganizacao();
            return;
        }

        if (string.IsNullOrEmpty(livroSelecionado))
        {
            return;
        }

        livrosNasPosicoes[indice] = livroSelecionado;
        livroSelecionado = "";
        AtualizarOrganizacao();
        VerificarVitoria();
    }

    void AtualizarOrganizacao()
    {
        atualizarInterface = true;
        AtualizarLivros();
        AtualizarPosicoes();
    }

    public string PegarLivroSelecionado()
    {
        return livroSelecionado;
    }

    void VerificarVitoria()
    {
        if (ordemCorreta.Length != livrosNasPosicoes.Length)
        {
            return;
        }

        for (int i = 0; i < livrosNasPosicoes.Length; i++)
        {
            if (string.IsNullOrEmpty(livrosNasPosicoes[i]) || livrosNasPosicoes[i] != ordemCorreta[i])
            {
                return;
            }
        }

        puzzleConcluido = true;
        if (mensagemConclusao != null)
        {
            mensagemConclusao.SetActive(true);
        }

        if (areaOrganizacao != null)
        {
            areaOrganizacao.FecharPainel();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
