using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Apresenta o mesmo livro deitado na mochila e na bancada.
/// </summary>
public sealed class VisualLivroOrganizacao
{
    private readonly TMP_Text textoOriginal;
    private readonly RectTransform raizVisual;
    private readonly Image imagem;
    private readonly TextMeshProUGUI nomeLivro;

    public VisualLivroOrganizacao(RectTransform raiz, TMP_Text textoOriginal)
    {
        this.textoOriginal = textoOriginal;

        GameObject objetoVisual = new GameObject(
            "VisualLivro",
            typeof(RectTransform)
        );
        objetoVisual.layer = raiz.gameObject.layer;
        raizVisual = objetoVisual.GetComponent<RectTransform>();
        raizVisual.SetParent(raiz, false);
        Centralizar(raizVisual);

        GameObject objetoImagem = new GameObject(
            "SpriteLivro",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image)
        );
        objetoImagem.layer = raiz.gameObject.layer;
        imagem = objetoImagem.GetComponent<Image>();
        imagem.rectTransform.SetParent(raizVisual, false);
        Centralizar(imagem.rectTransform);
        imagem.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        imagem.preserveAspect = true;
        imagem.raycastTarget = true;

        // O nome precisa ser um filho posterior à imagem para ficar por cima dela.
        GameObject objetoNome = new GameObject(
            "NomeLivro",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(TextMeshProUGUI)
        );
        objetoNome.layer = raiz.gameObject.layer;
        nomeLivro = objetoNome.GetComponent<TextMeshProUGUI>();
        nomeLivro.rectTransform.SetParent(raizVisual, false);
        nomeLivro.rectTransform.anchorMin = Vector2.zero;
        nomeLivro.rectTransform.anchorMax = Vector2.one;
        nomeLivro.rectTransform.offsetMin = new Vector2(4f, 1f);
        nomeLivro.rectTransform.offsetMax = new Vector2(-4f, -1f);

        if (textoOriginal != null)
        {
            nomeLivro.font = textoOriginal.font;
            nomeLivro.fontSharedMaterial = textoOriginal.fontSharedMaterial;
            nomeLivro.fontStyle = textoOriginal.fontStyle;
        }

        nomeLivro.color = Color.white;
        nomeLivro.alignment = TextAlignmentOptions.Center;
        nomeLivro.textWrappingMode = TextWrappingModes.NoWrap;
        nomeLivro.overflowMode = TextOverflowModes.Ellipsis;
        nomeLivro.enableAutoSizing = true;
        nomeLivro.raycastTarget = true;

        // Os setters do TMP criam um material próprio, preservando o material da cena.
        nomeLivro.outlineWidth = 0.2f;
        nomeLivro.outlineColor = Color.black;

        objetoVisual.SetActive(false);
    }

    public void Mostrar(string nome, Sprite sprite, Vector2 tamanho, float tamanhoFonte = 20f)
    {
        if (raizVisual.sizeDelta != tamanho)
        {
            raizVisual.sizeDelta = tamanho;
        }

        // O sprite original é vertical; invertemos os limites antes de girá-lo.
        Vector2 tamanhoVertical = new Vector2(tamanho.y, tamanho.x);
        if (imagem.rectTransform.sizeDelta != tamanhoVertical)
        {
            imagem.rectTransform.sizeDelta = tamanhoVertical;
        }
        imagem.sprite = sprite;
        imagem.enabled = sprite != null;

        nomeLivro.text = nome;
        nomeLivro.fontSize = tamanhoFonte;
        nomeLivro.fontSizeMin = Mathf.Min(12f, tamanhoFonte);
        nomeLivro.fontSizeMax = tamanhoFonte;

        if (textoOriginal != null && textoOriginal.enabled)
        {
            textoOriginal.enabled = false;
        }

        if (!raizVisual.gameObject.activeSelf)
        {
            raizVisual.gameObject.SetActive(true);
        }
    }

    public void Ocultar()
    {
        if (raizVisual.gameObject.activeSelf)
        {
            raizVisual.gameObject.SetActive(false);
        }

        if (textoOriginal != null && !textoOriginal.enabled)
        {
            textoOriginal.enabled = true;
        }
    }

    public void MostrarSelecionado(bool selecionado)
    {
        nomeLivro.color = selecionado ? new Color(1f, 1f, 0.4f, 1f) : Color.white;
    }

    private static void Centralizar(RectTransform transformacao)
    {
        transformacao.anchorMin = new Vector2(0.5f, 0.5f);
        transformacao.anchorMax = new Vector2(0.5f, 0.5f);
        transformacao.pivot = new Vector2(0.5f, 0.5f);
        transformacao.anchoredPosition = Vector2.zero;
    }
}
