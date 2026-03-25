using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StrangeGuyCutscene : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogueSystem;
    public GameObject avatarBox;        // Khung chứa mặt Player
    public TextMeshProUGUI nameText;    // NameText (TMP)
    public TextMeshProUGUI contentText; // Text (TMP) chính
    public Image avatarImage;

    [Header("Guide Panel Images")]
    public GameObject[] guideImages;    // Kéo 1, 2, 3, 4, 5 từ Hierarchy vào đây

    [Header("Cutscene Assets")]
    public Sprite playerAvatar;         // Ảnh mặt Player
    public Transform player;            // Object Player
    public GameObject strangeGuy;       // Object NPC MoiGioi trên Map
    public string nextMapName = "Map_Home";

    private bool hasReachedTrigger = false;

    void Start()
    {
        dialogueSystem.SetActive(false);
        strangeGuy.SetActive(false);

        // Ẩn hết hình Guide lúc đầu
        foreach (GameObject img in guideImages)
        {
            if (img != null) img.SetActive(false);
        }

        StartCoroutine(PlayCutsceneFlow());
    }

    IEnumerator PlayCutsceneFlow()
    {
        // 1. Player tự đi bộ tới vạch
        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetBool("IsMoving", true);
            anim.SetFloat("Horizontal", 1f);
        }

        while (!hasReachedTrigger)
        {
            player.Translate(Vector2.right * 2f * Time.deltaTime);
            yield return null;
        }

        // 2. Bắt đầu hội thoại
        if (anim != null) anim.SetBool("IsMoving", false);
        strangeGuy.SetActive(true);
        dialogueSystem.SetActive(true);

        // --- KỊCH BẢN THOẠI ---
        // Tham số: Tên - Ảnh Player - Index Hình Guide - Nội dung

        // Đoạn 1: Ông chú nói (Ẩn NameBox, ẩn AvatarBox, hiện hình 1)
        yield return StartCoroutine(ShowLine("", null, 0, "Nhìn mặt chú em ngơ ngơ dị chắc chắn là sinh viên mới lên Sài Gòn tìm trọ đúng ôn?"));

        // Đoạn 2: Ông chú dụ dỗ (Hiện hình 2)
        yield return StartCoroutine(ShowLine("", null, 1, "Chú em học trường nào? Xời FPT á hả, chú cho thuê trọ kế bên trường đây, lại đây chú giới thiệu cho."));

        // Đoạn 3: Player mừng rỡ (Hiện NameBox "Player", hiện AvatarBox)
        yield return StartCoroutine(ShowLine("Player", playerAvatar, -1, "Dạ cảm ơn anh, may quá em cũng đang tìm trọ, thế này như sắp chết đuối thì vớ được phao đời!"));

        // Đoạn 4: Đòi tiền cọc (Hiện hình 3)
        yield return StartCoroutine(ShowLine("", null, 2, "Đúng đắng lắm chú em, đưa anh m cọc trước 500 đi anh m dẫn dô coi phòng luôn."));

        // 3. Chuyển Map
        SceneManager.LoadScene(nextMapName);
    }

    IEnumerator ShowLine(string charName, Sprite pAvatar, int guideImgIndex, string text)
    {
        // --- 1. XỬ LÝ NAMEBOX (ẨN/HIỆN) ---
        GameObject nameBox = nameText.transform.parent.gameObject;
        if (string.IsNullOrEmpty(charName))
        {
            nameBox.SetActive(false); // Ẩn cái khung trắng khi không có tên
        }
        else
        {
            nameBox.SetActive(true);
            nameText.text = charName;
        }

        // --- 2. XỬ LÝ AVATARBOX PLAYER ---
        avatarBox.SetActive(pAvatar != null);
        if (pAvatar != null) avatarImage.sprite = pAvatar;

        // --- 3. XỬ LÝ GUIDE IMAGES (HÌNH TO) ---
        foreach (GameObject img in guideImages)
        {
            if (img != null) img.SetActive(false);
        }

        if (guideImgIndex >= 0 && guideImgIndex < guideImages.Length)
        {
            // Bật cả thằng cha (GuidePanel) nếu nó đang tắt
            guideImages[guideImgIndex].transform.parent.gameObject.SetActive(true);
            guideImages[guideImgIndex].SetActive(true);
        }

        // --- 4. CHẠY CHỮ & FIX LỆCH ---
        contentText.text = "";
        // Đảm bảo Text căn lề chuẩn
        contentText.alignment = TextAlignmentOptions.TopLeft;

        foreach (char c in text)
        {
            contentText.text += c;
            yield return new WaitForSeconds(0.04f);
        }

        // Đợi nhấn Z
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        yield return new WaitForEndOfFrame();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hasReachedTrigger = true;
        }
    }
}