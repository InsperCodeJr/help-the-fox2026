using UnityEngine;
using TMPro;

public class PosicaoOrganizacao : MonoBehaviour
{
    public int numeroPosicao;
    public TMP_Text textoPosicao;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        Vector2 posicaoMouse = Input.mousePosition;

        bool mouseEmCima =
            RectTransformUtility.RectangleContainsScreenPoint(
                rectTransform,
                posicaoMouse
            );

        if (mouseEmCima && Input.GetMouseButtonDown(0))
        {
            Debug.Log(
                "CLIQUE DETECTADO NA POSICAO "
                + numeroPosicao
            );

            InterfaceOrganizacao interfaceOrganizacao =
                GetComponentInParent<InterfaceOrganizacao>();

            if (interfaceOrganizacao == null)
            {
                Debug.LogError(
                    "NAO ENCONTREI InterfaceOrganizacao NO PAI!"
                );

                return;
            }

            Debug.Log("InterfaceOrganizacao encontrada!");

            interfaceOrganizacao.ColocarLivroNaPosicao(
                numeroPosicao
            );
        }
    }

    public void AtualizarTexto(string idLivro)
    {
        if (string.IsNullOrEmpty(idLivro))
        {
            textoPosicao.text = numeroPosicao.ToString();
        }
        else
        {
            textoPosicao.text = idLivro;
        }
    }
}