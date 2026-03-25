using UnityEngine;

[CreateAssetMenu(fileName = "NPC_", menuName = "FPT Adventure/NPC Data", order = 1)]
public class NPCData : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public string npcName = "Tên NPC";
    [TextArea(3, 6)]
    public string description = "Tính cách + vai trò";

    [Header("Avatar")]
    public Sprite faceSprite;                 // Hình đại diện khuôn mặt

    [Header("Chỉ số khởi đầu")]
    public float initialAffection = 0f;        // % thiện cảm bắt đầu
    public string routeType = "Normal";        // Bad / Good / Dead / Happy / Normal

    [Header("Các chỉ số khác (dễ mở rộng)")]
    public int level = 1;                      // Level tương tác hiện tại (1-4)
    public float affection = 0f;               // Thiện cảm thực tế (sẽ +25% mỗi lần)

    [Header("Cutscene Mốc")]
    public bool unlockedCutscene25 = false;    // Mở khi đạt 25%
    public bool unlockedCutscene50 = false;    // Mở khi đạt 50%
    public bool unlockedCutscene75 = false;    // Mở khi đạt 75%
    public bool unlockedCutscene100 = false;   // Mở khi đạt 100%
}