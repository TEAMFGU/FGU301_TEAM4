using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class StrangeGuyCutscene : MonoBehaviour
{
    public Transform player; // Kéo SPlayer (con thật) vào đây
    public GameObject strangeGuy; // Kéo MoiGioi vào đây
    public GameObject dialoguePanel; // Kéo DialoguePanel vào đây
    public TextMeshProUGUI dialogueText; // Kéo Text (TMP) vào đây
    public float walkSpeed = 2f;

    private bool hasTriggered = false;

    void Start()
    {
        // Vừa vào Map là cho Player tự động đi sang phải luôn
        StartCoroutine(AutoWalkSequence());
    }

    IEnumerator AutoWalkSequence()
    {
        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetBool("IsMoving", true);
            anim.SetFloat("Horizontal", 1f); // Set hướng nhìn sang phải
        }

        // Player cứ thế đi bộ sang phải cho đến khi đụng Trigger
        while (!hasTriggered)
        {
            player.Translate(Vector2.right * walkSpeed * Time.deltaTime);
            yield return null;
        }

        // Khi đụng Trigger (hasTriggered = true), dừng lại và hiện thoại
        if (anim != null) anim.SetBool("IsMoving", false);

        strangeGuy.SetActive(true); // Hiện hình ông đa cấp
        dialoguePanel.SetActive(true); // Hiện khung thoại

        yield return StartCoroutine(TypeText("Ông chú lạ mặt: Nhìn mặt chú em ngơ ngơ dị chắc chắn là sinh viên mới lên Sài Gòn tìm trọ đúng ôn?"));
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

        yield return StartCoroutine(TypeText("Ông chú lạ mặt: Chú em học trường nào? xời FPT á hả chú cho thuê trọ kế bên trường đây..."));
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

        // Hết thoại thì chuyển về map Home
        SceneManager.LoadScene("Map_01Room");
    }

    // Hàm này kích hoạt khi Player chạm vào cái Box Collider giữa đường
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
        }
    }

    IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.04f);
        }
    }
}