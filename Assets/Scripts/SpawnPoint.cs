using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public string spawnPointID;

    void Awake()
    {
        string target = PlayerPrefs.GetString("SpawnPoint", "");
        if (target == spawnPointID && target != "")
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                player.transform.position = transform.position;
            PlayerPrefs.DeleteKey("SpawnPoint");
        }
    }
}