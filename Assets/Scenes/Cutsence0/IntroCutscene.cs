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
    GameObject[] npcInstances;
    Rigidbody2D[] npcRigidbodies;
    Animator playerAnim;

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

        npcInstances = new GameObject[npcs.Length];
        npcRigidbodies = new Rigidbody2D[npcs.Length];
        if (player != null) playerAnim = player.GetComponent<Animator>();
        StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence()
    {
        // 1. VỪA ĐI VỪA HIỆN THOẠI
        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        // Chạy TypeText mà KHÔNG dùng yield return để nó không bắt chờ
        StartCoroutine(TypeText("Sau khi kết thúc hành trình 12 năm học...\n" + playerName + " đã chọn FPT..."));

        // Nhân vật bắt đầu đi bộ ngay lập tức
        if (playerAnim != null)
        {
            playerAnim.SetBool("IsMoving", true);
            playerAnim.SetFloat("Horizontal", 0f);
            playerAnim.SetFloat("Vertical", 1f);
        }

        float timer = 0f;
        while (timer < walkTime)
        {
            player.Translate(Vector2.up * autoWalkSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        if (playerAnim != null)
        {
            playerAnim.SetBool("IsMoving", false);
            playerAnim.SetFloat("Vertical", 1f);
        }

        // 2. Xuất hiện NPC và dấu chấm than
        for (int i = 0; i < npcs.Length; i++)
        {
            if (npcs[i] != null)
            {
                npcInstances[i] = Instantiate(npcs[i], npcSpawnPoints[i].position, Quaternion.identity);
                npcRigidbodies[i] = npcInstances[i].GetComponent<Rigidbody2D>();
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
    }

    void FixedUpdate()
    {
        if (canStartChasing && !isCutsceneEnding)
        {
            MoveNPCsTowardsPlayer();
        }
    }

    void MoveNPCsTowardsPlayer()
    {
        float speed = autoWalkSpeed * npcFollowSpeedRatio;
        for (int i = 0; i < npcInstances.Length; i++)
        {
            GameObject npc = npcInstances[i];
            if (npc == null) continue;

            if (npcRigidbodies[i] != null)
            {
                Vector2 direction = ((Vector2)player.position - npcRigidbodies[i].position).normalized;
                npcRigidbodies[i].linearVelocity = direction * speed;
            }
            else
            {
                npc.transform.position = Vector2.MoveTowards(npc.transform.position, player.position, speed * Time.fixedDeltaTime);
            }

            if (Vector2.Distance(npc.transform.position, player.position) < 0.5f)
            {
                StartCoroutine(TriggerXeOmDialogue());
            }
        }
    }

    void StopAllNPCs()
    {
        foreach (var rb in npcRigidbodies)
            if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    IEnumerator TriggerXeOmDialogue()
    {
        isCutsceneEnding = true;
        canStartChasing = false;
        StopAllNPCs();
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