using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class OpenPC : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float interactionDistance = 1.5f;
    [SerializeField] private int sceneIndex = 2;

    private void Awake()
    {
        if (player != null) return;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            playerObject = GameObject.Find("player");
        }

        if (playerObject == null)
        {
            playerObject = GameObject.Find("Player");
        }

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void Update()
    {
        if (player == null || Keyboard.current == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool playerIsClose = distanceToPlayer <= interactionDistance;

        if (playerIsClose && Keyboard.current.eKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
