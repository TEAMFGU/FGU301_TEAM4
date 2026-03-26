using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TeleportPortal : MonoBehaviour
{
    [Header("Destinations")]
    public PortalDestination[] destinations;

    [Header("UI")]
    public GameObject portalMenuUI;
    public Transform buttonContainer;
    public GameObject destinationButtonPrefab;
    public TextMeshProUGUI hintText;

    [Header("Selection Highlight")]
    public Color normalColor   = Color.white;
    public Color selectedColor = Color.yellow;

    private bool playerInRange = false;
    private bool menuOpen      = false;
    private int  selectedIndex = 0;
    private Button[] spawnedButtons;

    void Update()
    {
        if (playerInRange && !menuOpen && Input.GetKeyDown(KeyCode.E))
        {
            OpenPortalMenu();
            return;
        }

        if (!menuOpen) return;

        // ── Điều khiển phím trong menu ────────────────────────────────────────
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + destinations.Length) % destinations.Length;
            UpdateSelection();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % destinations.Length;
            UpdateSelection();
        }
        else if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
        {
            TeleportTo(destinations[selectedIndex]);
        }
        else if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePortalMenu();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (hintText != null) hintText.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (hintText != null) hintText.gameObject.SetActive(false);
            if (menuOpen) ClosePortalMenu();
        }
    }

    void OpenPortalMenu()
    {
        menuOpen      = true;
        selectedIndex = 0;

        portalMenuUI.SetActive(true);
        if (hintText != null) hintText.gameObject.SetActive(false);

        // Khóa player di chuyển
        var pm = FindFirstObjectByType<PlayerMovement>();
        if (pm != null) pm.canMove = false;

        // Xóa button cũ
        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        // Tạo button mới
        spawnedButtons = new Button[destinations.Length];
        for (int i = 0; i < destinations.Length; i++)
        {
            GameObject btn = Instantiate(destinationButtonPrefab, buttonContainer);

            var tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) tmp.text = destinations[i].displayName;

            int localIndex = i;
            Button btnComp = btn.GetComponent<Button>();
            btnComp.onClick.AddListener(() => TeleportTo(destinations[localIndex]));
            spawnedButtons[i] = btnComp;
        }

        UpdateSelection();
    }

    void UpdateSelection()
    {
        if (spawnedButtons == null) return;

        for (int i = 0; i < spawnedButtons.Length; i++)
        {
            if (spawnedButtons[i] == null) continue;

            var tmp = spawnedButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
                tmp.color = (i == selectedIndex) ? selectedColor : normalColor;
        }
    }

    void ClosePortalMenu()
    {
        menuOpen = false;
        if (portalMenuUI != null) portalMenuUI.SetActive(false);
        if (hintText != null && playerInRange) hintText.gameObject.SetActive(true);

        // Mở lại di chuyển
        var pm = FindFirstObjectByType<PlayerMovement>();
        if (pm != null) pm.canMove = true;
    }

    void TeleportTo(PortalDestination dest)
    {
        ClosePortalMenu();
        PlayerPrefs.SetString("SpawnPoint", dest.spawnPointID);
        SceneManager.LoadScene(dest.sceneName);
    }
}

[System.Serializable]
public class PortalDestination
{
    public string displayName;
    public string sceneName;
    public string spawnPointID;
}