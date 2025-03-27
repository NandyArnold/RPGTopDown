using UnityEngine;

public class ScenePortal : MonoBehaviour, IInteractable
{
    [Header("Portal Settings")]
    public string targetSceneName;
    public string portalName;

    [Header("Interaction Settings")]
    public string interactionPrompt = "Press E to enter";
    public bool requiresInteraction = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!requiresInteraction && other.CompareTag("Player"))
        {
            UsePortal();
        }
    }
        
    public string GetInteractionPrompt()
    {
        return interactionPrompt;
    }

   
    public void Interact()
    {
        if(requiresInteraction)
        {
            UsePortal();
        }
    }


    private void UsePortal()
    {
        PlayerPrefs.SetString("LastUsedPortal", portalName);

        if(SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError("SceneTransitionManager not found in Scene!");
        }
    }

    public string GetName()
    {
        return portalName;
    }
}
