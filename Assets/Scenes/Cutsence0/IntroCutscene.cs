using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroCutscene : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public PlayerMovement playerMovement;
    public float autoWalkSpeed = 2f;
    public float walkTime = 3f;

    [Header("Dialogue UI (intro đi bộ)")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public float typeSpeed = 0.04f;

    [Header("Guide UI")]
    public GameObject guidePanel;
    public Image blackScreen;

    [Header("XeOm Dialogue")]
    public GameObject xeOmGuideImage;
    public Sprite xeOmAvatar;       // ← kéo sprite mặt xe ôm vào đây

    [Header("NPCs Setup")]
    public GameObject[] npcs;
    public Transform[] npcSpawnPoints;
    public GameObject[] exclamationMarks;
    public float npcFollowSpeedRatio = 0.7f;

    [Header("Scene Transition")]
    public string nextMapName = "Map000_Strange_guy";
    public GameObject teleportTrigger;

    bool canStartChasing = false;
    bool isCutsceneEnding = false;
    string playerName;
    GameObject[] npcInstances;
    Rigidbody2D[] npcRigidbodies;
    Rigidbody2D playerRb;
    Animator playerAnim;

    // flag dùng chung cho ShowCutsceneLine callback
    bool dialogueDone = false;

    void Start()
    {
        playerName = PlayerPrefs.GetString("PlayerName", "Người chơi");

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (guidePanel != null) guidePanel.SetActive(false);
        if (blackScreen != null) blackScreen.gameObject.SetActive(false);
        if (teleportTrigger != null) teleportTrigger.SetActive(false);
        if (xeOmGuideImage != null) xeOmGuideImage.SetActive(false);

        foreach (var mark in exclamationMarks) mark.SetActive(false);
        if (playerMovement != null) playerMovement.enabled = false;

        npcInstances = new GameObject[npcs.Length];
        npcRigidbodies = new Rigidbody2D[npcs.Length];

        if (player != null)
        {
            playerAnim = player.GetComponent<Animator>();
            playerRb = player.GetComponent<Rigidbody2D>();
        }

        StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence()
    {
        // 1. Đi bộ + thoại intro (dùng dialoguePanel cũ)
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        StartCoroutine(TypeText("Sau khi kết thúc hành trình 12 năm học...\n" + playerName + " đã chọn FPT..."));

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

        if (playerAnim != null) { playerAnim.SetBool("IsMoving", false); playerAnim.SetFloat("Vertical", 1f); }
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        // 2. Spawn NPC và dấu !
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

        // 3. Hướng dẫn
        if (blackScreen != null) blackScreen.gameObject.SetActive(true);
        if (guidePanel != null) guidePanel.SetActive(true);

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

        if (blackScreen != null) blackScreen.gameObject.SetActive(false);
        if (guidePanel != null) guidePanel.SetActive(false);
        if (teleportTrigger != null) teleportTrigger.SetActive(true);
        if (playerMovement != null) playerMovement.enabled = true;

        yield return new WaitForSeconds(0.1f);
        canStartChasing = true;
    }

    void Update() { }

    void FixedUpdate()
    {
        if (canStartChasing && !isCutsceneEnding)
            MoveNPCsTowardsPlayer();
    }

    void MoveNPCsTowardsPlayer()
    {
        float speed = autoWalkSpeed * npcFollowSpeedRatio;
        for (int i = 0; i < npcInstances.Length; i++)
        {
            GameObject npc = npcInstances[i];
            if (npc == null) continue;

            if (npcRigidbodies[i] != null)
                npcRigidbodies[i].linearVelocity =
                    ((Vector2)player.position - npcRigidbodies[i].position).normalized * speed;
            else
                npc.transform.position = Vector2.MoveTowards(
                    npc.transform.position, player.position, speed * Time.fixedDeltaTime);

            if (Vector2.Distance(npc.transform.position, player.position) < 0.5f)
            {
                isCutsceneEnding = true;
                canStartChasing = false;
                StopAllNPCs();
                StopPlayer();
                StartCoroutine(TriggerXeOmDialogue());
                return;
            }
        }
    }

    void StopAllNPCs()
    {
        foreach (var rb in npcRigidbodies)
            if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    void StopPlayer()
    {
        if (playerMovement != null) playerMovement.enabled = false;
        if (playerRb != null) playerRb.linearVelocity = Vector2.zero;
        if (playerAnim != null) playerAnim.SetBool("IsMoving", false);
    }

    IEnumerator TriggerXeOmDialogue()
    {
        if (DialogueManager.Instance == null)
        {
            Debug.LogError("IntroCutscene: DialogueManager.Instance null! Kéo Dialogue_Canvas.prefab vào scene Map00_BenXe.");
            yield break;
        }

        // Nền đen + hình xe ôm
        if (blackScreen != null) blackScreen.gameObject.SetActive(true);
        if (xeOmGuideImage != null) xeOmGuideImage.SetActive(true);

        // ── Dòng 1 ────────────────────────────────────────────────────────────
        dialogueDone = false;
        DialogueManager.Instance.ShowCutsceneLine(
            "Chú Xe Ôm", xeOmAvatar,
            "Cháu mới dưới quê lên đúng k? Chùm khu này đây",
            () => dialogueDone = true);
        yield return new WaitUntil(() => dialogueDone);

        // ── Dòng 2 ────────────────────────────────────────────────────────────
        dialogueDone = false;
        DialogueManager.Instance.ShowCutsceneLine(
            "Chú Xe Ôm", xeOmAvatar,
            "lên xe chú chở lấy rẻ 200k thôi!",
            () => dialogueDone = true);
        yield return new WaitUntil(() => dialogueDone);

        // ── Dọn dẹp & chuyển scene ────────────────────────────────────────────
        DialogueManager.Instance.ForceClose();
        if (blackScreen != null) blackScreen.gameObject.SetActive(false);
        if (xeOmGuideImage != null) xeOmGuideImage.SetActive(false);

        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene(nextMapName);
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