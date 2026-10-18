using UnityEngine;

public class PrateleiraInterativa : MonoBehaviour
{
    public GameObject textoInteracao;
    public GameObject painelLivros;
    public MovimentoRaposa movimentoRaposa;

    [Header("Estantes")]
    public GameObject estante06;
    public GameObject estante10;

    [Header("Tipo desta prateleira")]
    public bool usarEstante10 = false;

    private bool jogadorPerto = false;
    private bool painelAberto = false;

    void Start()
    {
        estante06.SetActive(false);
        estante10.SetActive(false);
        painelLivros.SetActive(false);
    }

    void Update()
    {
        if (jogadorPerto && Input.GetKeyDown(KeyCode.P) && !painelAberto)
        {
            movimentoRaposa.PararMovimento();
            movimentoRaposa.enabled = false;

            painelLivros.SetActive(true);
            textoInteracao.SetActive(false);

            if (usarEstante10)
            {
                estante06.SetActive(false);
                estante10.SetActive(true);
            }
            else
            {
                estante10.SetActive(false);
                estante06.SetActive(true);
            }

            painelAberto = true;
        }

        if (painelAberto && Input.GetKeyDown(KeyCode.Escape))
        {
            estante06.SetActive(false);
            estante10.SetActive(false);
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