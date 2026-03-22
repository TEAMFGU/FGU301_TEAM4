using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeleportPortal : MonoBehaviour
{
    [Header("Destinations")]
    public PortalDestination[] destinations;

    [Header("UI")]
    public GameObject portalMenuUI;
    public Transform buttonContainer;
    public GameObject destinationButtonPrefab;
    public TMPro.TextMeshProUGUI hintText;

    private bool playerInRange = false;
    private bool menuOpen = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !menuOpen)
            OpenPortalMenu();

        if (menuOpen && Input.GetKeyDown(KeyCode.Escape))
            ClosePortalMenu();
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
            ClosePortalMenu();
        }
    }

    void OpenPortalMenu()
    {
        menuOpen = true;
        portalMenuUI.SetActive(true);
        if (hintText != null) hintText.gameObject.SetActive(false);

        // Xóa button cũ (nếu có)
        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        // Tạo button cho từng điểm đến
        foreach (var dest in destinations)
        {
            GameObject btn = Instantiate(destinationButtonPrefab, buttonContainer);
            btn.GetComponentInChildren<Text>().text = dest.displayName;
            PortalDestination localDest = dest;
            btn.GetComponent<Button>().onClick.AddListener(() => TeleportTo(localDest));
        }
    }

    void ClosePortalMenu()
    {
        menuOpen = false;
        portalMenuUI.SetActive(false);
        if (hintText != null && playerInRange) hintText.gameObject.SetActive(true);
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