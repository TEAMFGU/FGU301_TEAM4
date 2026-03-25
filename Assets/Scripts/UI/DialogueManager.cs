using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Dialogue UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Image avatarImage;
    [SerializeField] private TextMeshProUGUI nameTagText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI hintText;

    [Header("Typing Effect")]
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private bool enableTypingEffect = true;

    [Header("Choice UI")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private GameObject[] optionRows;
    [SerializeField] private Image[] cursorImages;
    [SerializeField] private TextMeshProUGUI[] optionTexts;

    private string[] currentLines;
    private int lineIndex;
    private Action onDialogueComplete;
    private bool isOpen;
    private Coroutine typingCoroutine;

    // ─── hintText: biết sau dialogue cuối có choices không ───
    private bool hasChoicesAfterDialogue;

    // ─── Choice state ───
    private int selectedChoiceIndex;
    private bool isChoosingOption;
    private Action<int> onChoiceSelected;
    private int choiceCount;

    // ─── Cache PlayerMovement để khóa/mở di chuyển ───
    private PlayerMovement cachedPlayerMovement;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        else
            Debug.LogError("DialogueManager: dialoguePanel chua duoc gan trong Inspector!");

        if (choicePanel != null)
            choicePanel.SetActive(false);
    }

    private void Update()
    {
        // ─── Input chọn option (ưu tiên xử lý trước) ───
        if (isChoosingOption)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedChoiceIndex = (selectedChoiceIndex - 1 + choiceCount) % choiceCount;
                UpdateCursorDisplay();
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedChoiceIndex = (selectedChoiceIndex + 1) % choiceCount;
                UpdateCursorDisplay();
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                ConfirmChoice();
            }
            return;
        }

        // ─── Input dialogue bình thường ───
        if (!isOpen) return;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            // Nếu text đang typing → skip animation, hiển thị ngay
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
                messageText.text = currentLines[lineIndex];
                return;
            }

            NextLine();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            CloseDialogue();
        }
    }

    /// <param name="hasChoicesAfter">True nếu sau dialogue này sẽ hiện choices → hintText dòng cuối sẽ là "Tiếp tục" thay vì "Đóng"</param>
    public void ShowDialogue(NPCData npcData, string[] lines, Action onComplete = null, bool hasChoicesAfter = false)
    {
        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning("Khong co dialogue lines duoc cung cap!");
            return;
        }

        currentLines = lines;
        lineIndex = 0;
        onDialogueComplete = onComplete;
        isOpen = true;
        isChoosingOption = false;
        hasChoicesAfterDialogue = hasChoicesAfter;

        bool hasFace = npcData != null && npcData.faceSprite != null;
        avatarImage.gameObject.SetActive(hasFace);

        if (hasFace)
            avatarImage.sprite = npcData.faceSprite;

        nameTagText.text = npcData != null ? npcData.npcName : "???";

        if (choicePanel != null) choicePanel.SetActive(false);
        if (messageText != null) messageText.gameObject.SetActive(true);

        dialoguePanel.SetActive(true);
        DisplayLine(currentLines[0]);
    }

    /// <summary>
    /// Hiển thị bảng chọn option. Khóa PlayerMovement khi đang chọn.
    /// </summary>
    public void ShowChoices(string[] choices, Action<int> callback)
    {
        if (choices == null || choices.Length == 0)
        {
            Debug.LogWarning("ShowChoices: Không có lựa chọn nào!");
            callback?.Invoke(-1);
            return;
        }

        if (optionTexts == null || optionTexts.Length == 0)
        {
            Debug.LogError("ShowChoices: optionTexts chưa được gán trong Inspector!");
            callback?.Invoke(-1);
            return;
        }

        choiceCount = Mathf.Min(choices.Length, optionTexts.Length);
        onChoiceSelected = callback;
        selectedChoiceIndex = 0;
        isChoosingOption = true;

        // Thiết lập nội dung từng option row
        for (int i = 0; i < optionRows.Length; i++)
        {
            bool active = i < choiceCount;
            if (optionRows[i] != null) optionRows[i].SetActive(active);
            if (active && optionTexts[i] != null) optionTexts[i].text = choices[i];
        }

        if (dialoguePanel != null && !dialoguePanel.activeInHierarchy)
            dialoguePanel.SetActive(true);

        if (messageText != null) messageText.gameObject.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(true);

        if (hintText != null)
        {
            hintText.text = "W/S: Chon  |  Z: Xac nhan";
            hintText.color = Color.white;
        }

        // ─── Khóa di chuyển player khi đang chọn ───
        SetPlayerMovement(false);

        UpdateCursorDisplay();
    }

    public bool IsOpen() => isOpen || isChoosingOption;

    private void DisplayLine(string line)
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "Ban");
        string displayText = line
            .Replace("{player}", playerName)
            .Replace("{playerName}", playerName);

        if (messageText != null)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            if (enableTypingEffect)
                typingCoroutine = StartCoroutine(TypeTextCoroutine(displayText));
            else
            {
                messageText.text = displayText;
            }
        }
        else
        {
            Debug.LogError("messageText chua duoc gan trong Inspector!");
        }

        UpdateHintText();
    }

    /// <summary>
    /// Cập nhật hintText đúng theo trạng thái dòng hiện tại.
    /// Dòng cuối + có choices sau → "Tiep tuc"
    /// Dòng cuối + không có choices → "Dong"
    /// Dòng giữa → "Tiep tuc"
    /// </summary>
    private void UpdateHintText()
    {
        if (hintText == null) return;

        bool isLastLine = lineIndex >= currentLines.Length - 1;

        if (!isLastLine)
        {
            // Còn dòng tiếp theo → luôn là "Tiếp tục"
            hintText.text = "Z: Tiep tuc  |  X: Huy";
            hintText.color = Color.gray;
        }
        else if (hasChoicesAfterDialogue)
        {
            // Dòng cuối nhưng sau đó có choices → vẫn là "Tiếp tục"
            hintText.text = "Z: Tiep tuc  |  X: Huy";
            hintText.color = Color.gray;
        }
        else
        {
            // Dòng cuối, không có choices → mới là "Đóng"
            hintText.text = "Z: Dong  |  X: Huy";
            hintText.color = Color.cyan;
        }
    }

    private IEnumerator TypeTextCoroutine(string fullText)
    {
        messageText.text = "";
        int charIndex = 0;

        while (charIndex < fullText.Length)
        {
            messageText.text += fullText[charIndex];
            charIndex++;
            yield return new WaitForSeconds(typingSpeed);
        }

        typingCoroutine = null;
    }

    private void NextLine()
    {
        lineIndex++;

        if (lineIndex >= currentLines.Length)
        {
            CloseDialogue();
            return;
        }

        DisplayLine(currentLines[lineIndex]);
    }

    private void CloseDialogue()
    {
        isOpen = false;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialoguePanel.SetActive(false);
        onDialogueComplete?.Invoke();
    }

    private void UpdateCursorDisplay()
    {
        for (int i = 0; i < cursorImages.Length; i++)
        {
            if (cursorImages[i] != null)
                cursorImages[i].enabled = (i == selectedChoiceIndex);
        }
    }

    private void ConfirmChoice()
    {
        int selected = selectedChoiceIndex;
        isChoosingOption = false;

        if (messageText != null) messageText.gameObject.SetActive(true);
        if (choicePanel != null) choicePanel.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (hintText != null) hintText.text = "";

        // ─── Mở lại di chuyển player sau khi chọn xong ───
        SetPlayerMovement(true);

        onChoiceSelected?.Invoke(selected);
    }

    /// <summary>
    /// Khóa/mở PlayerMovement. Cache lại sau lần tìm đầu tiên.
    /// </summary>
    private void SetPlayerMovement(bool enabled)
    {
        if (cachedPlayerMovement == null)
            cachedPlayerMovement = FindFirstObjectByType<PlayerMovement>();

        if (cachedPlayerMovement != null)
            cachedPlayerMovement.canMove = enabled;
    }
}