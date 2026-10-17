using UnityEngine;

public class PrateleiraInterativa : MonoBehaviour
{
    public GameObject textoInteracao;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            textoInteracao.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            textoInteracao.SetActive(false);
        }
    }
}