using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    public Item item;
    public int quantity = 1;

    public string GetInteractionPrompt()
    {
        return "Press E to Pick-Up" + item.itemName;
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
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Inventory is full!");
        }




    }

}
