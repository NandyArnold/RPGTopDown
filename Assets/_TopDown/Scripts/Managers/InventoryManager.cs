using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    [Header("Input Settings")]

    public InputActionReference inventoryAction;


    [Header("Inventory Settings")]

    public int inventorySize = 20;

    public List<InventorySlot> slots = new List<InventorySlot>();

    Dictionary<string, InventorySlot> itemsByld = new Dictionary<string, InventorySlot>();


    [Header("UI References")]

    public GameObject inventoryPanel;

    public Transform slotsGrid;

    public GameObject slotPrefab;

    private bool isInventoryOpen = false;

    public static InventoryManager Instance { get; private set; }

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        for (int i = 0; i < inventorySize; i++)
        {
            slots.Add(new InventorySlot());
        }

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        if (inventoryAction != null)
        {
            inventoryAction.action.started += OnInventoryInput;
        }

    }




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {

        RefreshInventoryUI();

    }

    private void OnEnable()
    {
        if(inventoryAction != null)
        {
            inventoryAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (inventoryAction != null)
        {
            inventoryAction.action.Disable();
        }
    }



    private void OnDestroy()
    {
        
        if(inventoryAction != null)
        {
            inventoryAction.action.started -= OnInventoryInput;
        }

    }
    private void OnInventoryInput(InputAction.CallbackContext obj)
    {
        ToggleInventory();
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        if(inventoryPanel)
        {
            inventoryPanel.SetActive(isInventoryOpen);
        }
    }

    public bool AddItem(Item item, int quantity =1)
    {
        if(item.isStackable)

        {
            for(int i=0; i< slots.Count; i++)
            {
                if (slots[i].item == item && slots[i].quantity< item.maxStackSize)
                {
                    quantity = slots[i].AddItem(item, quantity);

                    if(quantity <= 0)
                    {
                        RefreshInventoryUI();
                        return true;
                    }

                }

            }
        }

        for(int i=0; i< slots.Count; i++)
        {

            if (slots[i].IsEmpty())
            {
                quantity = slots[i].AddItem(item, quantity);

                if(quantity <= 0)
                {
                    RefreshInventoryUI();

                    return true;
                }
            }
        }

        Debug.Log("Inventory full, can't add more " + item.itemName);
        RefreshInventoryUI();
        return false;

    }

    public void RemoveItem(Item item, int quantity = 1)
    {
        for(int i=0; i<slots.Count; i ++)
        {
            if (slots[i].item == item)
            {
                slots[i].RemoveItem(quantity);

                RefreshInventoryUI();
                
                return;
            }

        }

    }

    public void UseItem(int slotIndex)
    {
        if(slotIndex >= 0 && slotIndex<slots.Count)
        {

            if (!slots[slotIndex].IsEmpty())
            {
                slots[slotIndex].item.Use();

                RefreshInventoryUI();

            }
        }

    }

    private void RefreshInventoryUI()
    {
        if (slotsGrid == null || slotPrefab == null)
        {
            return;
        }

        foreach(Transform child in slotsGrid)
        {
            Destroy(child.gameObject);
        }

        for(int i=0; i< slots.Count; i++)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotsGrid);

            InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();
             
             if(slotUI != null)
            {
               slotUI.SetupSlot(i, slots[i]); 

             }
        }


    }

}