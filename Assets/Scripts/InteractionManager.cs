using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance;

    public bool isInteracting = false;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // Z = tương tác
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!isInteracting)
                StartInteraction();
        }

        // X = hủy hoặc mở menu
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (isInteracting)
                CancelInteraction();
            else
                ToggleMenu();
        }
    }

    void StartInteraction()
    {
        isInteracting = true;
        Debug.Log("👉 Bắt đầu tương tác");
    }

    void CancelInteraction()
    {
        isInteracting = false;
        Debug.Log("❌ Hủy tương tác");
    }

    void ToggleMenu()
    {
        Debug.Log("📜 Mở MENU RPG");
    }
}
