using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundMask;

    Rigidbody2D rb;
    bool isGrounded;

    void Awake() { rb = GetComponent<Rigidbody2D>(); }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

        // Flip sprite when changing direction
        if (x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Pause key (Escape) pressed → toggling pause menu");
            LevelUI.TogglePause();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Reset key (R) pressed → respawning player");
            LevelState.Respawn();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("Unplayable key (U) pressed → marking level as unplayable");
            LevelState.MarkUnplayable();
        }
    }
}
