using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public string portalName;

    private void Start()
    {
        string lastPortal = PlayerPrefs.GetString("LastUsedPortal", "");

        if(lastPortal == portalName)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if(player != null)
            {
                player.transform.position = transform.position;

                PlayerPrefs.DeleteKey("LastUsedPortal");
            }
        }
    }
}




