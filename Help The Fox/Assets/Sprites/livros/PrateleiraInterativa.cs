using UnityEngine;

public class PrateleiraInterativa : MonoBehaviour
{
    public GameObject textoInteracao;
    public GameObject painelLivros;
    public MovimentoRaposa movimentoRaposa;

    private bool jogadorPerto = false;
    private bool painelAberto = false;

    void Update()
    {
        if (jogadorPerto && Input.GetKeyDown(KeyCode.P) && !painelAberto)
        {
            movimentoRaposa.PararMovimento();
            movimentoRaposa.enabled = false;

            painelLivros.SetActive(true);
            textoInteracao.SetActive(false);

            painelAberto = true;
        }

        if (painelAberto && Input.GetKeyDown(KeyCode.Escape))
        {
            painelLivros.SetActive(false);

            movimentoRaposa.enabled = true;

            painelAberto = false;

            if (jogadorPerto)
            {
                textoInteracao.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = true;

            if (!painelAberto)
            {
                textoInteracao.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = false;
            textoInteracao.SetActive(false);
        }
    }
}