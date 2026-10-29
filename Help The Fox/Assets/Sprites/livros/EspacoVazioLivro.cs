using UnityEngine;

public class EspacoVazioLivro : MonoBehaviour
{
    public int numeroLivro;

    private RectTransform rectTransform;
    private InterfaceMochila interfaceMochila;

    void Start()
    {
        interfaceMochila = GetComponentInParent<InterfaceMochila>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (interfaceMochila != null && interfaceMochila.EstaAberta) return;

        if (string.IsNullOrEmpty(PrateleiraInterativa.PrateleiraAtual))
        {
            return;
        }

        string idLivro =
            PrateleiraInterativa.PrateleiraAtual
            + "_L"
            + numeroLivro.ToString("00");

        if (!EstadoLivros.LivroFoiRetirado(idLivro))
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
            EstadoLivros.DevolverLivro(idLivro);

            Debug.Log("LIVRO DEVOLVIDO AO CLICAR NO ESPACO: " + idLivro);
        }
    }
}
