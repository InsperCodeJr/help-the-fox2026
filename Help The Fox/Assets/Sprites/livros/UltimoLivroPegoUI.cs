using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class UltimoLivroPegoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text fonteModelo;
    [SerializeField] private RectTransform[] estantes = new RectTransform[0];

    private const float LarguraColuna = 260f;
    private const float Margem = 24f;

    private RectTransform painelLivros;
    private RectTransform painelLateral;
    private TMP_Text textoNomeLivro;
    private Vector3[] escalasOriginais;
    private Bounds[] limitesEstantes;
    private Vector2 ultimoTamanho = new Vector2(-1f, -1f);
    private string ultimoNomeExibido;

    public TMP_Text TextoNomeLivro => textoNomeLivro;
    public RectTransform PainelLateral => painelLateral;

    void OnEnable()
    {
        CriarInterface();
        ultimoTamanho = new Vector2(-1f, -1f);
        AjustarLayout();
        AtualizarNome(true);
    }

    void LateUpdate()
    {
        // O rect do Canvas e a coleta pelo mouse ja foram atualizados neste frame.
        AjustarLayout();
        AtualizarNome();
    }

    void CriarInterface()
    {
        if (painelLateral != null) return;

        painelLivros = (RectTransform)transform;
        escalasOriginais = new Vector3[estantes.Length];
        limitesEstantes = new Bounds[estantes.Length];
        for (int i = 0; i < estantes.Length; i++)
        {
            if (estantes[i] == null) continue;
            escalasOriginais[i] = estantes[i].localScale;
            limitesEstantes[i] = CalcularLimites(estantes[i]);
        }

        GameObject objeto = new GameObject("PainelUltimoLivroPego", typeof(RectTransform),
            typeof(CanvasRenderer), typeof(Image));
        objeto.layer = gameObject.layer;
        painelLateral = objeto.GetComponent<RectTransform>();
        painelLateral.SetParent(transform, false);
        painelLateral.anchorMin = new Vector2(1f, 0.5f);
        painelLateral.anchorMax = new Vector2(1f, 0.5f);
        painelLateral.pivot = new Vector2(1f, 0.5f);
        painelLateral.anchoredPosition = new Vector2(-Margem, 0f);
        painelLateral.sizeDelta = new Vector2(LarguraColuna, 150f);

        Image fundo = objeto.GetComponent<Image>();
        fundo.color = new Color(0.12f, 0.09f, 0.06f, 0.88f);
        fundo.raycastTarget = false;

        TMP_Text titulo = CriarTexto("TituloUltimoLivro", -16f, 34f, 24f);
        titulo.text = "Último livro pego:";
        textoNomeLivro = CriarTexto("NomeUltimoLivro", -62f, 64f, 28f);
    }

    TMP_Text CriarTexto(string nome, float y, float altura, float tamanhoFonte)
    {
        GameObject objeto = new GameObject(nome, typeof(RectTransform),
            typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        objeto.layer = gameObject.layer;
        TMP_Text texto = objeto.GetComponent<TMP_Text>();
        RectTransform retangulo = texto.rectTransform;
        retangulo.SetParent(painelLateral, false);
        retangulo.anchorMin = new Vector2(0f, 1f);
        retangulo.anchorMax = new Vector2(0f, 1f);
        retangulo.pivot = new Vector2(0f, 1f);
        retangulo.anchoredPosition = new Vector2(16f, y);
        retangulo.sizeDelta = new Vector2(LarguraColuna - 32f, altura);

        if (fonteModelo != null)
        {
            texto.font = fonteModelo.font;
            texto.fontSharedMaterial = fonteModelo.fontSharedMaterial;
        }
        texto.color = Color.white;
        texto.alignment = TextAlignmentOptions.Left;
        texto.fontSize = tamanhoFonte;
        texto.enableAutoSizing = true;
        texto.fontSizeMin = 16f;
        texto.fontSizeMax = tamanhoFonte;
        texto.textWrappingMode = TextWrappingModes.NoWrap;
        texto.overflowMode = TextOverflowModes.Ellipsis;
        texto.raycastTarget = false;
        return texto;
    }

    void AtualizarNome(bool forcar = false)
    {
        string nome = EstadoLivros.PegarUltimoLivroPego();
        if (!forcar && nome == ultimoNomeExibido) return;

        ultimoNomeExibido = nome;
        textoNomeLivro.text = string.IsNullOrEmpty(nome) ? "Nenhum livro pego" : nome;
    }

    void AjustarLayout()
    {
        Vector2 tamanho = painelLivros.rect.size;
        if (tamanho.x <= 0f || tamanho.y <= 0f || tamanho == ultimoTamanho) return;
        ultimoTamanho = tamanho;

        float larguraDisponivel = Mathf.Max(1f, tamanho.x - LarguraColuna - 3f * Margem);
        float alturaDisponivel = Mathf.Max(1f, tamanho.y - 2f * Margem);
        float centroEsquerda = painelLivros.rect.xMin + Margem + larguraDisponivel * 0.5f;

        for (int i = 0; i < estantes.Length; i++)
        {
            RectTransform estante = estantes[i];
            if (estante == null) continue;

            Bounds limites = limitesEstantes[i];
            Vector3 escala = escalasOriginais[i];
            float larguraOriginal = limites.size.x * Mathf.Abs(escala.x);
            float alturaOriginal = limites.size.y * Mathf.Abs(escala.y);
            float fator = Mathf.Min(1f, larguraDisponivel / Mathf.Max(1f, larguraOriginal),
                alturaDisponivel / Mathf.Max(1f, alturaOriginal));
            escala.x *= fator;
            escala.y *= fator;

            estante.anchorMin = new Vector2(0.5f, 0.5f);
            estante.anchorMax = new Vector2(0.5f, 0.5f);
            estante.localScale = escala;
            estante.anchoredPosition = new Vector2(
                centroEsquerda - painelLivros.rect.center.x - limites.center.x * escala.x,
                -limites.center.y * escala.y);
        }
    }

    static Bounds CalcularLimites(RectTransform estante)
    {
        Bounds limites = new Bounds();
        bool primeiroCanto = true;
        Vector3[] cantos = new Vector3[4];
        // Estante_06 tem raiz de 100x100, mas a imagem filha tem cerca de 679x377.
        foreach (RectTransform filho in estante.GetComponentsInChildren<RectTransform>(true))
        {
            filho.GetLocalCorners(cantos);
            foreach (Vector3 canto in cantos)
            {
                Vector3 local = estante.InverseTransformPoint(filho.TransformPoint(canto));
                if (primeiroCanto)
                {
                    limites = new Bounds(local, Vector3.zero);
                    primeiroCanto = false;
                }
                else limites.Encapsulate(local);
            }
        }
        return limites;
    }
}
