using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoTeleport : MonoBehaviour
{
    public string targetScene;      // Scene đích
    public string spawnPointID;     // SpawnPoint ở scene đích

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerPrefs.SetString("SpawnPoint", spawnPointID);
            SceneManager.LoadScene(targetScene);
        }
    }
}