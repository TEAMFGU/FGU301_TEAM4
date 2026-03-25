using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 8f;

    private Rigidbody2D rb;
    private Animator anim;

    private Vector2 movement;
    private Vector2 lastDir = Vector2.down; // hướng nhìn mặc định

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Chỉ cho đi 1 hướng (RPG style)
        if (Mathf.Abs(h) > Mathf.Abs(v))
        {
            v = 0;
        }
        else
        {
            h = 0;
        }

        movement = new Vector2(h, v);

        // Kiểm tra có đang di chuyển không
        bool isMoving = movement != Vector2.zero;

        if (isMoving)
            lastDir = movement;

        // lastDir luôn là hướng di chuyển cuối (±1) → cả Walk lẫn Idle đều dùng được
        anim.SetBool("IsMoving", isMoving);
        anim.SetFloat("Horizontal", lastDir.x);
        anim.SetFloat("Vertical", lastDir.y);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement.normalized * moveSpeed;
    }
}