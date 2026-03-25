using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Component gắn trên mỗi dòng NPC trong Status Panel.
/// </summary>
public class NPCStatusEntry : MonoBehaviour
{
    [SerializeField] private Image faceImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI affinityText;

    public void Setup(Sprite face, string npcName, int affinity)
    {
        if (faceImage != null)
        {
            faceImage.gameObject.SetActive(face != null);
            if (face != null) faceImage.sprite = face;
        }

        if (nameText != null)
            nameText.text = npcName;

        if (affinityText != null)
            affinityText.text = $"{affinity} / 100";
    }
}
