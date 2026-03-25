using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;

    void Awake()
    {
        // Kiểm tra xem đã có bản sao nào của nhạc nền chưa
        if (instance == null)
        {
            instance = this;
            // Giữ lại Object này khi chuyển Scene
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Nếu đã có rồi thì tự hủy bản sao mới để tránh lặp nhạc
            Destroy(gameObject);
        }
    }
}