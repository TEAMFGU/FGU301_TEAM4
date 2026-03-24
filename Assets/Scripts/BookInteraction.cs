using UnityEngine;

public class BookInteraction : MonoBehaviour
{
    private bool playerInRange = false;
    private bool readToday = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !readToday)
            ReadBook();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log(readToday ? "Hôm nay đã đọc rồi" : "Nhấn E để đọc sách");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    void ReadBook()
    {
        readToday = true;

        // Cộng điểm (tuỳ bạn lưu điểm ở đâu)
        int score = PlayerPrefs.GetInt("PlayerScore", 0);
        PlayerPrefs.SetInt("PlayerScore", score + 10);
        Debug.Log("📖 Đọc sách! +10 điểm | Tổng: " + (score + 10));

        // Cập nhật quest nếu có
        DailyQuestManager.Instance.OnInteractObject("Book");
    }

    // Reset khi ngủ dậy
    public void ResetDaily() { readToday = false; }
}