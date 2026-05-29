using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;
    private Vector2 lastDirection = Vector2.down;
    private string currentAnimation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        ReadInput();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void ReadInput()
    {
        moveInput = Vector2.zero;

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.aKey.isPressed) moveInput.x -= 1;
        if (keyboard.dKey.isPressed) moveInput.x += 1;
        if (keyboard.sKey.isPressed) moveInput.y -= 1;
        if (keyboard.wKey.isPressed) moveInput.y += 1;

        moveInput = moveInput.normalized;
    }

    private void UpdateAnimation()
    {
        if (moveInput != Vector2.zero)
        {
            lastDirection = moveInput;

            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                PlayAnimation(moveInput.x > 0 ? "right_walk" : "left_walk");
            }
            else
            {
                PlayAnimation(moveInput.y > 0 ? "up_walk" : "down_walk");
            }
        }
        else
        {
            if (Mathf.Abs(lastDirection.x) > Mathf.Abs(lastDirection.y))
            {
                PlayAnimation(lastDirection.x > 0 ? "right_idle" : "left_idle");
            }
            else
            {
                PlayAnimation(lastDirection.y > 0 ? "back_idle" : "front_idle");
            }
        }
    }

    private void PlayAnimation(string animationName)
    {
        if (currentAnimation == animationName) return;

        animator.Play(animationName);
        currentAnimation = animationName;
    }
}