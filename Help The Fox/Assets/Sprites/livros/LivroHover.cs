using UnityEngine;
using UnityEngine.UI;

public class LivroHover : MonoBehaviour
{
    public int numeroLivro;

    [Header("Espaco vazio")]
    public GameObject espacoVazio;

    private RectTransform rectTransform;
    private Image imagem;

    private bool mouseEmCima = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        imagem = GetComponent<Image>();

        imagem.color = new Color(1f, 1f, 1f, 0f);

        if (espacoVazio != null)
        {
            espacoVazio.SetActive(false);
        }
    }

    void Update()
    {
        // Se nenhuma prateleira estiver aberta
        if (string.IsNullOrEmpty(PrateleiraInterativa.PrateleiraAtual))
        {
            imagem.color = new Color(1f, 1f, 1f, 0f);

            if (espacoVazio != null)
            {
                espacoVazio.SetActive(false);
            }

            return;
        }

        string idLivro =
            PrateleiraInterativa.PrateleiraAtual
            + "_L"
            + numeroLivro.ToString("00");

        // Se esse livro já foi retirado dessa prateleira
        if (EstadoLivros.LivroFoiRetirado(idLivro))
        {
            imagem.color = new Color(1f, 1f, 1f, 0f);

            if (espacoVazio != null)
            {
                espacoVazio.SetActive(true);
            }

            return;
        }

        // Livro ainda está na prateleira
        if (espacoVazio != null)
        {
            espacoVazio.SetActive(false);
        }

        Vector2 posicaoMouse = Input.mousePosition;

        mouseEmCima = RectTransformUtility.RectangleContainsScreenPoint(
            rectTransform,
            posicaoMouse
        );

        if (mouseEmCima)
        {
            imagem.color = new Color(
                1f,
                1f,
                0.75f,
                1f
            );

            if (Input.GetMouseButtonDown(0))
            {
                EstadoLivros.RetirarLivro(idLivro);

                imagem.color = new Color(
                    1f,
                    1f,
                    1f,
                    0f
                );

                if (espacoVazio != null)
                {
                    espacoVazio.SetActive(true);
                }
            }
        }
        else
        {
            imagem.color = new Color(
                1f,
                1f,
                1f,
                0f
            );
        }
    }
}