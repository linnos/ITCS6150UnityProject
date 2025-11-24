using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    public float moveSpeed = 9f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundMask;

    Rigidbody2D rb;
    bool isGrounded;

    Camera mainCam;
    public Vector3 cameraOffset = new Vector3(0f, 1.5f, -10f);
    public float cameraSmooth = 5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Find or create the main camera
        if (Camera.main != null)
        {
            mainCam = Camera.main;
        }
        else
        {
            GameObject camObj = new GameObject("MainCamera");
            camObj.tag = "MainCamera";
            mainCam = camObj.AddComponent<Camera>();
        }

        // Position camera initially
        mainCam.transform.position = transform.position + cameraOffset;
    }

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

        if(this.transform.position.y < 2f)
        {
            Debug.Log("Player fell out of bounds → respawning player");
            LevelState.Respawn();
        }
    }

    void LateUpdate()
    {
        if (mainCam != null)
        {
            Vector3 targetPos = transform.position + cameraOffset;
            targetPos.z = cameraOffset.z; // keep z offset consistent
            mainCam.transform.position = targetPos;
        }
    }


}
