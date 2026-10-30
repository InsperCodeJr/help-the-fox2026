using UnityEngine;

public class MovimentoRaposa : MonoBehaviour
{
    public float velocidade = 5f;

    private Rigidbody2D rb;
    private Vector2 movimento;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        movimento.x = 0;
        movimento.y = 0;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            movimento.x += 1;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            movimento.x -= 1;
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            movimento.y += 1;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            movimento.y -= 1;
        }

        movimento = movimento.normalized;

        bool andando = movimento != Vector2.zero;

        animator.SetBool("Andando", andando);

        if (andando)
        {
            animator.SetFloat("Horizontal", movimento.x);
            animator.SetFloat("Vertical", movimento.y);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movimento * velocidade;
    }

    public void PararMovimento()
    {
        movimento = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("Andando", false);
    }
}