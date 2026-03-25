using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoTeleport : MonoBehaviour
{
    public string targetScene;
    public string spawnPointID;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Có object chạm vào: " + other.name + " | Tag: " + other.tag);

        if (other.CompareTag("Player"))
        {
            Debug.Log("✅ Player chạm portal → Load: " + targetScene);
            PlayerPrefs.SetString("SpawnPoint", spawnPointID);
            SceneManager.LoadScene(targetScene);
        }
    }
}