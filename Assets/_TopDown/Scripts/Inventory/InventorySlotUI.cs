using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{

    [Header("UI References")]

    public Image itemIcon;

    public TextMeshProUGUI quantityText;

    public Image slotBackground;

    private int slotIndex;

    private InventorySlot slotData;

    public void SetupSlot(int index, InventorySlot data)
    {
        slotIndex = index;
        slotData = data;

        if(data.IsEmpty())
        {
            itemIcon.enabled = false;

            quantityText.gameObject.SetActive(false);
        }

        else
        {
            itemIcon.sprite = data.item.icon;

            itemIcon.enabled = true;

            if(data.quantity > 1)
            {
                quantityText.text = data.quantity.ToString();
                quantityText.gameObject.SetActive(true);
            }

            else
            {
                quantityText.gameObject.SetActive(false);

            }


        }


    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if(!slotData.IsEmpty())
            {
                InventoryManager.Instance.UseItem(slotIndex);
            }

        }

        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(!slotData.IsEmpty())
            {
                InventoryManager.Instance.RemoveItem(slotData.item, 1);
            }

        }
        
    }
  
}





