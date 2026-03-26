using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Singleton – quản lý menu in-game (nhấn X).
/// [DefaultExecutionOrder(-20)]: chạy TRƯỚC DialogueManager (order 0)
/// để xử lý X key trước khi DialogueManager có cơ hội bắt nó.
/// </summary>
[DefaultExecutionOrder(-20)]
public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject statusPanel;

    [Header("Main Menu – Cursor & Items (index 0=Status, 1=Save, 2=Quit)")]
    [SerializeField] private Image[] menuCursors;           // Btn_Play sprite, ẩn/hiện theo selectedIndex
    [SerializeField] private TextMeshProUGUI[] menuItemTexts; // "Thông tin", "Lưu Game", "Thoát"

    [Header("Status Panel")]
    [SerializeField] private TextMeshProUGUI academicScoreText;
    [SerializeField] private TextMeshProUGUI stressText;
    [SerializeField] private Transform npcListContainer;    // VerticalLayoutGroup chứa các NPCEntry
    [SerializeField] private GameObject npcEntryPrefab;     // Prefab NPCStatusEntry

    [Header("NPC Data – kéo tất cả 13 NPC ScriptableObject vào đây")]
    [SerializeField] private NPCData[] allNPCData;

    private bool isOpen;
    private bool isInStatus;
    private int selectedIndex;
    private const int MenuItemCount = 3;

    private PlayerMovement cachedPlayerMovement;

    // ─── Singleton ───────────────────────────────────────────────────────────

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

    private void Start()
    {
        if (menuPanel != null)
            menuPanel.SetActive(false);
    }

    // ─── Input ───────────────────────────────────────────────────────────────

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            // Dialogue đang mở → nhường X cho DialogueManager
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsOpen())
                return;

            if (!isOpen)
                OpenMenu();
            else if (isInStatus)
                ShowMainMenu();   // Đang xem Status → quay về main menu
            else
                CloseMenu();      // Đang ở main menu → đóng

            return;
        }

        // Chỉ xử lý navigation khi menu đang mở và ở main menu
        if (!isOpen || isInStatus) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + MenuItemCount) % MenuItemCount;
            UpdateCursorDisplay();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % MenuItemCount;
            UpdateCursorDisplay();
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            ConfirmSelection();
        }
    }

    // ─── Menu control ─────────────────────────────────────────────────────────

    private void OpenMenu()
    {
        isOpen = true;
        selectedIndex = 0;
        menuPanel.SetActive(true);
        ShowMainMenu();
        SetPlayerMovement(false);
    }

    public void CloseMenu()
    {
        isOpen = false;
        isInStatus = false;
        if (menuPanel != null) menuPanel.SetActive(false);
        SetPlayerMovement(true);
    }

    private void ShowMainMenu()
    {
        isInStatus = false;
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (statusPanel != null)  statusPanel.SetActive(false);
        UpdateCursorDisplay();
    }

    private void ShowStatus()
    {
        isInStatus = true;
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (statusPanel != null)  statusPanel.SetActive(true);
        RefreshStatusPanel();
    }

    private void ConfirmSelection()
    {
        switch (selectedIndex)
        {
            case 0: ShowStatus(); break;
            case 1: DoSaveGame(); break;
            case 2: DoQuitGame(); break;
        }
    }

    // ─── Status Panel ─────────────────────────────────────────────────────────

    private readonly List<GameObject> spawnedEntries = new List<GameObject>();

    private void RefreshStatusPanel()
    {
        if (PlayerDataManager.Instance == null)
        {
            Debug.LogWarning("[MenuManager] PlayerDataManager.Instance == null → không thể hiện status!");
            return;
        }

        if (academicScoreText != null)
            academicScoreText.text = $"Học lực: {PlayerDataManager.Instance.GetAcademicScore()}";

        if (stressText != null)
            stressText.text = $"Stress: {PlayerDataManager.Instance.GetStress()} / 100";

        if (npcListContainer == null)
        {
            Debug.LogWarning("[MenuManager] npcListContainer chưa gán trong Inspector!");
            return;
        }
        if (allNPCData == null || allNPCData.Length == 0)
        {
            Debug.LogWarning("[MenuManager] allNPCData rỗng hoặc null! Hãy kéo 13 NPC ScriptableObject vào Inspector.");
            return;
        }

        // ── Resolve đúng Content target ──
        Transform contentTarget = npcListContainer;
        ScrollRect parentSR = npcListContainer.GetComponent<ScrollRect>();
        if (parentSR != null && parentSR.content != null)
        {
            contentTarget = parentSR.content;
        }
        else
        {
            parentSR = contentTarget.GetComponentInParent<ScrollRect>();
        }

        // ── Safety: Viewport cần mask hoạt động ──
        if (parentSR != null && parentSR.viewport != null)
        {
            var viewport = parentSR.viewport;
            var mask = viewport.GetComponent<Mask>();
            if (mask != null)
            {
                var viewportImage = viewport.GetComponent<Image>();
                if (viewportImage == null || !viewportImage.enabled)
                {
                    mask.enabled = false;
                    if (viewport.GetComponent<RectMask2D>() == null)
                        viewport.gameObject.AddComponent<RectMask2D>();
                }
            }
        }

        RectTransform contentRT = contentTarget.GetComponent<RectTransform>();

        // ── Cấu hình Content RectTransform cho vertical scroll ──
        if (contentRT != null)
        {
            contentRT.anchorMin = new Vector2(0, 1);
            contentRT.anchorMax = new Vector2(1, 1);
            contentRT.pivot = new Vector2(0.5f, 1);
        }

        // ── Cấu hình VerticalLayoutGroup ──
        var vlg = contentTarget.GetComponent<VerticalLayoutGroup>();
        if (vlg == null)
            vlg = contentTarget.gameObject.AddComponent<VerticalLayoutGroup>();
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.spacing = 5;
        vlg.padding = new RectOffset(5, 5, 5, 5);

        // ── Tắt ContentSizeFitter nếu có (không đáng tin cậy khi add runtime) ──
        var csf = contentTarget.GetComponent<ContentSizeFitter>();
        if (csf != null) csf.enabled = false;

        // ── Xóa entries cũ ──
        foreach (var old in spawnedEntries)
        {
            if (old != null)
            {
                old.SetActive(false);
                Destroy(old);
            }
        }
        spawnedEntries.Clear();

        // Lấy font từ text hiện có
        TMP_FontAsset sharedFont = academicScoreText != null ? academicScoreText.font : null;

        // ── Tạo entry cho từng NPC ──
        const float ROW_HEIGHT = 60f;
        int createdCount = 0;
        foreach (var npcData in allNPCData)
        {
            if (npcData == null) continue;
            int affinity = PlayerDataManager.Instance.GetNPCAffinity(npcData.npcName);
            GameObject row = CreateNPCRow(contentTarget, sharedFont, npcData.faceSprite, npcData.npcName, affinity);
            spawnedEntries.Add(row);
            createdCount++;
        }

        // ── Tự tính Content height (không phụ thuộc ContentSizeFitter) ──
        if (contentRT != null && createdCount > 0)
        {
            float totalHeight = vlg.padding.top + vlg.padding.bottom
                              + createdCount * ROW_HEIGHT
                              + Mathf.Max(0, createdCount - 1) * vlg.spacing;
            contentRT.sizeDelta = new Vector2(0, totalHeight);
        }

        // ── Force rebuild layout ──
        Canvas.ForceUpdateCanvases();
        if (contentRT != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRT);

        // Scroll về đầu danh sách
        if (parentSR != null)
            parentSR.verticalNormalizedPosition = 1f;
    }

    /// <summary>
    /// Tạo 1 dòng NPC trong Status panel hoàn toàn bằng code (tránh lỗi prefab Canvas lồng nhau).
    /// </summary>
    private GameObject CreateNPCRow(Transform parent, TMP_FontAsset font, Sprite face, string npcName, int affinity)
    {
        // ── Row container ──
        GameObject row = new GameObject("NPC_" + npcName, typeof(RectTransform));
        row.layer = 5; // UI
        row.transform.SetParent(parent, false);

        var hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.padding = new RectOffset(5, 5, 5, 5);
        hlg.spacing = 10;
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;

        var rowLE = row.AddComponent<LayoutElement>();
        rowLE.preferredHeight = 60;

        // ── Face image ──
        GameObject faceGO = new GameObject("Face", typeof(RectTransform));
        faceGO.layer = 5;
        faceGO.transform.SetParent(row.transform, false);
        faceGO.AddComponent<CanvasRenderer>();
        var img = faceGO.AddComponent<Image>();
        img.preserveAspect = true;
        if (face != null) img.sprite = face;
        else faceGO.SetActive(false);
        var faceLE = faceGO.AddComponent<LayoutElement>();
        faceLE.preferredWidth = 48;
        faceLE.preferredHeight = 48;
        faceLE.minWidth = 48;
        faceLE.minHeight = 48;

        // ── Name text ──
        GameObject nameGO = new GameObject("Name", typeof(RectTransform));
        nameGO.layer = 5;
        nameGO.transform.SetParent(row.transform, false);
        var nameTMP = nameGO.AddComponent<TextMeshProUGUI>();
        nameTMP.text = npcName;
        nameTMP.fontSize = 20;
        nameTMP.color = Color.white;
        if (font != null) nameTMP.font = font;
        var nameLE = nameGO.AddComponent<LayoutElement>();
        nameLE.flexibleWidth = 1;

        // ── Affinity text ──
        GameObject affGO = new GameObject("Affinity", typeof(RectTransform));
        affGO.layer = 5;
        affGO.transform.SetParent(row.transform, false);
        var affTMP = affGO.AddComponent<TextMeshProUGUI>();
        affTMP.text = $"{affinity} / 100";
        affTMP.fontSize = 20;
        affTMP.color = new Color(1f, 0.82f, 0f, 1f); // Gold
        affTMP.alignment = TextAlignmentOptions.Right;
        if (font != null) affTMP.font = font;
        var affLE = affGO.AddComponent<LayoutElement>();
        affLE.preferredWidth = 100;

        return row;
    }

    // ─── Actions ──────────────────────────────────────────────────────────────

    private void DoSaveGame()
    {
        if (SaveSystem.Instance != null)
            SaveSystem.Instance.SaveGame();
    }

    private void DoQuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ─── Helpers ──────────────────────────────────────────────────────────────

    private void UpdateCursorDisplay()
    {
        for (int i = 0; i < menuCursors.Length; i++)
        {
            if (menuCursors[i] != null)
                menuCursors[i].enabled = (i == selectedIndex);
        }
    }

    private void SetPlayerMovement(bool enabled)
    {
        if (cachedPlayerMovement == null)
            cachedPlayerMovement = FindFirstObjectByType<PlayerMovement>();
        if (cachedPlayerMovement != null)
            cachedPlayerMovement.canMove = enabled;
    }
}
