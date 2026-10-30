using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class CriadorEventSystem : MonoBehaviour
{
    void Awake()
    {
        if (EventSystem.current == null)
        {
            GameObject objetoEventSystem = new GameObject("EventSystem");

            objetoEventSystem.AddComponent<EventSystem>();
            objetoEventSystem.AddComponent<InputSystemUIInputModule>();

            Debug.Log("EventSystem criado automaticamente.");
        }
    }
}