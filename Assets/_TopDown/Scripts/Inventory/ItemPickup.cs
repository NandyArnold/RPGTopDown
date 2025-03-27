using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    public Item item;
    public int quantity = 1;

    public string GetInteractionPrompt()
    {
        return "Press E to Pick-Up" + item.itemName;
    }

    public string GetName()
    {
        return item.itemName;
    }

    public string GetNameOfObject()
    {
        return item.itemName;
    }

    public void Interact()
    {
        Pickup();
    }

    private void Pickup()
    {
        bool wasPickedUp = InventoryManager.Instance.AddItem(item, quantity);

        if(wasPickedUp)
        {
            if(QuestManager.Instance != null)
            {
                QuestManager.Instance.ItemCollected(item);
            }
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Inventory is full!");
        }




    }

}
