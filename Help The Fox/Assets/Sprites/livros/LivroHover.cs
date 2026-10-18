using UnityEngine;
using UnityEngine.UI;

public class LivroHover : MonoBehaviour
{
    private RectTransform rectTransform;
    private Image imagem;

    private bool mouseEmCima = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        imagem = GetComponent<Image>();

        // Começa completamente invisível
        imagem.color = new Color(1f, 1f, 1f, 0f);
    }

    void Update()
    {
        Vector2 posicaoMouse = Input.mousePosition;

        mouseEmCima = RectTransformUtility.RectangleContainsScreenPoint(
            rectTransform,
            posicaoMouse
        );

        if (mouseEmCima)
        {
            // Faz a cópia do livro aparecer destacada
            imagem.color = new Color(
                1f,
                1f,
                0.75f,
                1f
            );

            // Detecta clique esquerdo
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("LIVRO ESCOLHIDO: " + gameObject.name);
            }
        }
        else
        {
            // Fora do livro, a cópia fica invisível
            imagem.color = new Color(
                1f,
                1f,
                1f,
                0f
            );
        }
    }
}