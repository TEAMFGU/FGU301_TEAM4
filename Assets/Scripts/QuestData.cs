using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Quest/QuestData")]
public class QuestData : ScriptableObject
{
    public string questID;
    public string questName;
    public string description;

    public enum QuestType { VisitScene, TalkToNPC }
    public QuestType questType;

    public string targetScene;   // Dùng khi type = VisitScene
    public string targetNPCName; // Dùng khi type = TalkToNPC
    public int rewardPoints;     // Điểm thưởng
    public string targetObjectName; // Tên object cần tương tác
}