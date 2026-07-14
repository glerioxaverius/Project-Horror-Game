using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<ItemData> items = new List<ItemData>();
    [SerializeField] private InventoryUI inventoryUI; 

    public void AddItem(ItemData itemData)
    {
        items.Add(itemData);
        Debug.Log($"[Inventory] Berhasil mengambil: {itemData.itemName}.");

        if (inventoryUI != null)
        {
            inventoryUI.UpdateUI(items);
        }
    }
}