using UnityEngine;
using System.Collections;

public class HomeIntro : MonoBehaviour
{
    [Header("Guide Panel Image")]
    public GameObject guideImage2;

    [Header("Player Avatar")]
    public Sprite playerAvatar;

    private bool dialogueDone = false;

    void Start()
    {
        if (guideImage2 != null) guideImage2.SetActive(false);

        // Chỉ chạy intro khi player đến từ Map000_Strange_guy
        if (PlayerPrefs.GetInt("PlayHomeIntro", 0) != 1)
            return;

        // Xóa flag ngay để lần sau vào Home không chạy lại
        PlayerPrefs.DeleteKey("PlayHomeIntro");
        PlayerPrefs.Save();

        if (DialogueManager.Instance == null)
        {
            Debug.LogError("HomeIntro: DialogueManager.Instance là null! Kéo Dialogue_Canvas prefab vào scene này.");
            return;
        }

        StartCoroutine(PlayHomeIntro());
    }

    IEnumerator PlayHomeIntro()
    {
        yield return null; // đợi SpawnPoint xong

        yield return Line("Player", playerAvatar, false,
            "đơ mơ phòng này mà dám lấy giá dị à ông già!?!?!");

        yield return Line("", null, true,
            "thì chú em cọc gòi anh đưa tới phòng không ở thì dọn đi mất cọc =)))");

        yield return Line("Player", playerAvatar, false,
            "ngu hay sao tự nhiên mất 500 lỡ r coi như tiền mua bài học thôi");

        yield return Line("Player", playerAvatar, false,
            "má nay mệt vơ lơ chắc đi ngủ đã mai tính");
        if (guideImage2 != null) guideImage2.SetActive(false);
        DialogueManager.Instance.ForceClose();
    }

    IEnumerator Line(string speaker, Sprite avatar, bool showGuide, string text)
    {
        if (guideImage2 != null) guideImage2.SetActive(showGuide);

        dialogueDone = false;
        DialogueManager.Instance.ShowCutsceneLine(speaker, avatar, text, () => dialogueDone = true);
        yield return new WaitUntil(() => dialogueDone);
    }
}