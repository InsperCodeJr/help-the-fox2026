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

        if (EstadoLivros.LivroFoiRetirado(idLivro))
        {
            imagem.color = new Color(1f, 1f, 1f, 0f);

            if (espacoVazio != null)
            {
                espacoVazio.SetActive(true);
            }

            return;
        }

        if (espacoVazio != null)
        {
            espacoVazio.SetActive(false);
        }

        Vector2 posicaoMouse = Input.mousePosition;

        mouseEmCima =
            RectTransformUtility.RectangleContainsScreenPoint(
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
                bool conseguiuPegar =
                    EstadoLivros.RetirarLivro(idLivro);

                if (conseguiuPegar)
                {
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