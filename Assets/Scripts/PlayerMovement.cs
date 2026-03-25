using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 8f;

    /// <summary>
    /// Set false để khóa di chuyển (dùng khi đang chọn dialogue option)
    /// </summary>
    [HideInInspector] public bool canMove = true;

    private Rigidbody2D rb;
    private Animator anim;

    private Vector2 movement;
    private Vector2 lastDir = Vector2.down;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!canMove)
        {
            movement = Vector2.zero;
            // Giữ idle animation đúng hướng cuối
            anim.SetBool("IsMoving", false);
            anim.SetFloat("Horizontal", lastDir.x);
            anim.SetFloat("Vertical", lastDir.y);
            return;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Chỉ cho đi 1 hướng (RPG style)
        if (Mathf.Abs(h) > Mathf.Abs(v))
            v = 0;
        else
            h = 0;

        movement = new Vector2(h, v);

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