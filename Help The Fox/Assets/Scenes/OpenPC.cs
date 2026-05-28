using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneChanger : MonoBehaviour
{
    public int sceneIndex = 2;

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}