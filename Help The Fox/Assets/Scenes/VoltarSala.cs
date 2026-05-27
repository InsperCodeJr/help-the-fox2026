using UnityEngine;
using UnityEngine.SceneManagement;
public class VoltarSala : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Voltar()
    {
        SceneManager.LoadScene(1);
    }
}