using UnityEngine;
using TMPro;

public class LivroOrganizacao : MonoBehaviour
{
    public int indiceLivro;

    private RectTransform rectTransform;
    private TMP_Text texto;

    private Color corNormal;
    private Color corSelecionado;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        texto = GetComponent<TMP_Text>();

        corNormal = Color.white;
        corSelecionado = new Color(1f, 1f, 0.4f, 1f);
    }

    void Update()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        Vector2 posicaoMouse = Input.mousePosition;

        bool mouseEmCima =
            RectTransformUtility.RectangleContainsScreenPoint(
                rectTransform,
                posicaoMouse
            );

        if (mouseEmCima && Input.GetMouseButtonDown(0))
        {
            InterfaceOrganizacao interfaceOrganizacao =
                GetComponentInParent<InterfaceOrganizacao>();

            interfaceOrganizacao.SelecionarLivro(indiceLivro);
        }
    }

    public void MostrarSelecionado(bool selecionado)
    {
        if (selecionado)
        {
            texto.color = corSelecionado;
        }
        else
        {
            texto.color = corNormal;
        }
    }
}
