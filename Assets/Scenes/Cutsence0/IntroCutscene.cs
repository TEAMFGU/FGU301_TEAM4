using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IntroCutscene : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public PlayerMovement playerMovement;
    public float autoWalkSpeed = 2f;
    public float walkTime = 3f;

    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public float typeSpeed = 0.04f;

    [Header("Guide UI")]
    public GameObject guidePanel;
    public Image blackScreen;

    [Header("NPCs Setup")]
    public GameObject[] npcs;
    public Transform[] npcSpawnPoints;
    public GameObject[] exclamationMarks;
    public float npcFollowSpeedRatio = 0.7f;

    [Header("Scene Transition")]
    public string nextMapName = "Map01";
    public GameObject teleportTrigger;

    bool canStartChasing = false;
    bool isCutsceneEnding = false;
    string playerName;

    void Start()
    {
        playerName = PlayerPrefs.GetString("PlayerName", "Người chơi");

        // Ẩn hết từ đầu
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (guidePanel != null) guidePanel.SetActive(false);
        if (blackScreen != null) blackScreen.gameObject.SetActive(false);
        if (teleportTrigger != null) teleportTrigger.SetActive(false);

        foreach (var mark in exclamationMarks) mark.SetActive(false);
        if (playerMovement != null) playerMovement.enabled = false;

        StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence()
    {
        // 1. VỪA ĐI VỪA HIỆN THOẠI
        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        // Chạy TypeText mà KHÔNG dùng yield return để nó không bắt chờ
        StartCoroutine(TypeText("Sau khi kết thúc hành trình 12 năm học...\n" + playerName + " đã chọn FPT..."));

        // Nhân vật bắt đầu đi bộ ngay lập tức
        float timer = 0f;
        while (timer < walkTime)
        {
            player.Translate(Vector2.up * autoWalkSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        // 2. Xuất hiện NPC và dấu chấm than
        for (int i = 0; i < npcs.Length; i++)
        {
            if (npcs[i] != null)
            {
                npcs[i].transform.position = npcSpawnPoints[i].position;
                npcs[i].SetActive(true);
                if (exclamationMarks.Length > i) exclamationMarks[i].SetActive(true);
            }
        }

        yield return new WaitForSeconds(1.0f);

        // 3. HIỆN HƯỚNG DẪN
        if (blackScreen != null) blackScreen.gameObject.SetActive(true);
        if (guidePanel != null) guidePanel.SetActive(true);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        // Đợi ấn Z
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

        // 4. BIẾN MẤT NGAY LẬP TỨC KHI ẤN Z
        if (blackScreen != null) blackScreen.gameObject.SetActive(false);
        if (guidePanel != null) guidePanel.SetActive(false);

        // Trả quyền điều khiển và hiện Trigger
        if (teleportTrigger != null) teleportTrigger.SetActive(true);
        if (playerMovement != null) playerMovement.enabled = true;

        yield return new WaitForSeconds(0.1f); // Nghỉ một chút để tránh dính phím
        canStartChasing = true;
    }

    void Update()
    {
        if (canStartChasing && !isCutsceneEnding)
        {
            MoveNPCsTowardsPlayer();
        }
    }

    void MoveNPCsTowardsPlayer()
    {
        float speed = autoWalkSpeed * npcFollowSpeedRatio;
        foreach (GameObject npc in npcs)
        {
            if (npc != null)
            {
                npc.transform.position = Vector2.MoveTowards(npc.transform.position, player.position, speed * Time.deltaTime);
                if (Vector2.Distance(npc.transform.position, player.position) < 0.5f)
                {
                    StartCoroutine(TriggerXeOmDialogue());
                }
            }
        }
    }

    IEnumerator TriggerXeOmDialogue()
    {
        isCutsceneEnding = true;
        canStartChasing = false;
        if (playerMovement != null) playerMovement.enabled = false;

        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        yield return StartCoroutine(TypeText("Chú xe ôm: Cháu mới dưới quê lên đúng k? Chùm khu này đây, lên xe chú chở lấy rẻ 200k thôi!"));

        yield return new WaitForSeconds(2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextMapName);
    }

    IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }
}