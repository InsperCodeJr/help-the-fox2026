using TMPro;
using UnityEngine;

public class LivroOrganizacao : MonoBehaviour
{
    public int indiceLivro;

    private InterfaceOrganizacao interfaceOrganizacao;
    private RectTransform retangulo;
    private TMP_Text texto;
    private VisualLivroOrganizacao visual;
    private Camera cameraInterface;

    public void Inicializar(InterfaceOrganizacao organizacao)
    {
        if (visual != null)
        {
            return;
        }

        interfaceOrganizacao = organizacao;
        retangulo = GetComponent<RectTransform>();
        texto = GetComponent<TMP_Text>();
        visual = new VisualLivroOrganizacao(retangulo, texto);
        Canvas canvas = GetComponentInParent<Canvas>().rootCanvas;
        cameraInterface = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
    }

    void Update()
    {
        if (retangulo != null && Input.GetMouseButtonDown(0)
            && RectTransformUtility.RectangleContainsScreenPoint(
                retangulo, Input.mousePosition, cameraInterface))
        {
            interfaceOrganizacao.SelecionarLivro(indiceLivro);
        }
    }

    public void ExibirLivro(string identificador, Sprite sprite)
    {
        texto.text = identificador;
        visual.Mostrar(identificador, sprite, retangulo.rect.size - new Vector2(8f, 6f));
    }

    public void MostrarSelecionado(bool selecionado)
    {
        visual?.MostrarSelecionado(selecionado);
    }
}
