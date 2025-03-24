using UnityEngine;

public class LocationTrigger : MonoBehaviour
{
    [Header("Location Settings")]
    public string locationName;
    public string locationTitle;
    public bool showNotification = true;

    private bool visited = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!visited && other.CompareTag("Player"))
        {
            visited = true;
            if(showNotification)
            {
                string title = !string.IsNullOrEmpty(locationTitle) ? locationTitle : locationName;
                Debug.Log($"Discovered area: {title}");
            }

            if(QuestManager.Instance != null)
            {
                QuestManager.Instance.LocationVisited(locationName);
            }
        }


    }


}
