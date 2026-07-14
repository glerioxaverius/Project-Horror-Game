using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    private void Awake()
    {
        ClearSlot();
    }

    // Fungsi untuk memunculkan gambar item
    public void DisplayItem(ItemData item)
    {
        if (item == null || iconImage == null) return;

        iconImage.sprite = item.itemIcon;
        iconImage.enabled = true; 
    }

    public void ClearSlot()
    {
        if (iconImage == null) return;

        iconImage.sprite = null;
        iconImage.enabled = false;
    }
}