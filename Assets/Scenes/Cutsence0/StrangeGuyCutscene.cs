using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StrangeGuyCutscene : MonoBehaviour
{
    [Header("Guide Panel Images")]
    public GameObject[] guideImages;   // size=2 → [0]=ảnh1, [1]=ảnh2

    [Header("Cutscene Assets")]
    public Transform  player;           
    public GameObject strangeGuy;
    public Sprite     playerAvatar;    // kéo sprite mặt player vào
    public string     nextMapName = "Scene_Map001_Home";

    private bool hasReachedTrigger = false;
    private bool dialogueDone      = false;

    void Start()
    {
        strangeGuy.SetActive(false);
        foreach (GameObject img in guideImages)
            if (img != null) img.SetActive(false);

        StartCoroutine(PlayCutsceneFlow());
    }

    IEnumerator PlayCutsceneFlow()
    {
        // ── 1. Player tự đi sang phải cho đến khi chạm trigger ──────────────
        Animator anim = player.GetComponent<Animator>();
        if (anim != null) { anim.SetBool("IsMoving", true); anim.SetFloat("Horizontal", 1f); }

        while (!hasReachedTrigger)
        {
            player.Translate(Vector2.right * 2f * Time.deltaTime);
            yield return null;
        }

        // ── 2. Dừng player, bật NPC ──────────────────────────────────────────
        if (anim != null) anim.SetBool("IsMoving", false);
        strangeGuy.SetActive(true);

        // ── 3. Dialogue + hình xuất hiện đồng thời, nhấn Z để qua dòng tiếp ─
        yield return Line("",       null,         0,  "Nhìn mặt chú em ngơ ngơ dị chắc chắn là sinh viên mới lên Sài Gòn tìm trọ đúng ôn?");
        yield return Line("",       null,         1,  "Chú em học trường nào? Xời FPT á hả, chú cho thuê trọ kế bên trường đây, lại đây chú giới thiệu cho.");
        yield return Line("Player", playerAvatar, -1, "Dạ cảm ơn anh, may quá em cũng đang tìm trọ, thế này như sắp chết đuối thì vớ được phao đời!");
        yield return Line("",       null,         0,  "Đúng đắng lắm chú em, đưa anh m cọc trước 500 đi anh m dẫn dô coi phòng luôn.");

        // ── 4. Chuyển map sau khi hết thoại ─────────────────────────────────
        SceneManager.LoadScene(nextMapName);
    }

    // ── Hiện 1 dòng thoại: guide image + dialogue cùng lúc ──────────────────
    IEnumerator Line(string speaker, Sprite avatar, int guideIndex, string text)
    {
        // Ẩn hết guide image cũ
        foreach (GameObject img in guideImages)
            if (img != null) img.SetActive(false);

        // Bật guide image mới (nếu có)
        if (guideIndex >= 0 && guideIndex < guideImages.Length && guideImages[guideIndex] != null)
            guideImages[guideIndex].SetActive(true);

        // Hiện dialogue → đợi người dùng nhấn Z
        dialogueDone = false;
        DialogueManager.Instance.ShowCutsceneLine(speaker, avatar, text, () => dialogueDone = true);
        yield return new WaitUntil(() => dialogueDone);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            hasReachedTrigger = true;
    }
}