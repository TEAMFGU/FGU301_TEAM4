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
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Thao tác di chuyển bằng các phím W, A, S, D
        if (Input.GetKey(KeyCode.W)) v += 1;
        if (Input.GetKey(KeyCode.S)) v -= 1;
        if (Input.GetKey(KeyCode.A)) h -= 1;
        if (Input.GetKey(KeyCode.D)) h += 1;

        anim.SetFloat("Horizontal", h);
        anim.SetFloat("Vertical", v);

        // Tính speed, sử dụng speed để điều chỉnh animation blend tree
        float speed = Mathf.Sqrt(h * h + v * v);  
        anim.SetFloat("Speed", speed);

        transform.Translate(new Vector3(h, v, 0) * speed * Time.deltaTime);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement.normalized * moveSpeed;
    }



    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Tự tìm Rigidbody2D trên người nó
        anim = GetComponent<Animator>(); // Tự tìm Animator trên người nó
    }
}