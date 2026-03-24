using UnityEngine;

public class BedInteraction : MonoBehaviour
{
    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
            Sleep();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Nhấn E để ngủ");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    void Sleep()
    {
        // Reset quest sang ngày mới
        DailyQuestManager.Instance.OnSleep();
        Debug.Log("😴 Đi ngủ... Chào buổi sáng!");
        // Sau này có thể thêm: màn hình fade đen, animation ngủ...
    }
}