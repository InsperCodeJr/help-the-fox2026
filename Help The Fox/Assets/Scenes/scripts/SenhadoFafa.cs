using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LoginScreen : MonoBehaviour
{
    public TMP_InputField passwordField;
    private string correctPassword = "Insper";

    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            CheckPassword();
        }
    }

    public void CheckPassword()
    {
        if (passwordField.text == correctPassword)
        {
            SceneManager.LoadScene(3);
        }
        else
        {
            passwordField.text = "Senha Errada!";
        }
    }
}