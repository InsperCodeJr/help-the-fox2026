using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipalManager : MonoBehaviour
{
    public void Jogar()
    {
       SceneManager.LoadScene(1);
    }
}
