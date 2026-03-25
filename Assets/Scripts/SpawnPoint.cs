using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour
{
    public string spawnPointID;

    void Start()
    {
        StartCoroutine(WaitAndSpawn());
    }

    IEnumerator WaitAndSpawn()
    {
        yield return null; // Chờ 1 frame

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