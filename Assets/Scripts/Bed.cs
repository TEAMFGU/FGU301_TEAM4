using UnityEngine;
using System.Collections;

public class Bed : MonoBehaviour
{
    [Header("Tuỳ chọn")]
    public Sprite bedIcon;
    public bool onlyAllowAtEvening = false;

    [Header("Indicator (ngôi sao nhấp nháy)")]
    public SpriteRenderer interactIndicator;
    public float bobSpeed   = 2f;
    public float bobHeight  = 0.15f;
    public float pulseSpeed = 3f;       

    private bool isInRange = false;
    private bool isSleeping = false;
    private Vector3 indicatorOrigin;

    // ─────────────────────────────────────────────────────────────────────────
    private void Start()
    {
        if (interactIndicator != null)
        {
            indicatorOrigin = interactIndicator.transform.localPosition;
            interactIndicator.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        AnimateIndicator();

        if (!isInRange || isSleeping) return;
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsOpen()) return;

        if (Input.GetKeyDown(KeyCode.Z))
            StartCoroutine(SleepAndSave());
    }

    // ── Bob + pulse indicator ─────────────────────────────────────────────────
    private void AnimateIndicator()
    {
        if (interactIndicator == null || !interactIndicator.gameObject.activeSelf) return;

        // Lên xuống
        float newY = indicatorOrigin.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        interactIndicator.transform.localPosition = new Vector3(
            indicatorOrigin.x, newY, indicatorOrigin.z);

        // Nhấp nháy alpha
        float alpha = Mathf.Lerp(0.4f, 1f, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
        Color c = interactIndicator.color;
        c.a = alpha;
        interactIndicator.color = c;
    }

    // ─────────────────────────────────────────────────────────────────────────
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isInRange = true;
        if (interactIndicator != null) interactIndicator.gameObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isInRange = false;
        if (interactIndicator != null) interactIndicator.gameObject.SetActive(false);
    }

    // ─────────────────────────────────────────────────────────────────────────
    IEnumerator SleepAndSave()
    {
        isSleeping = true;
        if (interactIndicator != null) interactIndicator.gameObject.SetActive(false);

        if (onlyAllowAtEvening && DayManager.Instance != null
            && DayManager.Instance.CurrentTime != TimeOfDay.Evening)
        {
            if (DialogueManager.Instance != null)
            {
                bool skip = false;
                DialogueManager.Instance.ShowCutsceneLine("", bedIcon,
                    "Chưa tối mà ngủ gì... Làm gì đi đã!", () => skip = true);
                yield return new WaitUntil(() => skip);
            }
            isSleeping = false;
            if (isInRange && interactIndicator != null)
                interactIndicator.gameObject.SetActive(true);
            yield break;
        }

        int nextDay = (DayManager.Instance != null) ? DayManager.Instance.CurrentDay + 1 : 2;
        DayManager.Instance?.SetDayTime(nextDay, TimeOfDay.Morning);

        if (SaveSystem.Instance != null) SaveSystem.Instance.SaveGame();
        else Debug.LogWarning("Bed: SaveSystem.Instance null!");

        if (DialogueManager.Instance != null)
        {
            bool done = false;
            DialogueManager.Instance.ShowCutsceneLine("", bedIcon,
                $"Game đã lưu  |  Ngày {nextDay} – Sáng bắt đầu!", () => done = true);
            yield return new WaitUntil(() => done);
        }
        else
        {
            Debug.LogWarning("Bed: DialogueManager null – kéo Dialogue_Canvas.prefab vào scene!");
            yield return new WaitForSeconds(1f);
        }

        isSleeping = false;
    }
}