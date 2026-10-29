using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceMochila : MonoBehaviour
{
    public GameObject painelMochila;
    public TMP_Text textoMochila;
    public MovimentoRaposa movimentoRaposa;

    [Header("Visual dos livros")]
    [SerializeField] private InterfaceOrganizacao organizacaoLivros;
    [SerializeField] private bool mostrarLivros = true;

    private bool mochilaAberta;
    private bool retomarMovimento;
    private RectTransform[] itensLivros;
    private VisualLivroOrganizacao[] visuaisLivros;
    private TMP_Text textoVazio;
    private TMP_Text textoResumo;
    private TMP_Text instrucaoPapeis;
    private readonly List<string> livrosExibidos = new List<string>();
    private readonly List<PapelInventario> papeisExibidos = new List<PapelInventario>();
    private readonly List<RectTransform> itensPapeis = new List<RectTransform>();
    private readonly List<Image> imagensPapeis = new List<Image>();
    private readonly List<TMP_Text> nomesPapeis = new List<TMP_Text>();

    private RectTransform painelLeitura;
    private Image imagemLeitura;
    private TMP_Text textoLeitura;
    private int quadroAberturaLeitura;
    private Vector2 tamanhoLeituraAnterior = new Vector2(-1f, -1f);

    public bool EstaAberta => mochilaAberta;
    public bool EstaLendo => painelLeitura != null && painelLeitura.gameObject.activeSelf;
    public RectTransform PainelLeitura => painelLeitura;
    public Image ImagemLeitura => imagemLeitura;
    public TMP_Text TextoLeitura => textoLeitura;
    public IReadOnlyList<RectTransform> ItensPapeis => itensPapeis;

    void Start()
    {
        InicializarVisualizacao();
        painelMochila.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (EstaLendo) FecharLeitura();
            else if (mochilaAberta) FecharMochila();
            else AbrirMochila();
        }
        else if (EstaLendo && Input.GetKeyDown(KeyCode.Escape))
        {
            FecharLeitura();
        }

        if (mochilaAberta)
        {
            AtualizarTexto();
            AjustarTamanhoLeitura();
            ProcessarClique(Input.mousePosition, Input.GetMouseButtonDown(0), Time.frameCount);
        }
    }

    void LateUpdate()
    {
        if (mochilaAberta && movimentoRaposa != null && movimentoRaposa.enabled)
        {
            movimentoRaposa.PararMovimento();
            movimentoRaposa.enabled = false;
        }
    }

    void OnDisable()
    {
        if (painelLeitura != null) painelLeitura.gameObject.SetActive(false);
        if (painelMochila != null) painelMochila.SetActive(false);
        if (mochilaAberta && movimentoRaposa != null) movimentoRaposa.enabled = retomarMovimento;
        mochilaAberta = false;
    }

    void InicializarVisualizacao()
    {
        if (itensLivros != null) return;

        if (organizacaoLivros == null)
        {
            organizacaoLivros = GetComponentInChildren<InterfaceOrganizacao>(true);
        }

        PosicionarNoTopo(textoMochila.rectTransform, 0f, -45f, new Vector2(520f, 50f));
        textoMochila.alignment = TextAlignmentOptions.Center;
        textoMochila.textWrappingMode = TextWrappingModes.NoWrap;
        textoMochila.enableAutoSizing = true;
        textoMochila.fontSizeMin = 18f;
        textoMochila.fontSizeMax = 36f;

        textoVazio = CriarTexto("MochilaVazia", painelMochila.transform, 24f);
        PosicionarNoTopo(textoVazio.rectTransform, 0f, -110f, new Vector2(320f, 50f));

        textoResumo = CriarTexto("ResumoMochila", painelMochila.transform, 21f);
        PosicionarNoTopo(textoResumo.rectTransform, 0f, -83f, new Vector2(520f, 30f));
        textoResumo.gameObject.SetActive(false);

        instrucaoPapeis = CriarTexto("InstrucaoPapeis", painelMochila.transform, 19f);
        instrucaoPapeis.rectTransform.anchorMin = new Vector2(0.5f, 0f);
        instrucaoPapeis.rectTransform.anchorMax = new Vector2(0.5f, 0f);
        instrucaoPapeis.rectTransform.anchoredPosition = new Vector2(0f, 30f);
        instrucaoPapeis.rectTransform.sizeDelta = new Vector2(520f, 35f);
        instrucaoPapeis.text = "Clique no papel para ler";
        instrucaoPapeis.gameObject.SetActive(false);

        int limite = EstadoLivros.LimiteMochila();
        itensLivros = new RectTransform[limite];
        visuaisLivros = new VisualLivroOrganizacao[limite];

        for (int i = 0; i < limite; i++)
        {
            TMP_Text textoItem = CriarTexto("LivroMochilaVisual_" + (i + 1).ToString("00"),
                painelMochila.transform, 20f);
            itensLivros[i] = textoItem.rectTransform;
            PosicionarNoTopo(itensLivros[i], 0f, -110f - i * 50f, new Vector2(200f, 50f));
            visuaisLivros[i] = new VisualLivroOrganizacao(itensLivros[i], textoItem);
            itensLivros[i].gameObject.SetActive(false);
        }
    }

    TMP_Text CriarTexto(string nome, Transform pai, float tamanhoFonte)
    {
        GameObject objeto = new GameObject(nome, typeof(RectTransform), typeof(CanvasRenderer),
            typeof(TextMeshProUGUI));
        objeto.layer = painelMochila.layer;
        objeto.transform.SetParent(pai, false);

        TMP_Text texto = objeto.GetComponent<TMP_Text>();
        texto.font = textoMochila.font;
        texto.fontSharedMaterial = textoMochila.fontSharedMaterial;
        texto.color = Color.white;
        texto.alignment = TextAlignmentOptions.Center;
        texto.fontSize = tamanhoFonte;
        texto.raycastTarget = false;
        return texto;
    }

    static void PosicionarNoTopo(RectTransform retangulo, float x, float y, Vector2 tamanho)
    {
        retangulo.anchorMin = new Vector2(0.5f, 1f);
        retangulo.anchorMax = new Vector2(0.5f, 1f);
        retangulo.pivot = new Vector2(0.5f, 0.5f);
        retangulo.anchoredPosition = new Vector2(x, y);
        retangulo.sizeDelta = tamanho;
    }

    void AbrirMochila()
    {
        InicializarVisualizacao();
        if (!mochilaAberta)
        {
            retomarMovimento = movimentoRaposa != null && movimentoRaposa.enabled;
        }
        mochilaAberta = true;
        if (movimentoRaposa != null)
        {
            movimentoRaposa.PararMovimento();
            movimentoRaposa.enabled = false;
        }
        AtualizarTexto(true);
        painelMochila.transform.SetAsLastSibling();
        painelMochila.SetActive(true);
    }

    void FecharMochila()
    {
        mochilaAberta = false;
        FecharLeitura();
        painelMochila.SetActive(false);
        if (movimentoRaposa != null) movimentoRaposa.enabled = retomarMovimento;
    }

    void AtualizarTexto(bool forcar = false)
    {
        List<string> livros = mostrarLivros
            ? EstadoLivros.PegarLivrosDaMochila() : new List<string>();
        livros.Sort(StringComparer.Ordinal);
        List<PapelInventario> papeis = EstadoPapeis.PegarPapeisDaMochila();

        bool mudou = forcar || livros.Count != livrosExibidos.Count || papeis.Count != papeisExibidos.Count;
        for (int i = 0; !mudou && i < livros.Count; i++)
        {
            mudou = livros[i] != livrosExibidos[i];
        }
        for (int i = 0; !mudou && i < papeis.Count; i++)
        {
            mudou = papeis[i].Id != papeisExibidos[i].Id;
        }
        if (!mudou) return;

        livrosExibidos.Clear();
        livrosExibidos.AddRange(livros);
        papeisExibidos.Clear();
        papeisExibidos.AddRange(papeis);

        bool temPapeis = papeis.Count > 0;
        textoMochila.text = mostrarLivros && !temPapeis
            ? "MOCHILA - " + livros.Count + "/" + EstadoLivros.LimiteMochila()
            : "MOCHILA";
        textoResumo.gameObject.SetActive(temPapeis);
        textoResumo.text = (mostrarLivros ? livros.Count + "/7 livros · " : "")
            + papeis.Count + (papeis.Count == 1 ? " papel" : " papéis");
        textoVazio.text = mostrarLivros ? "Nenhum livro" : "Mochila vazia";
        textoVazio.gameObject.SetActive(livros.Count == 0 && !temPapeis);
        instrucaoPapeis.gameObject.SetActive(temPapeis);

        bool duasColunas = livros.Count > 0 && temPapeis;
        instrucaoPapeis.rectTransform.anchoredPosition = new Vector2(duasColunas ? 190f : 0f, 30f);
        instrucaoPapeis.rectTransform.sizeDelta = new Vector2(duasColunas ? 300f : 520f, 35f);
        for (int i = 0; i < itensLivros.Length; i++)
        {
            bool temLivro = i < livros.Count;
            itensLivros[i].gameObject.SetActive(temLivro);
            PosicionarNoTopo(itensLivros[i], duasColunas ? -190f : 0f,
                (temPapeis ? -130f : -110f) - i * 50f, new Vector2(200f, 50f));
            if (temLivro)
            {
                Sprite sprite = organizacaoLivros != null ? organizacaoLivros.PegarSprite(livros[i]) : null;
                visuaisLivros[i].Mostrar(livros[i], sprite, new Vector2(192f, 44f));
            }
        }

        while (itensPapeis.Count < papeis.Count) CriarItemPapel();
        for (int i = 0; i < itensPapeis.Count; i++)
        {
            bool ativo = i < papeis.Count;
            itensPapeis[i].gameObject.SetActive(ativo);
            if (!ativo) continue;

            float x = duasColunas ? 190f : (i % 3 - Mathf.Min(2, papeis.Count - 1) * 0.5f) * 200f;
            int linha = duasColunas ? i : i / 3;
            PosicionarNoTopo(itensPapeis[i], x, -205f - linha * 190f, new Vector2(160f, 190f));
            imagensPapeis[i].sprite = papeis[i].SpriteMochila;
            nomesPapeis[i].text = papeis[i].Nome;
        }
    }

    void CriarItemPapel()
    {
        GameObject objeto = new GameObject("PapelMochila_" + (itensPapeis.Count + 1).ToString("00"),
            typeof(RectTransform));
        objeto.layer = painelMochila.layer;
        RectTransform item = (RectTransform)objeto.transform;
        item.SetParent(painelMochila.transform, false);

        GameObject objetoImagem = new GameObject("SpritePapel", typeof(RectTransform),
            typeof(CanvasRenderer), typeof(Image));
        objetoImagem.layer = painelMochila.layer;
        Image imagem = objetoImagem.GetComponent<Image>();
        imagem.rectTransform.SetParent(item, false);
        imagem.rectTransform.anchorMin = new Vector2(0.5f, 1f);
        imagem.rectTransform.anchorMax = new Vector2(0.5f, 1f);
        imagem.rectTransform.pivot = new Vector2(0.5f, 1f);
        imagem.rectTransform.anchoredPosition = Vector2.zero;
        imagem.rectTransform.sizeDelta = new Vector2(136f, 136f);
        imagem.preserveAspect = true;
        imagem.raycastTarget = false;

        TMP_Text nome = CriarTexto("NomePapel", item, 23f);
        nome.rectTransform.anchorMin = new Vector2(0.5f, 0f);
        nome.rectTransform.anchorMax = new Vector2(0.5f, 0f);
        nome.rectTransform.pivot = new Vector2(0.5f, 0f);
        nome.rectTransform.anchoredPosition = new Vector2(0f, 12f);
        nome.rectTransform.sizeDelta = new Vector2(160f, 34f);

        itensPapeis.Add(item);
        imagensPapeis.Add(imagem);
        nomesPapeis.Add(nome);
    }

    void ProcessarClique(Vector2 posicaoMouse, bool clicou, int quadro)
    {
        if (!mochilaAberta || !clicou) return;

        if (EstaLendo)
        {
            if (quadro > quadroAberturaLeitura) FecharLeitura();
            return;
        }

        Canvas canvas = GetComponentInParent<Canvas>().rootCanvas;
        Camera cameraInterface = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        for (int i = 0; i < papeisExibidos.Count; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(itensPapeis[i], posicaoMouse, cameraInterface))
            {
                AbrirLeitura(papeisExibidos[i]);
                quadroAberturaLeitura = quadro;
                return;
            }
        }
    }

    public void AbrirLeitura(PapelInventario papel)
    {
        if (papel == null || !EstadoPapeis.Possui(papel.Id) || papel.SpriteLeitura == null) return;
        if (!mochilaAberta) AbrirMochila();
        CriarPainelLeitura();
        imagemLeitura.sprite = papel.SpriteLeitura;
        textoLeitura.text = papel.Texto;
        quadroAberturaLeitura = Time.frameCount;
        painelLeitura.SetAsLastSibling();
        painelLeitura.gameObject.SetActive(true);
        painelMochila.SetActive(false);
        AjustarTamanhoLeitura();
    }

    public void FecharLeitura()
    {
        bool estavaLendo = EstaLendo;
        if (painelLeitura != null) painelLeitura.gameObject.SetActive(false);
        if (estavaLendo && mochilaAberta) painelMochila.SetActive(true);
    }

    void CriarPainelLeitura()
    {
        if (painelLeitura != null) return;

        Canvas canvas = GetComponentInParent<Canvas>().rootCanvas;
        GameObject objeto = new GameObject("PainelLeituraPapel", typeof(RectTransform),
            typeof(CanvasRenderer), typeof(Image));
        objeto.layer = painelMochila.layer;
        painelLeitura = (RectTransform)objeto.transform;
        painelLeitura.SetParent(canvas.transform, false);
        painelLeitura.anchorMin = Vector2.zero;
        painelLeitura.anchorMax = Vector2.one;
        painelLeitura.offsetMin = Vector2.zero;
        painelLeitura.offsetMax = Vector2.zero;
        objeto.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.82f);

        GameObject folha = new GameObject("PapelLeitura", typeof(RectTransform),
            typeof(CanvasRenderer), typeof(Image));
        folha.layer = painelMochila.layer;
        imagemLeitura = folha.GetComponent<Image>();
        imagemLeitura.rectTransform.SetParent(painelLeitura, false);
        imagemLeitura.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        imagemLeitura.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        imagemLeitura.rectTransform.anchoredPosition = Vector2.zero;
        imagemLeitura.preserveAspect = true;
        imagemLeitura.raycastTarget = false;

        textoLeitura = CriarTexto("TextoLeitura", folha.transform, 30f);
        textoLeitura.rectTransform.anchorMin = new Vector2(0.2f, 0.18f);
        textoLeitura.rectTransform.anchorMax = new Vector2(0.8f, 0.8f);
        textoLeitura.rectTransform.offsetMin = new Vector2(10f, 10f);
        textoLeitura.rectTransform.offsetMax = new Vector2(-10f, -10f);
        textoLeitura.color = new Color(0.2f, 0.12f, 0.06f, 1f);
        textoLeitura.alignment = TextAlignmentOptions.TopLeft;
        textoLeitura.textWrappingMode = TextWrappingModes.Normal;
        textoLeitura.enableAutoSizing = true;
        textoLeitura.fontSizeMin = 16f;
        textoLeitura.fontSizeMax = 30f;
        textoLeitura.overflowMode = TextOverflowModes.Ellipsis;

        TMP_Text instrucao = CriarTexto("InstrucaoFecharLeitura", painelLeitura, 20f);
        instrucao.rectTransform.anchorMin = new Vector2(0.5f, 0f);
        instrucao.rectTransform.anchorMax = new Vector2(0.5f, 0f);
        instrucao.rectTransform.anchoredPosition = new Vector2(0f, 28f);
        instrucao.rectTransform.sizeDelta = new Vector2(620f, 36f);
        instrucao.text = "Clique em qualquer lugar para fechar";
        painelLeitura.gameObject.SetActive(false);
    }

    void AjustarTamanhoLeitura()
    {
        if (painelLeitura == null || !EstaLendo) return;
        Vector2 tamanho = painelLeitura.rect.size;
        if (tamanho == tamanhoLeituraAnterior) return;
        tamanhoLeituraAnterior = tamanho;
        float lado = Mathf.Max(1f, Mathf.Min(tamanho.x - 64f, tamanho.y - 100f));
        imagemLeitura.rectTransform.sizeDelta = new Vector2(lado, lado);
    }
}
