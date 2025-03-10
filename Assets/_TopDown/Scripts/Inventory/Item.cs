using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("ItemBasic Info")]

    public string itemName;

    public Sprite icon;

    [TextArea(3, 10)]

    public string description;

    [Header("Item Properties")]

    public bool isStackable = true;
    public int maxStackSize = 99;

    [Header("Item Value")]

    public int buyPrice;

    public int sellPrice;

    public virtual void Use()
    {
        Debug.Log("Using item: " + itemName);

    }



}
