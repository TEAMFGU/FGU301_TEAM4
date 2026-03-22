using UnityEngine;
using UnityEngine.SceneManagement; // Thư viện này đã bao gồm quản lý Scene rồi

public class MenuHandler : MonoBehaviour
{
    void Update()
    {
        // Nếu nhấn Z thì tự động gọi hàm chuyển cảnh
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartNewGame();
        }
    }

    public void StartNewGame()
    {
        // SỬA CHỖ NÀY: Bỏ chữ "SceneManagement." đi, chỉ để SceneManager thôi
        SceneManager.LoadScene("Home");
    }
}