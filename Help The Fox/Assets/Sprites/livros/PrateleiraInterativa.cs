using UnityEngine;

public class PrateleiraInterativa : MonoBehaviour
{
    public GameObject textoInteracao;
    public GameObject painelLivros;
    public MovimentoRaposa movimentoRaposa;

    [Header("Identificacao")]
    public string idPrateleira;

    [Header("Estantes")]
    public GameObject estante06;
    public GameObject estante10;

    [Header("Tipo desta prateleira")]
    public bool usarEstante10 = false;

    private bool jogadorPerto = false;
    private bool painelAberto = false;

    public static string PrateleiraAtual { get; private set; }

    void Start()
    {
        estante06.SetActive(false);
        estante10.SetActive(false);
        painelLivros.SetActive(false);
    }

    void Update()
    {
        if (jogadorPerto && Input.GetKeyDown(KeyCode.P))
        {
            if (!painelAberto)
            {
                AbrirEstante();
            }
            else
            {
                FecharEstante();
            }
        }
    }

    void AbrirEstante()
    {
        movimentoRaposa.PararMovimento();
        movimentoRaposa.enabled = false;

        PrateleiraAtual = idPrateleira;

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

    void FecharEstante()
    {
        estante06.SetActive(false);
        estante10.SetActive(false);
        painelLivros.SetActive(false);

        movimentoRaposa.enabled = true;

        painelAberto = false;
        PrateleiraAtual = "";

        if (jogadorPerto)
        {
            textoInteracao.SetActive(true);
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