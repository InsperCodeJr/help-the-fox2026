using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PosicaoOrganizacao : MonoBehaviour
{
    public int numeroPosicao;
    public TMP_Text textoPosicao;

    private InterfaceOrganizacao interfaceOrganizacao;
    private RectTransform retangulo;
    private Image fundo;
    private VisualLivroOrganizacao visual;
    private Color corVazia;
    private Camera cameraInterface;

    public void Inicializar(InterfaceOrganizacao organizacao)
    {
        if (visual != null)
        {
            return;
        }

        interfaceOrganizacao = organizacao;
        retangulo = GetComponent<RectTransform>();
        fundo = GetComponent<Image>();
        corVazia = fundo.color;
        visual = new VisualLivroOrganizacao(retangulo, textoPosicao);
        Canvas canvas = GetComponentInParent<Canvas>().rootCanvas;
        cameraInterface = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
    }

    void Update()
    {
        if (retangulo != null && Input.GetMouseButtonDown(0)
            && RectTransformUtility.RectangleContainsScreenPoint(
                retangulo, Input.mousePosition, cameraInterface))
        {
            interfaceOrganizacao.ColocarLivroNaPosicao(numeroPosicao);
        }
    }

    public void ExibirLivro(string identificador, Sprite sprite)
    {
        bool vazio = string.IsNullOrEmpty(identificador);
        if (vazio)
        {
            visual.Ocultar();
            textoPosicao.text = numeroPosicao.ToString();
        }
        else
        {
            textoPosicao.text = identificador;
            visual.Mostrar(identificador, sprite, retangulo.rect.size - new Vector2(12f, 8f), 24f);
        }

        fundo.color = vazio ? corVazia : Color.clear;
    }
}
