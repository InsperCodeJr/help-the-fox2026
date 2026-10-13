using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RoomExitPortal : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float interactionDistance = 1.5f;
    [SerializeField] private bool useSceneIndex = true;
    [SerializeField] private int sceneIndex = 1;
    [SerializeField] private string sceneName = "SalaInicial";

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (player == null)
        {
            player = ResolvePlayer();
        }

        if (player == null)
        {
            return;
        }

        if (Vector2.Distance(transform.position, player.position) <= interactionDistance && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (useSceneIndex)
            {
                SceneManager.LoadScene(sceneIndex);
                return;
            }

            SceneManager.LoadScene(sceneName);
        }
    }

    private Transform ResolvePlayer()
    {
        GameObject foundPlayer = GameObject.FindWithTag("Player");

        if (foundPlayer == null)
        {
            foundPlayer = GameObject.Find("Player");
        }

        if (foundPlayer == null)
        {
            foundPlayer = GameObject.Find("player");
        }

        return foundPlayer != null ? foundPlayer.transform : null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.8f, 0.6f, 1f, 0.8f);
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
