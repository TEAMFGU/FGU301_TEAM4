using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Animator anim;

    private Vector2 movement;
    private Vector2 lastDir = Vector2.down; // Mặc định nhìn xuống

    void Update()
    {
        // 1. Lấy input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 2. Ép nhân vật chỉ đi 1 hướng (RPG style) để ko bị chéo
        if (Mathf.Abs(h) > Mathf.Abs(v)) v = 0;
        else h = 0;

        movement = new Vector2(h, v);

        // 3. Truyền dữ liệu vào Animator (PHẢI khớp tên với bảng Parameters)
        if (movement != Vector2.zero)
        {
            anim.SetFloat("Horizontal", movement.x);
            anim.SetFloat("Vertical", movement.y);
            anim.SetFloat("Speed", 1); // Đang đi
            lastDir = movement; // Lưu lại hướng cuối
        }
        else
        {
            // Khi dừng lại: Giữ hướng nhìn cuối cùng và Speed = 0
            anim.SetFloat("Horizontal", lastDir.x);
            anim.SetFloat("Vertical", lastDir.y);
            anim.SetFloat("Speed", 0); // Đứng yên
        }
    }

    void FixedUpdate()
    {
        rb.velocity = movement.normalized * moveSpeed;
    }



    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Tự tìm Rigidbody2D trên người nó
        anim = GetComponent<Animator>(); // Tự tìm Animator trên người nó
    }
}