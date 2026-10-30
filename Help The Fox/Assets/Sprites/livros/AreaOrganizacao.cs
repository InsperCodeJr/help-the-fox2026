using UnityEngine;

public class AreaOrganizacao : MonoBehaviour
{
    public GameObject textoOrganizacao;
    public GameObject painelOrganizacao;
    public MovimentoRaposa movimentoRaposa;

    private bool jogadorPerto = false;
    private bool painelAberto = false;
    private InterfaceMochila interfaceMochila;

    void Start()
    {
        interfaceMochila = FindAnyObjectByType<InterfaceMochila>();
        textoOrganizacao.SetActive(false);
        painelOrganizacao.SetActive(false);
    }

    void Update()
    {
        if (interfaceMochila != null && interfaceMochila.EstaAberta) return;

        if (jogadorPerto && Input.GetKeyDown(KeyCode.P))
        {
            if (painelAberto)
            {
                FecharPainel();
            }
            else
            {
                AbrirPainel();
            }
        }
    }

    void AbrirPainel()
    {
        movimentoRaposa.PararMovimento();
        movimentoRaposa.enabled = false;

        textoOrganizacao.SetActive(false);
        painelOrganizacao.SetActive(true);

        painelAberto = true;
    }

    public void FecharPainel()
    {
        painelOrganizacao.SetActive(false);

        movimentoRaposa.enabled = true;

        painelAberto = false;

        if (jogadorPerto)
        {
            textoOrganizacao.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = true;

            if (!painelAberto)
            {
                textoOrganizacao.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorPerto = false;
            textoOrganizacao.SetActive(false);
        }
    }
}
